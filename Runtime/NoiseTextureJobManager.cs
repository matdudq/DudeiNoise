using System;
using DudeiNoise.Utilities;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace DudeiNoise
{
	internal partial class NoiseTextureJobManager
	{
		#region Structs

		private class NoiseSettingsWithChannel
		{
			public NoiseSettings        noiseSettings  = null;
			public NoiseTextureChannel  textureChannel = default;
			public Action onCompleted    = null;
		}

		#endregion Structs
		
		#region Variables
		
		private NoiseSettingsWithChannel cachedNoiseSettingsWithChannel = null;

		private GenerateNoiseMapJobData generateNoiseMapJobData;
		
		private GenerateDefaultNoiseMapJob defaultNoiseMapJob;
		private GenerateValueNoiseMapJob valueNoiseMapJob; 
		private GeneratePerlinNoiseMapJob perlinNoiseMapJob;

		private NativeArray<Color> texturePixels;
		private int resolution;

		private bool isJobCompleted = true;

		#endregion Variables

		#region Constructors

		public NoiseTextureJobManager(NativeArray<Color> texturePixels, int resolution)
		{
			UpdateJobTextureData(texturePixels, resolution);
		}

		#endregion Constructors
		
		#region Public methods

		public void UpdateJobTextureData(NativeArray<Color> texturePixels, int resolution)
		{
			this.texturePixels = texturePixels;
			this.resolution = resolution;
			
			CreateJobs(this.texturePixels);
		}
		
		public void GenerateNoiseForChanelAsync(NoiseSettings noiseSettings, NoiseTextureChannel noiseChannel, MonoBehaviour context, Action onComplete = null)
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
			
	        isJobCompleted = false;
			
			ScheduleGenerateJobOfType(noiseSettings.noiseType, context, OnCompleteWrapped);

			void OnCompleteWrapped()
	        {
				onComplete?.Invoke();
		        isJobCompleted = true;
				
		        if (cachedNoiseSettingsWithChannel != null)
		        {
			        NoiseSettings cachedNoiseSettings = cachedNoiseSettingsWithChannel.noiseSettings;
			        NoiseTextureChannel cachedTextureChannel = cachedNoiseSettingsWithChannel.textureChannel;
			        Action cachedOnComplete = cachedNoiseSettingsWithChannel.onCompleted;
					
			        cachedNoiseSettingsWithChannel = null;

			        GenerateNoiseForChanelAsync(cachedNoiseSettings, cachedTextureChannel, context, cachedOnComplete);
		        }
	        }
        }
		
		#endregion Public methods

		#region Private methods

		private JobHandle ScheduleGenerateJobOfType(NoiseType noiseType, MonoBehaviour context, Action onComplete = null)
		{
			int pixelsCount = texturePixels.Length;
			int batchCount = pixelsCount / 6;
			
			switch (noiseType)
			{
				case NoiseType.Default:
					return defaultNoiseMapJob.ScheduleAsync(pixelsCount, batchCount, context, onComplete);
					break;
				case NoiseType.Value:
					return valueNoiseMapJob.ScheduleAsync(pixelsCount, batchCount, context, onComplete);
					break;
				case NoiseType.Perlin:
					return perlinNoiseMapJob.ScheduleAsync(pixelsCount, batchCount, context, onComplete);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(noiseType), noiseType, null);
			}
		}
		
		private void CreateJobs(NativeArray<Color> pixels)
		{
			defaultNoiseMapJob= new GenerateDefaultNoiseMapJob()
			{
				noiseTextureData = pixels
			};
			
			valueNoiseMapJob= new GenerateValueNoiseMapJob()
			{
				noiseTextureData = pixels
			}; 
			
			perlinNoiseMapJob= new GeneratePerlinNoiseMapJob()
			{
				noiseTextureData = pixels
			};
		}

		private void UpdateJobData(NoiseSettings noiseSettings, NoiseTextureChannel noiseTextureChannel)
		{
			generateNoiseMapJobData.Initialize();
			generateNoiseMapJobData.channelToOverride = noiseTextureChannel;
			generateNoiseMapJobData.positionOffset = noiseSettings.positionOffset;
			generateNoiseMapJobData.rotationOffset = noiseSettings.rotationOffset;
			generateNoiseMapJobData.scaleOffset = noiseSettings.scaleOffset;
			generateNoiseMapJobData.tillingPeriod = noiseSettings.tillingPeriod;
			generateNoiseMapJobData.tillingEnabled = noiseSettings.tillingEnabled;
			generateNoiseMapJobData.octaves = noiseSettings.octaves;
			generateNoiseMapJobData.lacunarity = noiseSettings.lacunarity;
			generateNoiseMapJobData.persistence = noiseSettings.persistence;
			generateNoiseMapJobData.woodPatternMultiplier = noiseSettings.woodPatternMultiplier;
			generateNoiseMapJobData.turbulenceEnabled = noiseSettings.turbulenceEnabled;
			generateNoiseMapJobData.falloffEnabled = noiseSettings.falloffEnabled;
			generateNoiseMapJobData.falloffShift = noiseSettings.falloffShift;
			generateNoiseMapJobData.falloffDensity = noiseSettings.falloffDensity;
			generateNoiseMapJobData.resolution = resolution;

			switch (noiseSettings.noiseType)
			{
				case NoiseType.Default:
					defaultNoiseMapJob.data = generateNoiseMapJobData;
					break;
				case NoiseType.Value:
					valueNoiseMapJob.data = generateNoiseMapJobData;
					break;
				case NoiseType.Perlin:
					perlinNoiseMapJob.data = generateNoiseMapJobData;
					break;
			}
		}
		
		#endregion Private methodsS
		
		#region Jobs Static Methods
		
		[BurstCompile]
		private static float WoodEffect(float sample, float woodPatternMultiplier)
		{
			if (woodPatternMultiplier > 1.0f)
			{
				sample *= woodPatternMultiplier;
				sample -= (int) sample;
			}

			return sample;
		}
		
		[BurstCompile]
		private static float Smooth(float t)
		{
			return t * t * t * (t * (t * 6f - 15f) + 10f);
		}

		[BurstCompile]
		private static float Dot(float2 g, float x, float y)
		{
			return g.x * x + g.y * y;
		}

		[BurstCompile]
		private static float Dot(float3 g, float x, float y, float z)
		{
			return g.x * x + g.y * y + g.z * z;
		}

		[BurstCompile]
		private static int PositiveModulo(int divident, int divisor)
		{
			int positiveDivident = divident % divisor + divisor;
			return positiveDivident % divisor;
		}

		[BurstCompile]
		private static float GetFalloffProbe(float x, float y, float a, float b, int size)
		{
			float normalizedX = math.clamp(x,0,size) / size * 2.0f - 1.0f;
			float normalizedY = math.clamp(y,0,size) / size * 2.0f - 1.0f;
			
			float falloffValue = math.max(math.abs(normalizedX), math.abs(normalizedY));
			float falloffTransition = FalloffCurve(falloffValue, a, b);
			
			return falloffTransition;
		}
            
		[BurstCompile]
		private static float FalloffCurve(float value, float a, float b)
		{
			return math.pow(value, a) / (math.pow(value, a) + math.pow(b - b * value, a));
		}
		
		#endregion Jobs Static Methods
		
		#region Editor methods

		#if UNITY_EDITOR
		public void GenerateNoiseForChanelAsync(NoiseSettings noiseSettings, NoiseTextureChannel noiseChannel, UnityEngine.Object context, Action onComplete = null)
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
			
			ScheduleGenerateJobOfType(noiseSettings.noiseType, context, OnCompleteWrapped);
			
			isJobCompleted = false;
			
			void OnCompleteWrapped()
			{
				onComplete?.Invoke();
				isJobCompleted = true;
				
				if (cachedNoiseSettingsWithChannel != null)
				{
					NoiseSettings cachedNoiseSettings = cachedNoiseSettingsWithChannel.noiseSettings;
					NoiseTextureChannel cachedTextureChannel = cachedNoiseSettingsWithChannel.textureChannel;
					Action cachedOnComplete = cachedNoiseSettingsWithChannel.onCompleted;
					
					cachedNoiseSettingsWithChannel = null;

					GenerateNoiseForChanelAsync(cachedNoiseSettings, cachedTextureChannel, context, cachedOnComplete);
				}
			}
		}

		private void ScheduleGenerateJobOfType(NoiseType noiseType, UnityEngine.Object context, Action onComplete = null)
		{
			int pixelsCount = texturePixels.Length;
			int batchCount = pixelsCount / 6;
			
			switch (noiseType)
			{
				case NoiseType.Default:
					defaultNoiseMapJob.ScheduleEditorAsync(pixelsCount, batchCount, context, onComplete);
					break;
				case NoiseType.Value:
					valueNoiseMapJob.ScheduleEditorAsync(pixelsCount, batchCount, context, onComplete);
					break;
				case NoiseType.Perlin:
					perlinNoiseMapJob.ScheduleEditorAsync(pixelsCount, batchCount, context, onComplete);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(noiseType), noiseType, null);
			}
		}
		
		#endif

		#endregion Editor methods
		
	}
}