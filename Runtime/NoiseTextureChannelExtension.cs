using System.IO;
using DudeiNoise.Utilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace DudeiNoise
{
    public static class NoiseTextureExtension
    {
        #region Public methods

        public static void SaveTextureAtFolder(this NoiseTexture noiseTexture, FolderReference folder)
        {
            if (folder.IsAssigned)
            {
                File.WriteAllBytes(ConstructSavePath(), noiseTexture.Texture.EncodeToPNG());
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("Cannot save texture! Export folder not set up.");
            }

            string ConstructSavePath()
            {
                return Path.Combine(folder.Path, noiseTexture.Texture.name);
            }
        }

        public static ColorWriteMask ToColorWriteMask(this NoiseTextureChannel textureChannel)
        {
            switch (textureChannel)
            {
                case NoiseTextureChannel.RED:
                    return ColorWriteMask.Red;
                case NoiseTextureChannel.GREEN:
                    return ColorWriteMask.Green;
                case NoiseTextureChannel.BLUE:
                    return ColorWriteMask.Blue;
                case NoiseTextureChannel.ALPHA:
                    return ColorWriteMask.Alpha;
            }

            Debug.LogError("Wrong Noise texture Channel value, cannot to convert to mask!");
            return ColorWriteMask.All;
        }

        #endregion Public methods
    }
}

