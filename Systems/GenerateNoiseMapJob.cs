using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Utilities;

namespace DudeiNoise
{
    public static partial class Noise
    {
        public struct GenerateNoiseMapJob : IDisposableJobParallelFor
        { 
            #region Variables
		
            public NoiseType noiseType;
            public int dimensions;
            
            
            public int tillingPeriod;
            public bool tillingEnabled;
            
            public int octaves;
            public float lacunarity;
            public float persistence;
            
            public float woodPatternMultiplier;
            
            public bool turbulenceEnabled;
            
            public bool falloffEnabled;
            public float falloffShift;
            public float falloffDensity;
            
            public int resolution;

            #endregion Variables
            
            
            public NativeArray<float3> points;
            public NativeArray<float2> localPoints;
            public NativeArray<float> noiseMapResult;
            
            public void Execute(int index)
            {
                float3 currentPoint = points[index];
                float2 currentLocal = localPoints[index];
                float probe = GetProbe(
                    currentPoint, 
                    dimensions,
                    tillingEnabled, 
                    tillingPeriod,
                    octaves, 
                    lacunarity, 
                    persistence, 
                    turbulenceEnabled, 
                    noiseType,
                    woodPatternMultiplier
                    );
                
                if (falloffEnabled)
                {
                    probe -= FalloffGenerator.GetFalloffProbe(currentLocal.x, currentLocal.y,falloffDensity,falloffShift, resolution);
                    probe = math.clamp(probe,0,1);
                }

                noiseMapResult[index] = probe;
            }

            public void Dispose()
            {
                points.Dispose();
                localPoints.Dispose();
                noiseMapResult.Dispose();
            }
        }
    }
}