using Unity.Mathematics;
using UnityEngine;

namespace DudeiNoise
{
	public static class FalloffGenerator
	{
		public static float[,] GetFalloffMap(int size, float a, float b)
		{
			float[,] falloffMap = new float[size,size];
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					falloffMap[i, j] = GetFalloffProbe(i, j, a, b, size);
				}
			}

			return falloffMap;
		}
		
		public static float GetFalloffProbe(float x, float y, float a, float b, int size)
		{
			float normalizedX = math.clamp(x,0,size) / size * 2.0f - 1.0f;
			float normalizedY = math.clamp(y,0,size) / size * 2.0f - 1.0f;
			
			float falloffValue = math.max(Mathf.Abs(normalizedX), math.abs(normalizedY));
			float falloffTransition = FalloffCurve(falloffValue, a, b);
			
			return falloffTransition;
		}
		
		private static float FalloffCurve(float value, float a, float b)
		{
			return math.pow(value, a) / (math.pow(value, a) + math.pow(b - b * value, a));
		}
	}
}