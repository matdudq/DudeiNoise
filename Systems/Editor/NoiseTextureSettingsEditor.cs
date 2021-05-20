#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DudeiNoise.Editor
{
		[CustomEditor(typeof(NoiseTextureSettings))]
    	public class NoiseTextureSettingsEditor : UnityEditor.Editor
    	{
			#region Variables

			private GUIContent textureDataHeaderGC = null;

			private GUIStyle headerStyle = null;

			private SerializedProperty exportFolderSP = null;
			private SerializedProperty resolutionSP = null;
			private SerializedProperty colorGradientSP = null;
			private SerializedProperty filterModeSP = null;
			
			#endregion Variables
			
    		#region Unity methods

			public void OnEnable()
			{
				textureDataHeaderGC = new GUIContent("Texture settings");
				
				exportFolderSP = serializedObject.FindProperty("exportFolder");
				resolutionSP = serializedObject.FindProperty("resolution");
				colorGradientSP = serializedObject.FindProperty("colorGradient");
				filterModeSP = serializedObject.FindProperty("filterMode");
				
			}

			public override void OnInspectorGUI()
    		{
				if (GUILayout.Button("Open Generator"))
    			{
    				NoiseGeneratorWindow.Open(target as NoiseTextureSettings);
    			}
    		}
			
    		#endregion Unity methods

            #region Public methods

            public void DrawCustomInspector()
            {
	            if (headerStyle == null)
	            {
		            headerStyle = new GUIStyle(GUI.skin.label)
		            {
			            fontStyle = FontStyle.Bold
		            };
	            }
				
	            serializedObject.Update();
				
	            GUILayout.Label(textureDataHeaderGC,headerStyle);

	            EditorGUI.BeginChangeCheck();
				
	            EditorGUILayout.PropertyField(exportFolderSP);
	            EditorGUILayout.PropertyField(resolutionSP);
	            EditorGUILayout.PropertyField(colorGradientSP);
	            EditorGUILayout.PropertyField(filterModeSP);
				
	            EditorGUILayout.Space();
				
	            if (EditorGUI.EndChangeCheck())
	            {
		            serializedObject.ApplyModifiedProperties();
		            EditorUtility.SetDirty(target);
	            }
            }

            #endregion Public methods
        }
}		
#endif