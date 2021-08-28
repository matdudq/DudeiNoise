Shader "Unlit/UnlitWithChannelMask"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ChannelMask ("Channel Mask", color) = (1,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float4 _ChannelMask;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color = tex2D(_MainTex, i.uv) * _ChannelMask;
                float maxChannel = max(color.r, color.g);
                maxChannel = max(maxChannel, color.b);
                maxChannel = max(maxChannel, color.a);
                return fixed4(maxChannel,maxChannel,maxChannel,1.0);
            }
            ENDCG
        }
    }
}
