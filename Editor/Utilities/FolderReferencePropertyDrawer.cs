#if UNITY_EDITOR
using System.IO;
using DudeiNoise.Utilities;
using UnityEngine;
using UnityEditor;

namespace DudeiNoise.Editor.Utilities
{
    /// <summary>
    /// Custom property drawer that allows us to use 
    /// <see cref="FolderReference"/> as a folder-only asset field.
    /// </summary>
    [CustomPropertyDrawer(typeof(FolderReference))]
    public class FolderReferencePropertyDrawer : PropertyDrawer
    {
        #region Variables

        private bool isInitialized;
        private SerializedProperty guidSP;
        private Object folderAsset;

        #endregion Variables

        #region Public methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!isInitialized)
            {
                Initialize(property);
            }

            Event ev = Event.current;

            GUIContent guiContent = EditorGUIUtility.ObjectContent(folderAsset, typeof(DefaultAsset));

            Rect rect = EditorGUI.PrefixLabel(position, label);
            Rect textFieldRect = rect;

            Rect objectFieldRect = rect;
            objectFieldRect.x = textFieldRect.xMax - 19f;
            objectFieldRect.y += 1.0f;
            objectFieldRect.height = objectFieldRect.height - 2;
            objectFieldRect.width = 18f;

            GUIStyle textFieldStyle = new GUIStyle("TextField")
            {
                imagePosition = folderAsset ? ImagePosition.ImageLeft : ImagePosition.TextOnly
            };

            if (objectFieldRect.Contains(ev.mousePosition) && ev.type == EventType.MouseDown)
            {
                //Object field press.
                string path = EditorUtility.OpenFolderPanel("Select a folder", "Assets", "");

                if (path.Contains(Application.dataPath))
                {
                    path = "Assets" + path.Substring(Application.dataPath.Length);
                    folderAsset = AssetDatabase.LoadAssetAtPath(path, typeof(DefaultAsset));
                    guidSP.stringValue = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(folderAsset));
                    property.serializedObject.ApplyModifiedProperties();
                }
                else
                {
                    Debug.LogError("The path must be in the Assets folder");
                }

                ev.Use();
            }

            if (GUI.Button(textFieldRect, guiContent, textFieldStyle) && folderAsset)
            {
                EditorGUIUtility.PingObject(folderAsset);
            }

            if (GUI.Button(objectFieldRect, "", GUI.skin.GetStyle("ObjectFieldButton")))
            {
                //Just for show.
            }

            if (textFieldRect.Contains(ev.mousePosition))
            {
                //Drag and drop support for folders.
                if (ev.type == EventType.DragUpdated)
                {
                    Object reference = DragAndDrop.objectReferences[0];
                    string path = AssetDatabase.GetAssetPath(reference);
                    DragAndDrop.visualMode = Directory.Exists(path)
                        ? DragAndDropVisualMode.Copy
                        : DragAndDropVisualMode.Rejected;

                    ev.Use();
                }
                else if (ev.type == EventType.DragPerform)
                {
                    Object reference = DragAndDrop.objectReferences[0];
                    string path = AssetDatabase.GetAssetPath(reference);

                    if (Directory.Exists(path))
                    {
                        folderAsset = reference;
                        guidSP.stringValue = AssetDatabase.AssetPathToGUID(path);
                        property.serializedObject.ApplyModifiedProperties();
                    }

                    ev.Use();
                }
            }
        }

        #endregion Public methods

        #region Private methods

        private void Initialize(SerializedProperty property)
        {
            guidSP = property.FindPropertyRelative("guid");
            folderAsset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guidSP.stringValue));
            isInitialized = true;
        }

        #endregion Private methods
    }
}
#endif