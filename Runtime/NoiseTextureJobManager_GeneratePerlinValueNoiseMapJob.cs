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
        private struct GeneratePerlinNoiseMapJob : IJobParallelFor
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

                float currentSample = PerlinNoise3D(ref currentPoint, data.tillingPeriod, data.tillingEnabled);
                float turbulenceSample = data.turbulenceEnabled ? math.abs(currentSample) : currentSample;
                float sum = turbulenceSample;
                
                float amplitude = 1f;
                float range = amplitude;

                for (int i = 1; i < data.octaves; i++)
                {
                    currentPoint *= data.lacunarity;
                    currentSample = PerlinNoise3D(ref currentPoint, data.tillingPeriod, data.tillingEnabled);
                    turbulenceSample = data.turbulenceEnabled ? math.abs(currentSample) : currentSample;
                        
                    amplitude *= data.persistence;
                    range += amplitude;
                    sum += turbulenceSample * amplitude;
                }

                float sample = (sum / range) * 0.5f + 0.5f;
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
            
            private float PerlinNoise3D(ref float3 point, int tillingPeriod, bool tillingEnabled)
            {
                int ix0 = (int) math.floor(point.x);
                int iy0 = (int) math.floor(point.y);
                int iz0 = (int) math.floor(point.z);

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

                float3 g000 = data.gradients3D[data.hash[h00 + iz0] & data.gradientsMask3D];
                float3 g100 = data.gradients3D[data.hash[h10 + iz0] & data.gradientsMask3D];
                float3 g010 = data.gradients3D[data.hash[h01 + iz0] & data.gradientsMask3D];
                float3 g110 = data.gradients3D[data.hash[h11 + iz0] & data.gradientsMask3D];
                float3 g001 = data.gradients3D[data.hash[h00 + iz1] & data.gradientsMask3D];
                float3 g101 = data.gradients3D[data.hash[h10 + iz1] & data.gradientsMask3D];
                float3 g011 = data.gradients3D[data.hash[h01 + iz1] & data.gradientsMask3D];
                float3 g111 = data.gradients3D[data.hash[h11 + iz1] & data.gradientsMask3D];

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

            
        #endregion Public methods
        }
    }
}