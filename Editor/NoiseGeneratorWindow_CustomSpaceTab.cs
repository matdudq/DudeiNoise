#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DudeiNoise.Editor
{
	public partial class NoiseGeneratorWindow
	{
		private class CustomSpaceTab : INoiseGeneratorModeTab
		{
			private NoiseGeneratorWindow owner;

			private bool isNoiseTypeSectionFolded = true;
			private bool isSpaceSectionFolded = true;
			private bool isOctavesSectionFolded = true;
			private bool isCustomPatternsSectionFolded = true;
			private bool isFalloffSectionFolded = true;

			public CustomSpaceTab(NoiseGeneratorWindow owner)
			{
				this.owner = owner;
			}

			public void OnTabEnter()
			{
				owner.tillingEnabledSP.boolValue = false;
				owner.SetDirty();
				
				owner.RegenerateTextures();
			}
			
			public void DrawInspector()
			{
				DrawNoiseTypeSection();
				DrawSpaceSettingsSection();
				DrawOctavesSettings();
				DrawCustomPatternsTab();
			}

			public bool DrawButton()
			{
				return GUILayout.Button(owner.customSpaceTabButtonGC);
			}
			
			private void DrawCustomPatternsTab()
			{
				isCustomPatternsSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isCustomPatternsSectionFolded, owner.customPatternsSectionHeaderGC);

				if (isCustomPatternsSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.woodPatternMultiplierSP);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.turbulenceSP);
					GUILayout.EndHorizontal();

					GUILayout.Space(10);
					GUILayout.EndVertical();
				}	
				
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			private void DrawNoiseTypeSection()
			{
				isNoiseTypeSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isNoiseTypeSectionFolded, owner.noiseTypeSectionHeaderGC);
				
				if (isNoiseTypeSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.noiseTypeSP);				
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.dimensionsSP);
					GUILayout.EndHorizontal();


					GUILayout.Space(10);
					GUILayout.EndVertical();
				}
				
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			private void DrawSpaceSettingsSection()
			{
				isSpaceSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isSpaceSectionFolded, owner.spaceSectionHeaderGC);

				if (isSpaceSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.positionOffsetSP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.rotationOffsetSP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.scaleOffsetSP);
					GUILayout.EndHorizontal();

					GUILayout.Space(10);
					GUILayout.EndVertical();
				}
				
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			private void DrawOctavesSettings()
			{
				isOctavesSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isOctavesSectionFolded, owner.octavesSectionHeaderGC);
				
				if (isOctavesSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.octavesSP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.lacunaritySP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.persistenceSP);
					GUILayout.EndHorizontal();

					GUILayout.Space(10);
					GUILayout.EndVertical();
				}

				EditorGUILayout.EndFoldoutHeaderGroup();
			}
			
			private void DrawFalloffSection()
			{
				isFalloffSectionFolded = EditorGUILayout.BeginFoldoutHeaderGroup(isFalloffSectionFolded, owner.falloffSectionHeaderGC);
				
				if (isFalloffSectionFolded)
				{
					GUILayout.BeginVertical(owner.sectionStyle);
					GUILayout.Space(10);
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.falloffEnabledSP);
					GUILayout.EndHorizontal();

					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.falloffParameterSP);
					GUILayout.EndHorizontal();
					
					GUILayout.BeginHorizontal();
					EditorGUILayout.PropertyField(owner.falloffShiftSP);
					GUILayout.EndHorizontal();
					
					GUILayout.Space(10);
					GUILayout.EndVertical();
				}
				
				EditorGUILayout.EndFoldoutHeaderGroup();
			}
		}
	}
}
#endif