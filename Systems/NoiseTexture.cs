using System;
using System.Collections.Generic;
using System.Linq;
using DudeiNoise.Editor.Utilities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DudeiNoise
{
	public partial class NoiseTexture
	{
		private class NoiseSettingsWithChannel
		{
			public NoiseSettings noiseSettings = null;
			public NoiseTextureChannel textureChannel = default;
			public Action<NoiseTexture> onCompleted = null;
		}
		
		#region Variables
		
		private readonly Texture2D texture = null;

		private NoiseSettingsWithChannel cachedNoiseSettingsWithChannel = null; 
		
		private GenerateNoiseMapJob generateNoiseMapJob;
		
		private bool isJobCompleted = true;
		
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
		
		private NativeArray<Color> Pixels
		{
			get
			{
				return texture.GetRawTextureData<Color>();
			}
		}
		
		#endregion Properties

		#region Operator overloads

		public Color this[int key]
		{
			get
			{
				return Pixels[key];
			}
		}

		#endregion Operator overloads
		
		#region Constructor
		
		public NoiseTexture(int resolution)
		{
			//TODO: HOW TO HANDLE MORE THAT ONE TYPE
			texture = new Texture2D(resolution, resolution, TextureFormat.RGBAFloat, false)
			{
				filterMode = FilterMode.Bilinear,
				wrapMode = TextureWrapMode.Clamp
			};
			
			CreateJobs();
		}
		
		#endregion

		#region Public methods
		
		public void GenerateNoiseForChanelAsync(NoiseSettings noiseSettings, NoiseTextureChannel noiseChannel, MonoBehaviour context, Action<NoiseTexture> onComplete = null)
        {
	        if (!isJobCompleted)
	        {
		        cachedNoiseSettingsWithChannel = new NoiseSettingsWithChannel()
		        {
			        noiseSettings = noiseSettings,
			        textureChannel = noiseChannel,
			        onCompleted = onComplete
		        };
		        return;
	        }
			
	        UpdateJobData(noiseSettings, noiseChannel);

	        int pixelsCount = Pixels.Length;
	        int batchCount = pixelsCount / 6;
	        
	        isJobCompleted = false;
	        generateNoiseMapJob.ScheduleAsync(pixelsCount, batchCount, context, OnCompleteWrapped);
			
	        void OnCompleteWrapped(GenerateNoiseMapJob noiseMapJob)
	        {
		        texture.Apply();
		        onComplete?.Invoke(this);
		        isJobCompleted = true;
				
		        if (cachedNoiseSettingsWithChannel != null)
		        {
			        NoiseSettings cachedNoiseSettings = cachedNoiseSettingsWithChannel.noiseSettings;
			        NoiseTextureChannel cachedTextureChannel = cachedNoiseSettingsWithChannel.textureChannel;
			        Action<NoiseTexture> cachedOnComplete = cachedNoiseSettingsWithChannel.onCompleted;
					
			        cachedNoiseSettingsWithChannel = null;

			        GenerateNoiseForChanelAsync(cachedNoiseSettings, cachedTextureChannel, context, cachedOnComplete);
		        }
	        }
        }
		
		public void GenerateNoiseForChanelImmediately(NoiseSettings noiseSettings, NoiseTextureChannel noiseChannel)
		{
			UpdateJobData(noiseSettings, noiseChannel);
			
			int pixelsCount = Pixels.Length;
			int batchCount = pixelsCount / 6;
			
			JobHandle handle = generateNoiseMapJob.Schedule(pixelsCount, batchCount);
			handle.Complete();
			
			texture.Apply();
		}
		
#if UNITY_EDITOR
		public void GenerateNoiseForChanelAsync(NoiseSettings noiseSettings, NoiseTextureChannel noiseChannel, Object context, Action<NoiseTexture> onComplete = null)
		{
			if (!isJobCompleted)
			{
				cachedNoiseSettingsWithChannel = new NoiseSettingsWithChannel()
				{
					noiseSettings = noiseSettings,
					textureChannel = noiseChannel,
					onCompleted = onComplete
				};
				return;
			}
			
			UpdateJobData(noiseSettings, noiseChannel);

			int pixelsCount = Pixels.Length;
			int batchCount = pixelsCount / 6;
            
			generateNoiseMapJob.ScheduleEditorAsync(pixelsCount, batchCount, context, OnCompleteWrapped);
			isJobCompleted = false;
			
			void OnCompleteWrapped(GenerateNoiseMapJob noiseMapJob)
			{
				texture.Apply();
				onComplete?.Invoke(this);
				isJobCompleted = true;
				
				if (cachedNoiseSettingsWithChannel != null)
				{
					NoiseSettings cachedNoiseSettings = cachedNoiseSettingsWithChannel.noiseSettings;
					NoiseTextureChannel cachedTextureChannel = cachedNoiseSettingsWithChannel.textureChannel;
					Action<NoiseTexture> cachedOnComplete = cachedNoiseSettingsWithChannel.onCompleted;
					
					cachedNoiseSettingsWithChannel = null;

					GenerateNoiseForChanelAsync(cachedNoiseSettings, cachedTextureChannel, context, cachedOnComplete);
				}
			}
		}
#endif

		#endregion Public methods

		#region Private methods

		private void CreateJobs()
		{
			generateNoiseMapJob = new GenerateNoiseMapJob()
			{
				noiseTextureData = Pixels
			};
		}

		private void UpdateJobData(NoiseSettings noiseSettings, NoiseTextureChannel noiseTextureChannel)
		{
			generateNoiseMapJob.noiseType = noiseSettings.noiseType;
			generateNoiseMapJob.channelToOverride = noiseTextureChannel;
			generateNoiseMapJob.dimensions = noiseSettings.dimensions;
			generateNoiseMapJob.positionOffset = noiseSettings.positionOffset;
			generateNoiseMapJob.rotationOffset= noiseSettings.rotationOffset;
			generateNoiseMapJob.scaleOffset= noiseSettings.scaleOffset;
			generateNoiseMapJob.tillingPeriod = noiseSettings.tillingPeriod;
			generateNoiseMapJob.tillingEnabled = noiseSettings.tillingEnabled;
			generateNoiseMapJob.octaves = noiseSettings.octaves;
			generateNoiseMapJob.lacunarity = noiseSettings.lacunarity;
			generateNoiseMapJob.persistence = noiseSettings.persistence;
			generateNoiseMapJob.woodPatternMultiplier = noiseSettings.woodPatternMultiplier;
			generateNoiseMapJob.turbulenceEnabled = noiseSettings.turbulenceEnabled;
			generateNoiseMapJob.falloffEnabled = noiseSettings.falloffEnabled;
			generateNoiseMapJob.falloffShift = noiseSettings.falloffShift;
			generateNoiseMapJob.falloffDensity = noiseSettings.falloffDensity;
			generateNoiseMapJob.resolution = Resolution;
			generateNoiseMapJob.noiseTextureData = Pixels;
		}
		
		#endregion Private methodsS
	}
}