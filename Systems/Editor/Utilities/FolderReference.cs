#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace DudeiNoise.Editor.Utilities
{
    /// <summary>
    /// Folder reference for editor scripts.
    /// </summary>
    [System.Serializable]
    public class FolderReference
    {
        #region Variables

        /// <summary>
        /// GUID of the folder.
        /// </summary>
        [SerializeField]
        private string guid = string.Empty;

        #endregion Variables

        #region Properties

        /// <inheritdoc cref="guid"/>
        public string GUID
        {
            get
            {
                return guid;
            }
        }

        /// <summary>
        /// Returns TRUE if the folder is assigned.
        /// </summary>
        public bool IsAssigned
        {
            get
            {
                return !string.IsNullOrEmpty(guid);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Path to the folder.
        /// Warning : this is EDITOR ONLY!
        /// </summary>
        public string Path => AssetDatabase.GUIDToAssetPath(guid);


        /// <summary>
        /// Asset of the folder.
        /// Warning : this is EDITOR ONLY!
        /// </summary>
        public DefaultAsset Asset => AssetDatabase.LoadAssetAtPath<DefaultAsset>(Path);
#endif

        #endregion Properties
    }
}
#endif