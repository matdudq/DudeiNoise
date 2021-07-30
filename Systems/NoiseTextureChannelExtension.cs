using UnityEngine.Rendering;

namespace DudeiNoise
{
    public static class NoiseTextureExtension
    {
        public static ColorWriteMask ToColorWriteMask(this NoiseTextureChannel textureChannel)
        {
            return (ColorWriteMask)textureChannel;
        }
    }
}

