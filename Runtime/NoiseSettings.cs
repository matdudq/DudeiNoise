using System;
using UnityEngine;

namespace DudeiNoise
{
	[Serializable]
	public class NoiseSettings
	{
		#region Variables
		
		[Tooltip("Defines method of generating.")]
		public NoiseType noiseType = NoiseType.Default;
		
		[Tooltip("You can move though noise surface by that. Position offset in 'noise-space'.")]
		public Vector3 positionOffset = Vector3.zero;
		[Tooltip("You can rotate noise surface by that to get more interesting results. Rotation offset in 'noise-space'.")]
		public Vector3 rotationOffset = Vector3.zero;
		[Tooltip("Manages frequency of noise, you can scale noise space by that.")]
		public Vector3 scaleOffset = Vector3.one;

		[Tooltip("Means region on texture where noise tilling will not happend."), Range(1,256)]
		public int tillingPeriod = 10;

		[Tooltip("Enables or disables noise tilling.")]
		public bool tillingEnabled = false;
		
		[Range(1, 8), Tooltip("Octaves represents count of noise layers - iterations sub noises.")]
		public int octaves = 1;
		[Range(1f, 4f), Tooltip("Lacunarity is multiplier for texture frequency during each sum iteration.")]
		public float lacunarity = 2f;
		[Range(0f, 1f), Tooltip("Persistence is multiplier for amplitude value during each sum iteration.")]
		public float persistence = 0.5f;
		
		[Range(1f, 100f), Tooltip("When set up to 1 leaves sample as it is. When larger than 1 allows us to get wood like pattern.")]
		public float woodPatternMultiplier = 1.0f;
		
		[Tooltip("Switches turbulence mode.")]
		public bool turbulenceEnabled = false;
		
		[Tooltip("Switches falloff mode.")]
		public bool falloffEnabled = false;
		
		[Tooltip("Controls falloff map transition.")]
		public float falloffShift = 3.0f;
		
		[Tooltip("Controls falloff map density.")]
		public float falloffDensity = 2.2f;

		#endregion Variables

		#region Utilities

		public NoiseSettings Copy()
		{
			return new NoiseSettings()
			{
				noiseType = this.noiseType,
				positionOffset = this.positionOffset,
				rotationOffset = this.rotationOffset,
				scaleOffset = this.scaleOffset,
				tillingPeriod = this.tillingPeriod,
				tillingEnabled = this.tillingEnabled,
				octaves = this.octaves,
				lacunarity = this.lacunarity,
				persistence = this.persistence,
				woodPatternMultiplier = this.woodPatternMultiplier,
				turbulenceEnabled = this.turbulenceEnabled,
				falloffEnabled = this.falloffEnabled,
				falloffShift = this.falloffShift,
				falloffDensity = this.falloffDensity
			};
		}

		#endregion Utilities
	}
}