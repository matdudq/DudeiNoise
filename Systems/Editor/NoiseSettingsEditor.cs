#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DudeiNoise.Editor
{
	[CustomEditor(typeof(NoiseSettings))]
	public class NoiseSettingsEditor : UnityEditor.Editor
	{
		#region Variables

		private GUIStyle headerStyle = null;

		private GUIContent noiseDataHeaderGC = null;
			
		private GUIContent useAdvancedToggleGC    = null;
		private GUIContent customPatternsHeaderGC = null;
		
		private SerializedProperty positionOffsetSP = null;
		private SerializedProperty rotationOffsetSP = null;
		private SerializedProperty scaleOffsetSP    = null;
			
		private SerializedProperty tillingPeriodSP  = null;
		private SerializedProperty tillingEnabledSP = null;

		private SerializedProperty dimensionsSP            = null;
		private SerializedProperty octavesSP               = null;
		private SerializedProperty lacunaritySP            = null;
		private SerializedProperty persistenceSP           = null;
		private SerializedProperty woodPatternMultiplierSP = null;
		
		private SerializedProperty turbulenceSP = null;
		
		private SerializedProperty noiseTypeSP = null;
		
		private float frequencyValue = 0;
		
		private bool useAdvancedSpaceSettings = false;

		#endregion Variables

		#region Public methods

		public void OnEnable()
		{
			noiseDataHeaderGC = new GUIContent("Noise settings");
				
			useAdvancedToggleGC = new GUIContent("Custom space settings");
			customPatternsHeaderGC = new GUIContent("Custom patterns");
			
			positionOffsetSP = serializedObject.FindProperty("positionOffset");
			rotationOffsetSP = serializedObject.FindProperty("rotationOffset");
			scaleOffsetSP = serializedObject.FindProperty("scaleOffset");
			tillingEnabledSP = serializedObject.FindProperty("tillingEnabled");
			tillingPeriodSP = serializedObject.FindProperty("tillingPeriod");
			dimensionsSP = serializedObject.FindProperty("dimensions");
			octavesSP = serializedObject.FindProperty("octaves");
			lacunaritySP = serializedObject.FindProperty("lacunarity");
			persistenceSP = serializedObject.FindProperty("persistence");
			woodPatternMultiplierSP = serializedObject.FindProperty("woodPatternMultiplier");
			turbulenceSP = serializedObject.FindProperty("turbulence");
			noiseTypeSP = serializedObject.FindProperty("noiseType");
			
			frequencyValue = scaleOffsetSP.vector3Value.x;
		}

		public override void OnInspectorGUI()
		{
			if (headerStyle == null)
			{
				headerStyle = new GUIStyle(GUI.skin.label)
				{
					fontStyle = FontStyle.Bold
				};
			}
			
			EditorGUI.BeginChangeCheck();

			
			DrawNoiseTypeTab();
			DrawSpaceSettings();
			DrawLayersSettings();
			DrawCustomPatternsTab();
			
		}
		
		#endregion Public methods

		#region Private methods

		private void DrawCustomPatternsTab()
		{
			EditorGUI.BeginChangeCheck();

			GUILayout.Label(customPatternsHeaderGC,headerStyle);
            
			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(woodPatternMultiplierSP);
			EditorGUILayout.PropertyField(turbulenceSP);
			
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
				EditorUtility.SetDirty(target);
			}
		}

		private void DrawNoiseTypeTab()
		{
			GUILayout.Label(noiseDataHeaderGC,headerStyle);
            			
			EditorGUILayout.Space();
            
			EditorGUILayout.PropertyField(noiseTypeSP);
			EditorGUILayout.PropertyField(tillingEnabledSP);

			if (tillingEnabledSP.boolValue)
			{
				EditorGUILayout.PropertyField(tillingPeriodSP);
				tillingPeriodSP.intValue = Mathf.RoundToInt(frequencyValue);
				float halfOfFrequency = frequencyValue * 0.5f;
				positionOffsetSP.vector3Value = new Vector3(halfOfFrequency,halfOfFrequency,halfOfFrequency);
			}
			
			EditorGUILayout.Space();
		}

		private void DrawSpaceSettings()
		{
			EditorGUILayout.PropertyField(dimensionsSP);
			useAdvancedSpaceSettings = EditorGUILayout.Toggle(useAdvancedToggleGC, useAdvancedSpaceSettings);
			if (useAdvancedSpaceSettings)
			{
				EditorGUILayout.PropertyField(positionOffsetSP);
				EditorGUILayout.PropertyField(rotationOffsetSP);
				EditorGUILayout.PropertyField(scaleOffsetSP);
				if (tillingEnabledSP.boolValue)
				{
					frequencyValue = Mathf.RoundToInt(scaleOffsetSP.vector3Value.x);
				}
				else
				{
					frequencyValue = scaleOffsetSP.vector3Value.x;

				}
			}
			else
			{
				scaleOffsetSP.vector3Value = Vector3.back;
				if (tillingEnabledSP.boolValue)
				{
					frequencyValue = Mathf.RoundToInt(EditorGUILayout.FloatField("Frequency", frequencyValue));
				}
				else
				{
					frequencyValue = EditorGUILayout.FloatField("Frequency", frequencyValue);
				}

				scaleOffsetSP.vector3Value = frequencyValue * Vector3.one;
			}

			EditorGUILayout.Space();
		}

		private void DrawLayersSettings()
		{
			EditorGUILayout.PropertyField(octavesSP);
			EditorGUILayout.PropertyField(lacunaritySP);
			EditorGUILayout.PropertyField(persistenceSP);
            
			EditorGUILayout.Space();
		}

		#endregion Private methods
		
	}
}
#endif