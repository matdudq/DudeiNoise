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

		private Color[] redChanelTextureArray = null;
		private Color[] greenChanelTextureArray = null;
		private Color[] blueChanelTextureArray = null;
		private Color[] alphaChanelTextureArray = null;
		
		private Dictionary<Type,INoiseGeneratorModeTab> tabs = null;
		private INoiseGeneratorModeTab activeTab = null;
		
		private NoiseTextureChannel activeNoiseTextureChannel = NoiseTextureChannel.ALPHA;
		
		private Vector2 scrollPos = Vector2.zero;
		
		#endregion Variables

		#region Properties

		private NoiseSettings CurrentNoiseSettings
		{
			get
			{
				return settings.GetNoiseSettingsForChannel(activeNoiseTextureChannel);
			}
		}

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
					case NoiseTextureChannel.FULL:
						return redChanelSettingsProperty;
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
			noiseTexture = new NoiseTexture(settings);
			
			textureSettingsEditor = (NoiseTextureSettingsEditor)UnityEditor.Editor.CreateEditor(settings);
			
			redChanelSettingsProperty = textureSettingsEditor.serializedObject.FindProperty("redChannelNoiseSettings");
			greenChanelSettingsProperty  = textureSettingsEditor.serializedObject.FindProperty("greenChannelNoiseSettings");
			blueChanelSettingsProperty  = textureSettingsEditor.serializedObject.FindProperty("blueChannelNoiseSettings");
			alphaChanelSettingsProperty  = textureSettingsEditor.serializedObject.FindProperty("alphaChannelNoiseSettings");
			
			textureWindow = TextureWindow.GetWindow(new Vector2(500,0),"Noise Texture", settings.resolution, settings.filterMode);
			
			redChanelTextureArray = new Color[NoiseSettings.maximalResolution*NoiseSettings.maximalResolution];
			greenChanelTextureArray = new Color[NoiseSettings.maximalResolution*NoiseSettings.maximalResolution];
			blueChanelTextureArray = new Color[NoiseSettings.maximalResolution*NoiseSettings.maximalResolution];
			alphaChanelTextureArray = new Color[NoiseSettings.maximalResolution*NoiseSettings.maximalResolution];
			
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
			
			DrawSaveAndChannelsButtons();
			
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

		private void DrawSaveAndChannelsButtons()
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
			
			if (GUILayout.Button(saveTextureButtonGC))
			{
				noiseTexture.SaveTexture();
			}
		}
		
		private void RegenerateTextures()
		{
			noiseTexture.UpdateTextureChannel(activeNoiseTextureChannel);
			
			RegenerateCachedChanelTextures();
				
			textureWindow.Repaint();
		}
		
		private void RegenerateCachedChanelTextures()
		{
			Color[] originalTextureArray = noiseTexture.Texture.GetPixels();
			
			for (int y = 0; y < settings.resolution; y++)
			{
				for (int x = 0; x < settings.resolution; x++)
				{
					Color original = originalTextureArray[y * settings.resolution + x];
					
					Color redChanel = new Color(original.r,original.r,original.r,1.0f);
					Color greenChanel = new Color(original.g,original.g,original.g,1.0f);
					Color blueChanel = new Color(original.b,original.b,original.b,1.0f);
					Color alphaChanel = new Color(original.a,original.a,original.a,1.0f);
					
					redChanelTextureArray[y * settings.resolution + x] = redChanel;
					greenChanelTextureArray[y * settings.resolution + x] = greenChanel;
					blueChanelTextureArray[y * settings.resolution + x] = blueChanel;
					alphaChanelTextureArray[y * settings.resolution + x] = alphaChanel;
				}
			}

			switch (activeNoiseTextureChannel)
			{
				case NoiseTextureChannel.RED:
					textureWindow.UpdateTexture(redChanelTextureArray, settings.resolution, settings.filterMode);
					break;
				case NoiseTextureChannel.GREEN:
					textureWindow.UpdateTexture(greenChanelTextureArray, settings.resolution, settings.filterMode);
					break;
				case NoiseTextureChannel.BLUE:
					textureWindow.UpdateTexture(blueChanelTextureArray, settings.resolution, settings.filterMode);
					break;
				case NoiseTextureChannel.ALPHA:
					textureWindow.UpdateTexture(alphaChanelTextureArray, settings.resolution, settings.filterMode);
					break;
				case NoiseTextureChannel.FULL:
					textureWindow.UpdateTexture(originalTextureArray, settings.resolution, settings.filterMode);
					break;
			}
		}
		
		private void ChangeChannel(NoiseTextureChannel channel, bool regenerateTexture = true)
		{
			activeNoiseTextureChannel = channel;

			UpdateActiveNoiseSettingsSp();
			RegenerateTextures();
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