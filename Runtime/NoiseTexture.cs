using System;
using Unity.Mathematics;
using UnityEngine;

namespace DudeiNoise
{
	public partial class NoiseTexture
	{
		#region Variables
		
		private readonly Texture2D texture = null;

		private readonly NoiseTextureJobManager noiseTextureJobManager = null;
		
		#endregion Variables

		#region Properties

		public Texture2D Texture
		{
			get
			{
				return texture;
			}
		}

		public int Resolution
		{
			get
			{
				return math.min(texture.width, texture.height);
			}
		}
		
		#endregion Properties
		
		#region Constructor
		
		public NoiseTexture(int resolution)
		{
			//TODO: HOW TO HANDLE MORE THAT ONE TYPE
			texture = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false)
			{
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Clamp
			};
			
			noiseTextureJobManager = new NoiseTextureJobManager(texture.GetRawTextureData<Color>(), resolution);
		}
		
		#endregion

		#region Public methods

		public void GenerateNoiseForChanel(NoiseSettings noiseSettings, NoiseTextureChannel noiseChannel, MonoBehaviour context, Action<NoiseTexture> onComplete = null)
		{
			noiseTextureJobManager.GenerateNoiseForChanelAsync(noiseSettings, noiseChannel, context, OnCompleteWrapped);

			void OnCompleteWrapped()
			{
				texture.Apply();
				onComplete?.Invoke(this);
			}
		}

		public void Resize(int newResolution)
		{
			if (newResolution == Resolution)
			{
				return;
			}
			
			texture.Resize(newResolution,newResolution);

			noiseTextureJobManager.UpdateJobTextureData(texture.GetRawTextureData<Color>(), Resolution);
		}

		public void SetFilterMode(FilterMode filterMode)
		{
			if (texture.filterMode == filterMode)
			{
				return;
			}

			texture.filterMode = filterMode;
		}
		
		#endregion Public methods

		#region Editor methods

		public void GenerateNoiseForChanelInEditor(NoiseSettings noiseSettings, NoiseTextureChannel  noiseChannel, UnityEngine.Object context, Action<NoiseTexture> onComplete = null)
		{
			noiseTextureJobManager.GenerateNoiseForChanelAsync(noiseSettings, noiseChannel, context, OnCompleteWrapped);

			void OnCompleteWrapped()
			{
				texture.Apply();
				onComplete?.Invoke(this);
			}

		}

		#endregion Editor methods
	}
}