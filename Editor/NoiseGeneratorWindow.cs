#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using DudeiNoise.Editor.Utilities;
using UnityEditor;
using UnityEngine;

namespace DudeiNoise.Editor
{
	public partial class NoiseGeneratorWindow : EditorWindow
	{
		#region Variables

		private NoiseTextureSettingsEditor textureSettingsEditor = null;
		private NoiseTextureSettings settings = null;
		
		private TextureWindow textureWindow = null;
		
		private NoiseTexture noiseTexture = null;
		
		private Dictionary<Type,INoiseGeneratorModeTab> tabs = null;
		private INoiseGeneratorModeTab activeTab = null;
		
		private NoiseTextureChannel activeNoiseTextureChannel = NoiseTextureChannel.ALPHA;
		
		private Vector2 scrollPos = Vector2.zero;
		
		#endregion Variables

		#region Properties

		private SerializedProperty CurrentNoiseSettingsSP
		{
			get
			{
				switch (activeNoiseTextureChannel)
				{
					case NoiseTextureChannel.RED:
						return redChanelSettingsProperty;
					case NoiseTextureChannel.GREEN:
						return greenChanelSettingsProperty;
					case NoiseTextureChannel.BLUE:
						return blueChanelSettingsProperty;
					case NoiseTextureChannel.ALPHA:
						return alphaChanelSettingsProperty;
				}
				
				Debug.Log( $"Something goes wrong with defined channel {activeNoiseTextureChannel}");
				return null;
			}
		}
		
		#endregion Properties
		
		#region Unity methods

		private void OnGUI()
		{
			if(!stylesInitialized)
			{
				InitializeStyles();
				stylesInitialized = true;
			}
			
			DrawEditorWindow();
		}

		private void OnDestroy()
		{
			textureWindow.Close();
		}

		#endregion Unity methods
		
		#region Public methods

		public static void Open(NoiseTextureSettings settings)
		{
			Vector2 windowSize = new Vector2(500, 600);
			
			NoiseGeneratorWindow window = GetWindow<NoiseGeneratorWindow>("Noise Texture Generator");
			
			window.position = new Rect(0,0,windowSize.x, windowSize.y);
			window.minSize = windowSize;
			window.maxSize = windowSize;

			window.Initialize(settings);
			
			window.Show();
		}

		#endregion Public methods

		#region Private methods

 		[MenuItem("Noise Generator Window", menuItem = "Tools/Noise Generator Window")]
        private static void ShowWindow()
        {
            List<NoiseTextureSettings> contentDownloaders = EditorGameExtensions.LoadProjectAssetsByType<NoiseTextureSettings>();

			NoiseTextureSettings newPreset = contentDownloaders.Count == 0 ? null : contentDownloaders[0];

            if (newPreset == null)
            {
	            Debug.Log( "Light-mapping Helper setup object have to be defined once inside a project. Creating deafault one ...");

                newPreset = CreateInstance<NoiseTextureSettings>();
                AssetDatabase.CreateAsset(newPreset, "Assets/" + nameof(NoiseTextureSettings) + ".asset");

                EditorUtility.SetDirty(newPreset);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            Open(newPreset);
        }

		private void Initialize(NoiseTextureSettings settings)
		{
			this.settings = settings;
			
			textureSettingsEditor = (NoiseTextureSettingsEditor)UnityEditor.Editor.CreateEditor(settings);
			
			redChanelSettingsProperty = textureSettingsEditor.serializedObject.FindProperty("redChannelNoiseSettings");
			greenChanelSettingsProperty  = textureSettingsEditor.serializedObject.FindProperty("greenChannelNoiseSettings");
			blueChanelSettingsProperty  = textureSettingsEditor.serializedObject.FindProperty("blueChannelNoiseSettings");
			alphaChanelSettingsProperty  = textureSettingsEditor.serializedObject.FindProperty("alphaChannelNoiseSettings");

			noiseTexture = new NoiseTexture(settings.resolution);
			textureWindow = TextureWindow.GetWindow(new Vector2(500,0),"Noise Texture", noiseTexture);
			
			InitielizeContents();
			
			ChangeChannel(NoiseTextureChannel.RED);
			
			tabs = new Dictionary<Type, INoiseGeneratorModeTab>()
			{
				{typeof(CustomSpaceTab), new CustomSpaceTab(this)},
				{typeof(TillingTab), new TillingTab(this)}
			};

			if (tillingEnabledSP.boolValue)
			{
				SwitchTab(typeof(TillingTab));
			}
			else
			{
				SwitchTab(typeof(CustomSpaceTab));
			}

		}
		
		private void SwitchTab(Type tab)
		{
			if (activeTab == tabs[tab])
			{
				return;
			}
			
			activeTab = tabs[tab];

			UpdateActiveNoiseSettingsSp();
			RegenerateTextures();
			
			activeTab?.OnTabEnter();
		}
		
		private void SwitchTab(INoiseGeneratorModeTab tab)
		{
			if (activeTab == tab)
			{
				return;
			}
			
			activeTab = tab;
			
			RegenerateTextures();

			activeTab?.OnTabEnter();
		}
		
		private void DrawEditorWindow()
		{
			EditorGUI.BeginChangeCheck();
			
			scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandHeight(true));
			
			EditorGUILayout.BeginVertical();
			
			textureSettingsEditor.DrawCustomInspector();

			DrawWindowTabs();
		
			if (EditorGUI.EndChangeCheck())
			{
				SetDirty();
				RegenerateTextures();
			}

			EditorGUILayout.EndVertical();
			EditorGUILayout.EndScrollView();
			
			DrawChannelsButtons();
			DrawSaveButton();
			
			EditorGUILayout.Space(3);
		}

		private void DrawWindowTabs()
		{
			GUILayout.BeginVertical(sectionStyle);

			GUILayout.BeginHorizontal();
			
			foreach (INoiseGeneratorModeTab tab in tabs.Values)
			{
				if (tab.DrawButton())
				{
					SwitchTab(tab);
				}
			}
			
			GUILayout.EndHorizontal();

			activeTab.DrawInspector();
			
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
		}

		private void DrawSaveButton()
		{
			if (GUILayout.Button(saveTextureButtonGC))
			{
				noiseTexture.SaveTextureAtFolder(settings.exportFolder);
			}
		}
		private void DrawChannelsButtons()
		{
			GUILayout.BeginHorizontal();
			
			if (GUILayout.Button(redChannelButtonGC))
			{
				ChangeChannel(NoiseTextureChannel.RED);
			}
			
			if (GUILayout.Button(blueChannelButtonGC))
			{
				ChangeChannel(NoiseTextureChannel.BLUE);
			}

			if (GUILayout.Button(greenChannelButtonGC))
			{
				ChangeChannel(NoiseTextureChannel.GREEN);
			}

			if (GUILayout.Button(alphaChannelButtonGC))
			{
				ChangeChannel(NoiseTextureChannel.ALPHA);
			}
			
			GUILayout.EndHorizontal();
		}
		
		private void ChangeChannel(NoiseTextureChannel channel)
		{
			activeNoiseTextureChannel = channel;

			UpdateActiveNoiseSettingsSp();
			RegenerateTextures();
		}
		
		private void RegenerateTextures()
		{
			noiseTexture.GenerateNoiseForChanelInEditor(settings.GetNoiseSettingsForChannel(activeNoiseTextureChannel), activeNoiseTextureChannel, this , OnNoiseGenerated);

			void OnNoiseGenerated(NoiseTexture texture)
			{
				textureWindow.RepaintForChannel(activeNoiseTextureChannel);
			}
		}
		private void SetDirty()
		{
			textureSettingsEditor.serializedObject.ApplyModifiedProperties();
			EditorUtility.SetDirty(settings);
		}
		
		#endregion Private methods
	}
}
#endif 