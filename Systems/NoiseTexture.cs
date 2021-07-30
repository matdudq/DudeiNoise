using System.IO;
using Unity.Collections;
using UnityEditor;
using UnityEngine;

namespace DudeiNoise
{
	public class NoiseTexture
	{
		#region Variables

		private NoiseTextureSettings settings = null;
		
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
			int currentResolution = settings.resolution;
			float[,] noiseBuffer = new float[currentResolution,currentResolution];
			
			if (texture.width != currentResolution)
			{
				texture.Resize( currentResolution, currentResolution);
			}

			if (texture.filterMode != settings.filterMode)
			{
				texture.filterMode = settings.filterMode;
			}

			NoiseSettings noiseSettings = settings.GetNoiseSettingsForChannel(noiseTextureChannel);
			Noise.GenerateNoiseMap(ref noiseBuffer, noiseSettings);
			
			NativeArray<Color32> textureValues = texture.GetRawTextureData<Color32>();
			
			for (int y = 0; y < currentResolution; y++)
			{
				for (int x = 0; x < currentResolution; x++)
				{
					Color color = textureValues[y * currentResolution + x];
					
					switch (noiseTextureChannel)
					{
						case NoiseTextureChannel.RED:
							color.r = noiseBuffer[x,y];
							break;
						case NoiseTextureChannel.GREEN:
							color.g = noiseBuffer[x,y];
							break;
						case NoiseTextureChannel.BLUE:
							color.b = noiseBuffer[x,y];
							break;
						case NoiseTextureChannel.ALPHA:
							color.a = noiseBuffer[x,y];
							break;
					}

					textureValues[y * currentResolution + x] = color;
				}
			}

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