using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace UNG
{
    internal partial class NoiseTextureJobManager
    {
        [BurstCompile]
        private struct GenerateWorleyNoiseMapJob : IJobParallelFor
        {
            #region Variables

            #region JobData

            [ReadOnly]
            public GenerateNoiseMapJobData data;
            
            public NativeArray<Color> noiseTextureData;

            #endregion JobData
            
            #endregion Variables

            #region Public methods
            
            public void Execute(int index)
            {
                int2 currentLocal = new int2(index % data.resolution, (int)math.floor(index / (float) data.resolution));
                Matrix4x4 noiseTRS = float4x4.TRS(data.positionOffset, quaternion.Euler(data.rotationOffset), data.scaleOffset);

                float3 point00 = noiseTRS.MultiplyPoint3x4(new float3(-0.5f, -0.5f, 0.0f));
                float3 point10 = noiseTRS.MultiplyPoint3x4(new float3(0.5f, -0.5f, 0.0f));
                float3 point01 = noiseTRS.MultiplyPoint3x4(new float3(-0.5f, 0.5f, 0.0f));
                float3 point11 = noiseTRS.MultiplyPoint3x4(new float3(0.5f, 0.5f, 0.0f));

                float stepSize = 1.0f / (data.resolution - 1);
                
                float3 point0 = math.lerp(point00, point01, currentLocal.y * stepSize);
                float3 point1 = math.lerp(point10, point11, currentLocal.y * stepSize);
                
                float3 currentPoint = math.lerp(point0, point1, currentLocal.x * stepSize);

                float currentSample = noise.cellular(currentPoint).x;
                float turbulenceSample = data.turbulenceEnabled ? math.abs(currentSample) : currentSample;
                float sum = turbulenceSample;
                
                float amplitude = 1f;
                float range = amplitude;

                for (int i = 1; i < data.octaves; i++)
                {
                    currentPoint *= data.lacunarity;
                    currentSample = noise.cellular(currentPoint).x;
                    turbulenceSample = data.turbulenceEnabled ? math.abs(currentSample) : currentSample;
                        
                    amplitude *= data.persistence;
                    range += amplitude;
                    sum += turbulenceSample * amplitude;
                }

                float sample = sum / range;
                float sampleWithCustomPattern = WoodEffect(sample, data.woodPatternMultiplier);
                float probe = sampleWithCustomPattern;

                if (data.falloffEnabled)
                {
                    probe -= GetFalloffProbe(currentLocal.x, currentLocal.y, data.falloffDensity, data.falloffShift, data.resolution);
                    probe = math.clamp(probe, 0, 1);
                }
                
                Color currentColor = noiseTextureData[index];
                currentColor[(int) data.channelToOverride] = probe;
                noiseTextureData[index] = currentColor;
            }

            #endregion Public methods
        }
    }
}