Shader "Hidden/StylizedFogEdges"
{
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "StylizedFogEdges"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // Camera depth, bound explicitly by the renderer feature (not the global sampler).
            TEXTURE2D_X_FLOAT(_DepthTexture);

            float  _PosterizeSteps;
            float4 _FogNearColor;
            float4 _FogFarColor;
            float  _FogNear;
            float  _FogFar;
            float  _FogContribution;
            float  _ColorEdgeThreshold;
            float  _DepthEdgeThreshold;
            float  _EdgeIntensity;
            float4 _EdgeColor;
            float  _EdgeWidth;
            float4 _TexelSize;   // (1/w, 1/h, w, h) of the source, set by the renderer feature
            float  _Saturation;
            float  _Contrast;
            float  _Gamma;

            float Luma(float3 c)
            {
                return dot(c, float3(0.2126, 0.7152, 0.0722));
            }

            float3 SampleScene(float2 uv)
            {
                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).rgb;
            }

            // Linear 0..1 eye depth at a screen UV.
            float Depth01(float2 uv)
            {
                float raw = SAMPLE_TEXTURE2D_X(_DepthTexture, sampler_PointClamp, uv).r;
                return Linear01Depth(raw, _ZBufferParams);
            }

            // Placeholder for luminance(VolumetricLighting): a near->far remap of linear depth.
            float VolumetricTerm(float2 uv)
            {
                return saturate((Depth01(uv) - _FogNear) / max(_FogFar - _FogNear, 1e-5));
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                float2 o  = _TexelSize.xy * _EdgeWidth;

                float3 sceneColor = SampleScene(uv);

                // ---- posterized fog ------------------------------------------------
                float steps      = max(_PosterizeSteps, 1.0);
                float posterized = floor(VolumetricTerm(uv) * steps) / steps;   // Multiply -> Floor -> Divide
                float3 fogColor  = lerp(_FogNearColor.rgb, _FogFarColor.rgb, posterized);

                // ---- colour edge mask --------------------------------------------
                float lc = Luma(sceneColor);
                float colorDiff = max(
                    max(abs(lc - Luma(SampleScene(uv + float2(o.x, 0.0)))),
                        abs(lc - Luma(SampleScene(uv - float2(o.x, 0.0))))),
                    max(abs(lc - Luma(SampleScene(uv + float2(0.0, o.y)))),
                        abs(lc - Luma(SampleScene(uv - float2(0.0, o.y))))));
                float colorEdgeMask = step(_ColorEdgeThreshold, colorDiff);

                // ---- depth edge mask (relative) --------------------------------
                float dc = Depth01(uv);
                float depthDiff = max(
                    max(abs(dc - Depth01(uv + float2(o.x, 0.0))),
                        abs(dc - Depth01(uv - float2(o.x, 0.0)))),
                    max(abs(dc - Depth01(uv + float2(0.0, o.y))),
                        abs(dc - Depth01(uv - float2(0.0, o.y))))) / (dc + 1e-4);
                float depthEdgeMask = step(_DepthEdgeThreshold, depthDiff);

                float edge = saturate(max(colorEdgeMask, depthEdgeMask) * _EdgeIntensity);

                // ---- composite --------------------------------------------------
                float3 sceneWithEdges    = lerp(sceneColor, _EdgeColor.rgb, edge);
                float3 sceneFogComposite = lerp(sceneWithEdges, fogColor, _FogContribution);

                // ---- optional colour adjust / gamma --------------------------
                float3 c = sceneFogComposite;
                c = lerp(Luma(c).xxx, c, _Saturation);
                c = (c - 0.5) * _Contrast + 0.5;
                c = pow(max(c, 0.0), 1.0 / max(_Gamma, 1e-5));

                return half4(max(c, 0.0), 1.0);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
