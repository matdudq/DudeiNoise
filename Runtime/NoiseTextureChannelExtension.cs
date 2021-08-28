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
        
        #endregion Public methods
    }
}

