using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace DudeiNoise
{
	public delegate float NoiseMethod(ref float3 point, int tillingPeriod, bool tillingEnabled);
	
	public static class Noise
	{
		#region Variables
		
		private static readonly NoiseMethod[] basicMethods = {
			Noise1D,
			Noise2D,
			Noise3D
		};
		
		private static readonly NoiseMethod[] valueMethods = {
			ValueNoise1D,
			ValueNoise2D,
			ValueNoise3D
		};
		
		private static readonly NoiseMethod[] perlinMethods = {
			PerlinNoise1D,
			PerlinNoise2D,
			PerlinNoise3D
		};

		public static NoiseMethod[][] methods = {
			basicMethods,
			valueMethods,
			perlinMethods
		};
		
		private static readonly int[] hash = {
			151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
			140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
			247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
			57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
			74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
			60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
			65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
			200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
			52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
			207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
			119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
			129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
			218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
			81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
			184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
			222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,
			151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
			140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
			247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
			57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
			74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
			60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
			65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
			200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
			52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
			207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
			119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
			129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
			218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
			81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
			184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
			222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
		};

		private const int hashMask = 255;

		private static float sqr2 = math.sqrt(2f);
		
		private static float[] gradients1D = {
			1f, -1f
		};

		private const int gradientsMask1D = 1;
		
		private static float2[] gradients2D = {
			new float2( 1f, 0f),
			new float2(-1f, 0f),
			new float2( 0f, 1f),
			new float2( 0f,-1f),
			math.normalize(new float2( 1f, 1f)),
			math.normalize(new float2(-1f, 1f)), 
			math.normalize(new float2( 1f,-1f)), 
			math.normalize(new float2(-1f,-1f))
		};
	
		private const int gradientsMask2D = 7;
		
		private static float3[] gradients3D = {
			new float3( 1f, 1f, 0f),
			new float3(-1f, 1f, 0f),
			new float3( 1f,-1f, 0f),
			new float3(-1f,-1f, 0f),
			new float3( 1f, 0f, 1f),
			new float3(-1f, 0f, 1f),
			new float3( 1f, 0f,-1f),
			new float3(-1f, 0f,-1f),
			new float3( 0f, 1f, 1f),
			new float3( 0f,-1f, 1f),
			new float3( 0f, 1f,-1f),
			new float3( 0f,-1f,-1f),
		
			new float3( 1f, 1f, 0f),
			new float3(-1f, 1f, 0f),
			new float3( 0f,-1f, 1f),
			new float3( 0f,-1f,-1f)
		};
	
		private const int gradientsMask3D = 15;
		
		#endregion Variablese

		#region Public methods

		public static void GenerateNoiseMap(ref float[,] noiseMap, NoiseSettings noiseSettings)
		{ 
			int resolution = math.min(noiseMap.GetLength(0), noiseMap.GetLength(1));
			
			Matrix4x4 noiseTRS = float4x4.TRS(noiseSettings.positionOffset, quaternion.Euler(noiseSettings.rotationOffset), noiseSettings.scaleOffset);
			
			float3 point00 = noiseTRS.MultiplyPoint3x4(new float3(-0.5f,-0.5f,0.0f));
			float3 point10 = noiseTRS.MultiplyPoint3x4(new float3(0.5f,-0.5f,0.0f));
			float3 point01 = noiseTRS.MultiplyPoint3x4(new float3(-0.5f,0.5f,0.0f));
			float3 point11 = noiseTRS.MultiplyPoint3x4(new float3(0.5f,0.5f,0.0f));

			float stepSize = 1.0f / (resolution-1);

			for (int y = 0; y < resolution; y++)
			{
				float3 point0 = math.lerp(point00,point01, y * stepSize);
				float3 point1 = math.lerp(point10,point11, y * stepSize);
				
				for (int x = 0; x < resolution; x++)
				{
					float3 currentPoint = math.lerp(point0, point1, x * stepSize);
					noiseMap[x, y] = GetProbe(currentPoint,noiseSettings);
				}
			}
		}
		
		#endregion Public methods
		
		#region Private methods
		
		private static float GetProbe(float3 point, NoiseSettings generatorSettings)
		{
			return GetProbe(point, generatorSettings.dimensions,
							generatorSettings.tillingEnabled, generatorSettings.tillingPeriod,
							generatorSettings.octaves, generatorSettings.lacunarity, 
							generatorSettings.persistence, generatorSettings.turbulenceEnabled, 
							generatorSettings.noiseType,generatorSettings.woodPatternMultiplier);
		}

		private static float GetProbe (float3 point, int dimensions, bool tillingEnabled, int tillingPeriod, int octaves, float lacunarity, float persistence, bool turbulence, NoiseType noiseType, float woodPatternMultiplier)
		{
			NoiseMethod method = GetNoiseMethod(noiseType, dimensions);
			
			float sum = turbulence ? math.abs(method(ref point,tillingPeriod,tillingEnabled)) : method(ref point,tillingPeriod,tillingEnabled);
			float amplitude = 1f;
			float range = amplitude;

			for (int i = 1; i < octaves; i++)
			{
				point *= lacunarity;
				float currentSample = turbulence ? math.abs(method(ref point,tillingPeriod,tillingEnabled)) : method(ref point,tillingPeriod,tillingEnabled);
				
				amplitude *= persistence;
				range += amplitude;
				sum += currentSample * amplitude;
			}

			float sample = sum / range;
			float normalizedSample = NormalizeSample(sample, noiseType);
			float sampleWithCustomPattern = ApplyWoodEffect(normalizedSample, woodPatternMultiplier);

			return sampleWithCustomPattern;
		}
		
		private static float NormalizeSample(float sample, NoiseType noiseType)
		{
			if (noiseType == NoiseType.Perlin) 
			{
				return sample * 0.5f + 0.5f;
			}

			return sample;
		}

		private static float ApplyWoodEffect(float sample, float woodPatternMultiplier)
		{
			if (woodPatternMultiplier > 1.0f)
			{
				sample *= woodPatternMultiplier;
				sample -= (int) sample;
			}

			return sample;
		}
		
		#region Basic noise

		private static float Noise1D (ref float3 point, int tillingPeriod , bool tillingEnabled )
		{
			int i0 = (int)math.floor(point.x);
			
			if (tillingEnabled)
			{
				i0 = PositiveModulo(i0, tillingPeriod);
			}
			
			return hash[i0 & hashMask] * (1.0f / hashMask);
		}
		
		private static float Noise2D (ref float3 point, int tillingPeriod, bool tillingEnabled) 
		{
			int ix = (int)math.floor(point.x);
			int iy = (int)math.floor(point.y);
			
			if (tillingEnabled)
			{
				ix = PositiveModulo(ix, tillingPeriod);
				iy = PositiveModulo(iy, tillingPeriod);
			}
			
			return hash[hash[ix & hashMask] + iy & hashMask] * (1f / hashMask);
		}

		private static float Noise3D (ref float3 point, int tillingPeriod, bool tillingEnabled)
		{
			int ix = (int)math.floor(point.x);
			int iy = (int)math.floor(point.y);
			int iz = (int)math.floor(point.z);
			
			if (tillingEnabled)
			{
				ix = PositiveModulo(ix, tillingPeriod);
				iy = PositiveModulo(iy, tillingPeriod);
				iz = PositiveModulo(iz, tillingPeriod);
			}
			
			return hash[hash[hash[ix & hashMask] + iy & hashMask] + iz & hashMask] * (1f / hashMask);
		}

		#endregion Basic noise

		#region Value noise

		private static float ValueNoise1D (ref float3 point, int tillingPeriod, bool tillingEnabled)
		{
			int i0 = (int)math.floor(point.x);
			
			float t = point.x - i0;
			
			if (tillingEnabled)
			{
				i0 = PositiveModulo(i0, tillingPeriod);
			}

			i0 &= hashMask;
			
			int i1 = i0 + 1;

			if (tillingEnabled)
			{
				i1 = PositiveModulo(i1, tillingPeriod);
			}

			int h0 = hash[i0];
			int h1 = hash[i1];

			return math.lerp(h0, h1, Smooth(t)) * (1f / hashMask);
		}

		private static float ValueNoise2D(ref float3 point, int tillingPeriod, bool tillingEnabled)
		{
			int ix0 = (int)math.floor(point.x);
			int iy0 = (int)math.floor(point.y);

			float xt = point.x - ix0;
			float yt = point.y - iy0;
			
			if (tillingEnabled)
			{
				ix0 = PositiveModulo(ix0, tillingPeriod);
				iy0 = PositiveModulo(iy0, tillingPeriod);
			}
			
			ix0 &= hashMask;
			iy0 &= hashMask;

			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;

			if (tillingEnabled)
			{
				ix1 = PositiveModulo(ix1, tillingPeriod);
				iy1 = PositiveModulo(iy1, tillingPeriod);
			}

			
			int h0 = hash[ix0];
			int h1 = hash[ix1];

			int h00 = hash[h0 + iy0];
			int h10 = hash[h1 + iy0];
			int h01 = hash[h0 + iy1];
			int h11 = hash[h1 + iy1];

			xt = Smooth(xt);
			yt = Smooth(yt);
			
			return math.lerp(math.lerp(h00, h10, xt),
							  math.lerp(h01, h11, xt),
							  yt) * (1f / hashMask);
		}

		private static float ValueNoise3D (ref float3 point, int tillingPeriod, bool tillingEnabled) 
		{
			int ix0 = (int)math.floor(point.x);
			int iy0 = (int)math.floor(point.y);
			int iz0 = (int)math.floor(point.z);
			
			float tx = point.x - ix0;
			float ty = point.y - iy0;
			float tz = point.z - iz0;
			
			if (tillingEnabled)
			{
				ix0 = PositiveModulo(ix0, tillingPeriod);
				iy0 = PositiveModulo(iy0, tillingPeriod);
				iz0 = PositiveModulo(iz0, tillingPeriod);
			}
			
			ix0 &= hashMask;
			iy0 &= hashMask;
			iz0 &= hashMask;
			
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;
			int iz1 = iz0 + 1;

			if (tillingEnabled)
			{
				ix1 = PositiveModulo(ix1, tillingPeriod);
				iy1 = PositiveModulo(iy1, tillingPeriod);
				iz1 = PositiveModulo(iz1, tillingPeriod);
			}
			
			int h0 = hash[ix0];
			int h1 = hash[ix1];
			int h00 = hash[h0 + iy0];
			int h10 = hash[h1 + iy0];
			int h01 = hash[h0 + iy1];
			int h11 = hash[h1 + iy1];
			int h000 = hash[h00 + iz0];
			int h100 = hash[h10 + iz0];
			int h010 = hash[h01 + iz0];
			int h110 = hash[h11 + iz0];
			int h001 = hash[h00 + iz1];
			int h101 = hash[h10 + iz1];
			int h011 = hash[h01 + iz1];
			int h111 = hash[h11 + iz1];

			tx = Smooth(tx);
			ty = Smooth(ty);
			tz = Smooth(tz);
			
			return math.lerp(math.lerp(math.lerp(h000, h100, tx), math.lerp(h010, h110, tx), ty),
							  math.lerp(math.lerp(h001, h101, tx), math.lerp(h011, h111, tx), ty),
							  tz) * (1f / hashMask);
		}

		#endregion Value noise

		#region Perlin noise

		private static float PerlinNoise1D(ref float3 point, int tillingPeriod, bool tillingEnabled)
		{
			int i0 = (int)math.floor(point.x);
			float t0 = point.x - i0;
			float t1 = t0 - 1.0f;

			if (tillingEnabled)
			{
				i0 = PositiveModulo(i0, tillingPeriod);
			}
			
			i0 &= hashMask;
			
			int i1 = i0 + 1;

			if (tillingEnabled)
			{
				i1 = PositiveModulo(i1, tillingPeriod);
			}

			float g0 = gradients1D[hash[i0] & gradientsMask1D];
			float g1 = gradients1D[hash[i1] & gradientsMask1D];

			float v0 = g0 * t0;
			float v1 = g1 * t1;

			float t = Smooth(t0);
			
			return math.lerp(v0,v1, t) * 2.0f;
		}
		private static float PerlinNoise2D(ref float3 point, int tillingPeriod, bool tillingEnabled)
		{
			int ix0 = (int)math.floor(point.x);
			int iy0 = (int)math.floor(point.y);
			
			float tx0 = point.x - ix0;
			float ty0 = point.y - iy0;
			float tx1 = tx0 - 1f;
			float ty1 = ty0 - 1f;
			
			if (tillingEnabled)
			{
				ix0 = PositiveModulo(ix0, tillingPeriod);
				iy0 = PositiveModulo(iy0, tillingPeriod);
			}

			ix0 &= hashMask;
			iy0 &= hashMask;
			
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;

			if (tillingEnabled)
			{
				ix1 = PositiveModulo(ix1, tillingPeriod);
				iy1 = PositiveModulo(iy1, tillingPeriod);
			}

			int h0 = hash[ix0];
			int h1 = hash[ix1];
			
			float2 g00 = gradients2D[hash[h0 + iy0] & gradientsMask2D];
			float2 g10 = gradients2D[hash[h1 + iy0] & gradientsMask2D];
			float2 g01 = gradients2D[hash[h0 + iy1] & gradientsMask2D];
			float2 g11 = gradients2D[hash[h1 + iy1] & gradientsMask2D];

			float v00 = Dot(g00, tx0, ty0);
			float v10 = Dot(g10, tx1, ty0);
			float v01 = Dot(g01, tx0, ty1);
			float v11 = Dot(g11, tx1, ty1);
		
			float tx = Smooth(tx0);
			float ty = Smooth(ty0);
			
			return math.lerp(math.lerp(v00, v10, tx),
							 math.lerp(v01, v11, tx),
							 ty) * sqr2;
		}

		private static float PerlinNoise3D(ref float3 point, int tillingPeriod, bool tillingEnabled)
		{
			int ix0 = (int)math.floor(point.x);
			int iy0 = (int)math.floor(point.y);
			int iz0 = (int)math.floor(point.z);
			
			float tx0 = point.x - ix0;
			float ty0 = point.y - iy0;
			float tz0 = point.z - iz0;
			float tx1 = tx0 - 1f;
			float ty1 = ty0 - 1f;
			float tz1 = tz0 - 1f;

			if (tillingEnabled)
			{
				ix0 = PositiveModulo(ix0, tillingPeriod);
				iy0 = PositiveModulo(iy0, tillingPeriod);
				iz0 = PositiveModulo(iz0, tillingPeriod);
			}

			ix0 &= hashMask;
			iy0 &= hashMask;
			iz0 &= hashMask;
			
			int ix1 = ix0 + 1;
			int iy1 = iy0 + 1;
			int iz1 = iz0 + 1;
		
			if (tillingEnabled)
			{
				ix1 = PositiveModulo(ix1, tillingPeriod);
				iy1 = PositiveModulo(iy1, tillingPeriod);
				iz1 = PositiveModulo(iz1, tillingPeriod);
			}
			
			int h0 = hash[ix0];
			int h1 = hash[ix1];
			int h00 = hash[h0 + iy0];
			int h10 = hash[h1 + iy0];
			int h01 = hash[h0 + iy1];
			int h11 = hash[h1 + iy1];
			
			float3 g000 = gradients3D[hash[h00 + iz0] & gradientsMask3D];
			float3 g100 = gradients3D[hash[h10 + iz0] & gradientsMask3D];
			float3 g010 = gradients3D[hash[h01 + iz0] & gradientsMask3D];
			float3 g110 = gradients3D[hash[h11 + iz0] & gradientsMask3D];
			float3 g001 = gradients3D[hash[h00 + iz1] & gradientsMask3D];
			float3 g101 = gradients3D[hash[h10 + iz1] & gradientsMask3D];
			float3 g011 = gradients3D[hash[h01 + iz1] & gradientsMask3D];
			float3 g111 = gradients3D[hash[h11 + iz1] & gradientsMask3D];

			float v000 = Dot(g000, tx0, ty0, tz0);
			float v100 = Dot(g100, tx1, ty0, tz0);
			float v010 = Dot(g010, tx0, ty1, tz0);
			float v110 = Dot(g110, tx1, ty1, tz0);
			float v001 = Dot(g001, tx0, ty0, tz1);
			float v101 = Dot(g101, tx1, ty0, tz1);
			float v011 = Dot(g011, tx0, ty1, tz1);
			float v111 = Dot(g111, tx1, ty1, tz1);

			float tx = Smooth(tx0);
			float ty = Smooth(ty0);
			float tz = Smooth(tz0);
			return math.lerp(math.lerp(math.lerp(v000, v100, tx), math.lerp(v010, v110, tx), ty),
							  math.lerp(math.lerp(v001, v101, tx), math.lerp(v011, v111, tx), ty),
							  tz);
		}
		
		[BurstCompile]
		public static NoiseMethod GetNoiseMethod(NoiseType noiseType, int dimensions)
		{
			return methods[(int) noiseType][dimensions - 1];
		}
		
		#endregion Perlin noise

		#region Calculation functions
		
		[BurstCompile]
		private static float Smooth (float t) 
		{
			return t * t * t * (t * (t * 6f - 15f) + 10f);
		}
		
		[BurstCompile]
		private static float Dot (float2 g, float x, float y) {
			return g.x * x + g.y * y;
		}
		
		[BurstCompile]
		private static float Dot (float3 g, float x, float y, float z) {
			return g.x * x + g.y * y + g.z * z;
		}
		
		[BurstCompile]
		private static int PositiveModulo(int divident, int divisor){
			int positiveDivident = divident % divisor + divisor;
			return positiveDivident % divisor;
		}
		
		#endregion Calculation functions
		
		#endregion Public methods
	}
}