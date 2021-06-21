using System.IO;
using UnityEditor;
using UnityEngine;

namespace DudeiNoise
{
	public class NoiseTexture
	{
		#region Variables

		private NoiseTextureSettings settings = null;

		private Color[] textureValues = null;

		private float[,] noiseBuffer = null;
		
		private Texture2D texture = null;

		#endregion Variables

		#region Properties

		public Texture2D Texture
		{
			get
			{
				return texture;
			}
		}
		

		#endregion Properties
		
		#region Constructor
		
		public NoiseTexture(NoiseTextureSettings generatorSettings)
		{
			this.settings = generatorSettings;
			this.textureValues = new Color[NoiseSettings.maximalResolution * NoiseSettings.maximalResolution];
			this.noiseBuffer = new float[NoiseSettings.maximalResolution, NoiseSettings.maximalResolution];
			texture = new Texture2D(settings.resolution, settings.resolution, TextureFormat.RGBA32, false)
			{
				name = "Noise",
				filterMode = settings.filterMode,
				wrapMode = TextureWrapMode.Clamp
			};
			
			UpdateTextureChannel(NoiseTextureChannel.ALPHA);
		}
		
		#endregion

		#region Public methods

		public float GetRedChanelProbe(float x, float y)
		{
			x = Mathf.Clamp01(x);
			y = Mathf.Clamp01(y);
			
			return texture.GetPixelBilinear(x, y).r;
		}
		
		public float GetGreenChanelProbe(float x, float y)
		{
			x = Mathf.Clamp01(x);
			y = Mathf.Clamp01(y);
			
			return texture.GetPixelBilinear(x, y).g;
		}
		
		public float GetBlueChanelProbe(float x, float y)
		{
			x = Mathf.Clamp01(x);
			y = Mathf.Clamp01(y);
			
			return texture.GetPixelBilinear(x, y).b;
		}
		
		public float GetAlphaChanelProbe(float x, float y)
		{
			x = Mathf.Clamp01(x);
			y = Mathf.Clamp01(y);
			
			return texture.GetPixelBilinear(x, y).a;
		}
		
		public Color GetProbe(float x, float y)
		{
			x = Mathf.Clamp01(x);
			y = Mathf.Clamp01(y);
			
			return texture.GetPixelBilinear(x, y);
		}
		
		public void SaveTexture()
		{
			if (settings.exportFolder.IsAssigned)
			{
				File.WriteAllBytes(ConstructSavePath(), texture.EncodeToPNG());
				AssetDatabase.Refresh();
			}
			else
			{
				Debug.LogError("Cannot save texture! Export folder not set up.");
			}
		}
		
		public void UpdateTextureChannel(NoiseTextureChannel noiseTextureChannel)
		{
			if (texture.width != settings.resolution)
			{
				texture.Resize( settings.resolution, settings.resolution);
			}

			if (texture.filterMode != settings.filterMode)
			{
				texture.filterMode = settings.filterMode;
			}
			
			switch (noiseTextureChannel)
			{
				case NoiseTextureChannel.RED:
					Noise.GenerateNoiseMap(ref noiseBuffer,settings.redChannelNoiseSettings);
					break;
				case NoiseTextureChannel.GREEN:
					Noise.GenerateNoiseMap(ref noiseBuffer,settings.greenChannelNoiseSettings);
					break;
				case NoiseTextureChannel.BLUE:
					Noise.GenerateNoiseMap(ref noiseBuffer,settings.blueChannelNoiseSettings);
					break;
				case NoiseTextureChannel.ALPHA:
					Noise.GenerateNoiseMap(ref noiseBuffer,settings.alphaChannelNoiseSettings);
					break;
				case NoiseTextureChannel.FULL:
					Noise.GenerateNoiseMap(ref noiseBuffer,settings.redChannelNoiseSettings);
					Noise.GenerateNoiseMap(ref noiseBuffer,settings.greenChannelNoiseSettings);
					Noise.GenerateNoiseMap(ref noiseBuffer,settings.blueChannelNoiseSettings);
					Noise.GenerateNoiseMap(ref noiseBuffer,settings.alphaChannelNoiseSettings);
					break;
			}
			
			for (int y = 0; y < settings.resolution; y++)
			{
				for (int x = 0; x < settings.resolution; x++)
				{
					switch (noiseTextureChannel)
					{
						case NoiseTextureChannel.RED:
							textureValues[y * settings.resolution + x].r = noiseBuffer[x,y];
							break;
						case NoiseTextureChannel.GREEN:
							textureValues[y * settings.resolution + x].g = noiseBuffer[x,y];
							break;
						case NoiseTextureChannel.BLUE:
							textureValues[y * settings.resolution + x].b = noiseBuffer[x,y];
							break;
						case NoiseTextureChannel.ALPHA:
							textureValues[y * settings.resolution + x].a = noiseBuffer[x,y];
							break;
						case NoiseTextureChannel.FULL:
							textureValues[y * settings.resolution + x].r = noiseBuffer[x,y];
							textureValues[y * settings.resolution + x].g = noiseBuffer[x,y];
							textureValues[y * settings.resolution + x].b = noiseBuffer[x,y];
							textureValues[y * settings.resolution + x].a = noiseBuffer[x,y]; 
							break;
					}
				}
			}

			texture.GetRawTextureData<Color>();
			texture.SetPixels(textureValues);
			texture.Apply();
		}

		#endregion Public methods

		#region Private methods
		
		private string ConstructSavePath()
		{
			return settings.exportFolder.Path + $"/Red_{settings.alphaChannelNoiseSettings.noiseType}{settings.alphaChannelNoiseSettings.dimensions}D"
			                                  + $"Green_{settings.alphaChannelNoiseSettings.noiseType}{settings.alphaChannelNoiseSettings.dimensions}D"
			                                  + $"Blue_{settings.alphaChannelNoiseSettings.noiseType}{settings.alphaChannelNoiseSettings.dimensions}D"
			                                  + $"Alpha_{settings.alphaChannelNoiseSettings.noiseType}{settings.alphaChannelNoiseSettings.dimensions}D"
			                                  + $"_Noise_{settings.resolution}.png";
		}

		#endregion Private methods
	}
}