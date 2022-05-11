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
        private struct GenerateValueNoiseMapJob : IJobParallelFor
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

                float currentSample = ValueNoise3D(ref currentPoint, data.tillingPeriod, data.tillingEnabled);
                float turbulenceSample = data.turbulenceEnabled ? math.abs(currentSample) : currentSample;
                float sum = turbulenceSample;
                
                float amplitude = 1f;
                float range = amplitude;

                for (int i = 1; i < data.octaves; i++)
                {
                    currentPoint *= data.lacunarity;
                    currentSample = ValueNoise3D(ref currentPoint, data.tillingPeriod, data.tillingEnabled);
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
            
            #region Private methods
            
           private float ValueNoise3D(ref float3 point, int tillingPeriod, bool tillingEnabled)
            {
                int ix0 = (int) math.floor(point.x);
                int iy0 = (int) math.floor(point.y);
                int iz0 = (int) math.floor(point.z);

                float tx = point.x - ix0;
                float ty = point.y - iy0;
                float tz = point.z - iz0;

                if (tillingEnabled)
                {
                    ix0 = PositiveModulo(ix0, tillingPeriod);
                    iy0 = PositiveModulo(iy0, tillingPeriod);
                    iz0 = PositiveModulo(iz0, tillingPeriod);
                }

                ix0 &= data.hashMask;
                iy0 &= data.hashMask;
                iz0 &= data.hashMask;

                int ix1 = ix0 + 1;
                int iy1 = iy0 + 1;
                int iz1 = iz0 + 1;

                if (tillingEnabled)
                {
                    ix1 = PositiveModulo(ix1, tillingPeriod);
                    iy1 = PositiveModulo(iy1, tillingPeriod);
                    iz1 = PositiveModulo(iz1, tillingPeriod);
                }

                int h0 = data.hash[ix0];
                int h1 = data.hash[ix1];
                int h00 = data.hash[h0 + iy0];
                int h10 = data.hash[h1 + iy0];
                int h01 = data.hash[h0 + iy1];
                int h11 = data.hash[h1 + iy1];
                int h000 = data.hash[h00 + iz0];
                int h100 = data.hash[h10 + iz0];
                int h010 = data.hash[h01 + iz0];
                int h110 = data.hash[h11 + iz0];
                int h001 = data.hash[h00 + iz1];
                int h101 = data.hash[h10 + iz1];
                int h011 = data.hash[h01 + iz1];
                int h111 = data.hash[h11 + iz1];

                tx = Smooth(tx);
                ty = Smooth(ty);
                tz = Smooth(tz);

                return math.lerp(math.lerp(math.lerp(h000, h100, tx), math.lerp(h010, h110, tx), ty),
                    math.lerp(math.lerp(h001, h101, tx), math.lerp(h011, h111, tx), ty),
                    tz) * (1f / data.hashMask);
            }

            
        #endregion Public methods
        }
    }
}