using System;
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
            if (!folder.IsAssigned)
            {
                Debug.LogError("Cannot save texture at not defined folder reference! Texture exported into Assets Folder.");

            }

            File.WriteAllBytes(ConstructSavePath(), noiseTexture.Texture.EncodeToPNG());
            AssetDatabase.Refresh();
            
            string ConstructSavePath()
            {
                string fileName = "ExportedTexture.png";
                
                if (folder.IsAssigned)
                {
                    return Path.Combine(folder.Path, fileName);
                }
                else
                {
                    return Path.Combine(Application.dataPath,fileName);
                }
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

