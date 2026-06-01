Shader "Hidden/TiltShift"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off ZTest Always Blend Off Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float  _Center;
            float  _AreaSize;
            float  _BlurAmount;
            float4 _TexelSize;

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = input.texcoord;

                float dist       = abs(uv.y - _Center);
                float sharpHalf  = _AreaSize * 0.5;
                float outerRange = max(0.001, 0.5 - sharpHalf);
                float t          = saturate((dist - sharpHalf) / outerRange);
                float blur       = smoothstep(0.0, 1.0, t) * _BlurAmount;

                if (blur < 0.001)
                    return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                half4  col         = 0;
                float  totalWeight = 0;
                float2 step        = _TexelSize.xy * blur;

                [unroll] for (int x = -2; x <= 2; x++)
                {
                    [unroll] for (int y = -2; y <= 2; y++)
                    {
                        float  w   = exp(-0.5 * float(x * x + y * y));
                        float2 off = float2(x, y) * step;
                        col         += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv + off) * w;
                        totalWeight += w;
                    }
                }
                return col / totalWeight;
            }
            ENDHLSL
        }
    }
}
