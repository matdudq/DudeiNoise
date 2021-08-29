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
		
		private Dictionary<Type,INoiseGeneratorTab> tabs = null;
		private INoiseGeneratorTab activeTab = null;
		
		private NoiseTextureChannel activeNoiseTextureChannel = NoiseTextureChannel.ALPHA;
		
		private Vector2 scrollPos = Vector2.zero;
		
		private float frequencyValue = 0;
			
		private bool isNoiseTypeSectionFolded = false;
		private bool isSpaceSectionFolded = false;
		private bool isLayersSectionFolded = false;
		private bool isCustomPatternsSectionFolded = false;
		private bool isFalloffSectionFolded = false;
		
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
			
			tabs = new Dictionary<Type, INoiseGeneratorTab>()
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
		
		private void SwitchTab(INoiseGeneratorTab tab)
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

			DrawWindowSections();
		
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

		private void DrawWindowSections()
		{
			GUILayout.BeginVertical(sectionStyle);
			
			DrawNoiseTypeSection();
			DrawSpaceSettingsSection();
			DrawLayersSettingsSection();
			DrawCustomPatternsSection();
			DrawFalloffSection();
			
			EditorGUILayout.EndVertical();
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
			noiseTexture.Resize(settings.resolution);
			noiseTexture.SetFilterMode(settings.filterMode);
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
		
		private void DrawNoiseTypeSection()
		{
			isNoiseTypeSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isNoiseTypeSectionFolded, noiseTypeSectionHeaderGC);
			
			if (isNoiseTypeSectionFolded)
			{
				GUILayout.BeginVertical(sectionStyle);
				GUILayout.Space(10);
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(noiseTypeSP);
				GUILayout.EndHorizontal();
				
				GUILayout.Space(10);
				GUILayout.EndVertical();
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
		}

		private void DrawSpaceSettingsSection()
		{
			isSpaceSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isSpaceSectionFolded, spaceSectionHeaderGC);

			if (isSpaceSectionFolded)
			{
				GUILayout.BeginVertical();
				
				GUILayout.BeginHorizontal();
			
				foreach (INoiseGeneratorTab tab in tabs.Values)
				{
					if (tab.DrawButton())
					{
						SwitchTab(tab);
					}
				}
			
				GUILayout.EndHorizontal();
				
				GUILayout.EndVertical();
				
				GUILayout.BeginVertical();

				activeTab.DrawTabContent();
				
				GUILayout.EndVertical();
			}
				
			EditorGUILayout.EndFoldoutHeaderGroup();
		}

		private void DrawCustomSpaceSectionContent()
		{
			GUILayout.BeginVertical(sectionStyle);
			GUILayout.Space(10);
					
			GUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(positionOffsetSP);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(rotationOffsetSP);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			EditorGUILayout.PropertyField(scaleOffsetSP);
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			GUILayout.EndVertical();
		}
		
		private void DrawTillingSectionContent()
		{
			GUILayout.BeginVertical(sectionStyle);
			GUILayout.Space(10);
			
			GUILayout.BeginHorizontal();
			
			frequencyValue = scaleOffsetSP.vector3Value.x;
			frequencyValue = Mathf.Max(EditorGUILayout.IntField("Frequency", (int)frequencyValue),1);

			tillingPeriodSP.intValue = Mathf.RoundToInt(frequencyValue);
			float halfOfFrequency = frequencyValue * 0.5f;
			positionOffsetSP.vector3Value = new Vector3(halfOfFrequency, halfOfFrequency, halfOfFrequency);
			
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			scaleOffsetSP.vector3Value = frequencyValue * Vector3.one;
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			GUILayout.EndVertical();
		}

		private void DrawLayersSettingsSection()
		{
			isLayersSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isLayersSectionFolded, octavesSectionHeaderGC);
			
			if (isLayersSectionFolded)
			{
				GUILayout.BeginVertical(sectionStyle);
				GUILayout.Space(10);
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(octavesSP);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(lacunaritySP);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(persistenceSP);
				GUILayout.EndHorizontal();

				GUILayout.Space(10);
				GUILayout.EndVertical();
			}
			
			EditorGUILayout.EndFoldoutHeaderGroup();
		}

		private void DrawCustomPatternsSection()
		{
			isCustomPatternsSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isCustomPatternsSectionFolded, customPatternsSectionHeaderGC);

			if (isCustomPatternsSectionFolded)
			{
				GUILayout.BeginVertical(sectionStyle);
				GUILayout.Space(10);
                    
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(woodPatternMultiplierSP);
				GUILayout.EndHorizontal();
                    
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(turbulenceSP);
				GUILayout.EndHorizontal();

				GUILayout.Space(10);
				GUILayout.EndVertical();
			}
                
			EditorGUILayout.EndFoldoutHeaderGroup();
		}
		
		private void DrawFalloffSection()
		{
			isFalloffSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isFalloffSectionFolded, falloffSectionHeaderGC);
			
			if (isFalloffSectionFolded)
			{
				GUILayout.BeginVertical(sectionStyle);
				GUILayout.Space(10);
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(falloffEnabledSP);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(falloffParameterSP);
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(falloffShiftSP);
				GUILayout.EndHorizontal();
				
				GUILayout.Space(10);
				GUILayout.EndVertical();
			}
			
			EditorGUILayout.EndFoldoutHeaderGroup();
		}
		
		#endregion Private methods
	}
}
#endif 