using DudeiNoise.Editor;
using UnityEditor;
using UnityEditor.Callbacks;

namespace DudeiNoise
{
    internal static class NoiseTextureSettingsAssetHandler
    {
        #region Editor

#if UNITY_EDITOR
        [OnOpenAsset(2)]
        public static bool OnOpenAsset(int id, int line)
        {
            NoiseTextureSettings target = EditorUtility.InstanceIDToObject(id) as NoiseTextureSettings;
            if (target != null)
            {
                NoiseGeneratorWindow.Open(target);
                return true;
            }

            return false;
        }			
#endif
        #endregion Editor
    }
}