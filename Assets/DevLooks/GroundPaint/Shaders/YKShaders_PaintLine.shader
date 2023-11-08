Shader "YKSEshaders/PaintLine"
{
    Properties
    {
        [HideInInspector]_SecondHitPos("", Vector) = (0, 0, 0, 0)
        [HideInInspector]_FirstHitPos("", Vector) = (0, 0, 0, 0)
        [HideInInspector]_PaintColor("", Color) = (0, 0, 0, 0)
        [HideInInspector]_PaintSize("Brush Size", Vector) = (1, 1, 0, 0)
        [HideInInspector]_BrushTexture("Brush Texture", 2D) = "black" {}
    }
    SubShader
    {
        Cull Off
		Zwrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            Texture2D _PaintRT;
            SamplerState sampler_PaintRT;
            Texture2D _BrushTexture;
            SamplerState sampler_BrushTexture;
            float2 _SecondHitPos;
            float2 _FirstHitPos;
            float2 _PaintSize;
            half4 _PaintColor;

            float2 CountBrushUV(float2 start, float2 end, float2 p, float2 size)
            {
                float2 brushuv = 0;
                float2 center = (end - start) * 0.5 + start;
                float2 dirs2c = center - start;
                float halflen = length(dirs2c);
                dirs2c = normalize(dirs2c);
                float2 linec2p = p - center;
                float xdis = dot(dirs2c, linec2p);
                brushuv.x = saturate(xdis/(halflen * 2.2 * size.x) + 0.5);


                float2 o = center + dirs2c * xdis;
                float2 dirs2p = normalize(p - start);
                float ydis = length(p - o) * -sign(cross(float3(dirs2p, 0), float3(dirs2c, 0)).z);
                brushuv.y = saturate(ydis/(size.y * 0.02) + 0.5);
                return brushuv;
            }

            Varyings Vertex (Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 Fragment (Varyings input) : SV_Target
            {
                float2 uv = input.uv;
                half4 color = SAMPLE_TEXTURE2D(_PaintRT, sampler_PaintRT, uv);
                float2 brushUV = CountBrushUV(_FirstHitPos, _SecondHitPos, uv, _PaintSize);
                float SoftBrush = SAMPLE_TEXTURE2D(_BrushTexture, sampler_BrushTexture, brushUV).r;
                float f1 = pow(SoftBrush, 4);
                float f2 = -pow(SoftBrush, 4) + 1;
                half4 paintColor = color * f2 +  _PaintColor * f1;
                color = paintColor;
                return color;
            }
            ENDHLSL
        }
    }
}