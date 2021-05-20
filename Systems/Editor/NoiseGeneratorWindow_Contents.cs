#if UNITY_EDITOR
using UnityEngine;

namespace DudeiNoise.Editor
{
    public partial class NoiseGeneratorWindow
    {
        #region Variables

        private GUIContent tillingTabButtonGC = null;
        private GUIContent customSpaceTabButtonGC = null;

        private GUIContent noiseTypeSectionHeaderGC = null;
        private GUIContent customPatternsSectionHeaderGC = null;
        private GUIContent frequencySectionHeaderGC = null;
        private GUIContent octavesSectionHeaderGC = null;
        private GUIContent spaceSectionHeaderGC = null;

        private GUIContent redChannelButtonGC = null;
        private GUIContent greenChannelButtonGC = null;
        private GUIContent blueChannelButtonGC = null;
        private GUIContent alphaChannelButtonGC = null;

        private GUIContent saveTextureButtonGC = null;

        #endregion Variables

        #region Private methods

        private void InitielizeContents()
        {
            tillingTabButtonGC = new GUIContent("Tilling Mode");
            customSpaceTabButtonGC = new GUIContent("Custom Space Mode");

            noiseTypeSectionHeaderGC = new GUIContent("Noise Type");
            customPatternsSectionHeaderGC = new GUIContent("Custom patterns");
            frequencySectionHeaderGC = new GUIContent("Frequency settings");
            octavesSectionHeaderGC = new GUIContent("Octaves settings");
            spaceSectionHeaderGC = new GUIContent("Space settings");

            redChannelButtonGC = new GUIContent("Red channel");
            greenChannelButtonGC = new GUIContent("Green channel");
            blueChannelButtonGC = new GUIContent("Blue channel");
            alphaChannelButtonGC = new GUIContent("Alpha channel");
            
            saveTextureButtonGC = new GUIContent("Save Texture");
        }

        #endregion Private methods
    }
}
#endif