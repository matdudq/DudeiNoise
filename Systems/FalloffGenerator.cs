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
			float normalizedX = Mathf.Clamp(x,0,size) / size * 2.0f - 1.0f;
			float normalizedY = Mathf.Clamp(y,0,size) / size * 2.0f - 1.0f;
			
			float falloffValue = Mathf.Max(Mathf.Abs(normalizedX), Mathf.Abs(normalizedY));
			float falloffTransition = FalloffCurve(falloffValue, a, b);
			
			return falloffTransition;
		}
		
		private static float FalloffCurve(float value, float a, float b)
		{
			return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
		}
	}
}