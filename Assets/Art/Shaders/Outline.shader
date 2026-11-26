Shader "Waystation/Outline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1, 0.85, 0.3, 1)
        _OutlineWidth ("Outline Width", Float) = 3
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
            "RenderType" = "Opaque"
        }

        Pass
        {
            Name "Outline"
            // Drawn through an override material, so the pass index matters more than the tag.
            // UniversalForward keeps it in the same shader-tag list the feature builds.
            Tags { "LightMode" = "UniversalForward" }

            Cull Front
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth; // pixels
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings Vert(Attributes input)
            {
                Varyings output;
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

                // Project the normal into clip space and push along its screen XY.
                // Clip x/y spans 2*w across the screen, so one pixel is 2*w / resolution.
                float2 clipNormal = mul((float3x3)GetWorldToHClipMatrix(), normalWS).xy;
                float lenSq = dot(clipNormal, clipNormal);
                float invLen = lenSq > 1e-8 ? rsqrt(lenSq) : 0;
                output.positionCS.xy += clipNormal * invLen * (_OutlineWidth * output.positionCS.w * 2.0 / _ScreenParams.xy);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
