﻿using Unity.Collections;
using Unity.Mathematics;

namespace DudeiNoise
{
	public struct GenerateNoiseMapJobData
	{
		#region Variables

		[ReadOnly] 
		public NoiseTextureChannel channelToOverride;
		
		[ReadOnly] 
		public float3 positionOffset;

		[ReadOnly] 
		public float3 rotationOffset;

		[ReadOnly] 
		public float3 scaleOffset;

		[ReadOnly] 
		public int tillingPeriod;

		[ReadOnly]
		public bool tillingEnabled;

		[ReadOnly]
		public int octaves;

		[ReadOnly]
		public float lacunarity;

		[ReadOnly]
		public float persistence;

		[ReadOnly]
		public float woodPatternMultiplier;

		[ReadOnly] 
		public bool turbulenceEnabled;

		[ReadOnly] 
		public bool falloffEnabled;

		[ReadOnly] 
		public float falloffShift;

		[ReadOnly] 
		public float falloffDensity;

		[ReadOnly]
		public int resolution;
		
		[ReadOnly]
		public NativeArray<int> hash;

		[ReadOnly]
		public int hashMask; 

		[ReadOnly]
		public float sqr2;

		[ReadOnly]
		public NativeArray<float3> gradients3D;

		[ReadOnly]
		public int gradientsMask3D;

		#endregion
		
		#region Public methods
		 
		public void Initialize()
		{
			if (hash.IsCreated || gradients3D.IsCreated)
			{
				return;
			}
			
			int[] hashArray = new int[] {
			    151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
			    140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
			    247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
			    57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
			    74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
			    60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
			    65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
			    200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
			    52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
			    207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
			    119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
			    129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
			    218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
			    81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
			    184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
			    222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180,
			    151, 160, 137, 91, 90, 15, 131, 13, 201, 95, 96, 53, 194, 233, 7, 225,
			    140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23, 190, 6, 148,
			    247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32,
			    57, 177, 33, 88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175,
			    74, 165, 71, 134, 139, 48, 27, 166, 77, 146, 158, 231, 83, 111, 229, 122,
			    60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244, 102, 143, 54,
			    65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169,
			    200, 196, 135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64,
			    52, 217, 226, 250, 124, 123, 5, 202, 38, 147, 118, 126, 255, 82, 85, 212,
			    207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42, 223, 183, 170, 213,
			    119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
			    129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104,
			    218, 246, 97, 228, 251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241,
			    81, 51, 145, 235, 249, 14, 239, 107, 49, 192, 214, 31, 181, 199, 106, 157,
			    184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254, 138, 236, 205, 93,
			    222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
			};
			
			hash = new NativeArray<int>(hashArray, Allocator.Persistent);

			hashMask = 255;
			
			sqr2 = math.sqrt(2f);
			
			float3[] gradient3DArray = new []{
				new float3(1f, 1f, 0f),
				new float3(-1f, 1f, 0f),
				new float3(1f, -1f, 0f),
				new float3(-1f, -1f, 0f),
				new float3(1f, 0f, 1f),
				new float3(-1f, 0f, 1f),
				new float3(1f, 0f, -1f),
				new float3(-1f, 0f, -1f),
				new float3(0f, 1f, 1f),
				new float3(0f, -1f, 1f),
				new float3(0f, 1f, -1f),
				new float3(0f, -1f, -1f),

				new float3(1f, 1f, 0f),
				new float3(-1f, 1f, 0f),
				new float3(0f, -1f, 1f),
				new float3(0f, -1f, -1f)
			};
			
			gradients3D = new NativeArray<float3>(gradient3DArray, Allocator.Persistent);

			gradientsMask3D = 15;
		}

		public void Dispose()
		{
			gradients3D.Dispose();
			hash.Dispose();
		}
		
		#endregion Public methods
	}
}