Shader "YKSEshaders/Paint"
{
    Properties
    {
        [HideInInspector]_HitCenter("", Vector) = (0, 0, 0, 0)
        [HideInInspector]_PaintColor("", Color) = (0, 0, 0, 0)
        _PaintRadius("Ridus", Range(0, 1)) = 0.2
        _BrushTexture("Brush Texture", 2D) = "black" {}
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
            float2 _HitCenter;
            float _PaintRadius;
            half4 _PaintColor;
            
            float3 hash(uint3 x )
            {
                x = ((x>>8)^x.yzx)*1103515245U;
                x = ((x>>8)^x.yzx)*1103515245U;
                x = ((x>>8)^x.yzx)*1103515245U;
                return float3(x)*(1.0/float(0xffffffff));
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
                //安卓端这里有bug不知道为什么a通道初始就是1
                half4 color = SAMPLE_TEXTURE2D(_PaintRT, sampler_PaintRT, uv);
                _PaintRadius *= 0.08;
                uint2 randomIndex = uint2(hash(uint3(_HitCenter * 20170909, _Time.z)).xy * 3);
                float2 brushUV = (saturate((_HitCenter - uv)*(1/_PaintRadius) * 0.5 + 0.5) + randomIndex) * 0.25;
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
