Shader "URP/VolumetricLightCone"
{
    // Soft "god-ray" cone for URP, Lethal Company style. Rendered in the Transparent
    // queue (additive) so it lands in the camera colour texture BEFORE
    // StylizedFogEdgesFeature's post pass - the fog / colour-grade then composites over
    // the beam for free. All shaping is done here: view-rim softness, length + tip fade,
    // scene-depth soft-particle fade, camera-proximity fade, animated fbm attenuation and
    // a Henyey-Greenstein forward-scatter term along the cone axis.
    Properties
    {
        [HDR] _Color            ("Colour", Color)                 = (1, 0.92, 0.72, 1)
        _Intensity              ("Intensity", Range(0, 20))       = 2.5
        _EdgeSoftness           ("Edge Softness", Range(0.25, 8)) = 3
        _LengthFade             ("Length Fade", Range(0.01, 6))   = 1.6
        _TipFade                ("Tip Fade", Range(0.001, 1))     = 0.12
        _DepthFade              ("Depth Fade Distance", Range(0.01, 12)) = 1.5
        _CameraFade             ("Camera Fade Distance", Range(0.01, 12)) = 1
        _Scattering             ("Forward Scattering g", Range(-0.95, 0.95)) = 0.6
        _ScatterBoost           ("Scatter Boost", Range(0, 6))    = 1.6
        _NoiseScale             ("Noise Scale", Float)            = 1.5
        _NoiseStrength          ("Noise Strength", Range(0, 1))   = 0.45
        _NoiseSpeed             ("Noise Speed (xyz)", Vector)     = (0.05, -0.13, 0.03, 0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
        }
        LOD 100

        Pass
        {
            Name "VolumetricLightConeForward"
            Tags { "LightMode" = "UniversalForward" }

            Blend One One
            ZWrite Off
            ZTest LEqual
            Cull Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float  _Intensity;
                float  _EdgeSoftness;
                float  _LengthFade;
                float  _TipFade;
                float  _DepthFade;
                float  _CameraFade;
                float  _Scattering;
                float  _ScatterBoost;
                float  _NoiseScale;
                float  _NoiseStrength;
                float4 _NoiseSpeed;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 positionOS : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                float2 uv         : TEXCOORD3;
                float  viewZ      : TEXCOORD4;
            };

            float hash31(float3 p)
            {
                p = frac(p * 0.3183099 + 0.1);
                p *= 17.0;
                return frac(p.x * p.y * p.z * (p.x + p.y + p.z));
            }

            float vnoise(float3 x)
            {
                float3 i = floor(x);
                float3 f = frac(x);
                f = f * f * (3.0 - 2.0 * f);

                float n000 = hash31(i + float3(0, 0, 0));
                float n100 = hash31(i + float3(1, 0, 0));
                float n010 = hash31(i + float3(0, 1, 0));
                float n110 = hash31(i + float3(1, 1, 0));
                float n001 = hash31(i + float3(0, 0, 1));
                float n101 = hash31(i + float3(1, 0, 1));
                float n011 = hash31(i + float3(0, 1, 1));
                float n111 = hash31(i + float3(1, 1, 1));

                return lerp(lerp(lerp(n000, n100, f.x), lerp(n010, n110, f.x), f.y),
                            lerp(lerp(n001, n101, f.x), lerp(n011, n111, f.x), f.y), f.z);
            }

            float fbm(float3 p)
            {
                float amp = 0.5;
                float sum = 0.0;
                [unroll]
                for (int i = 0; i < 3; i++)
                {
                    sum += amp * vnoise(p);
                    p *= 2.02;
                    amp *= 0.5;
                }
                return sum;
            }

            // Henyey-Greenstein phase - forward-biased scatter when looking down the beam.
            float HenyeyGreenstein(float cosTheta, float g)
            {
                float g2 = g * g;
                float denom = max(1e-3, 1.0 + g2 - 2.0 * g * cosTheta);
                return (1.0 - g2) / (4.0 * PI * pow(denom, 1.5));
            }

            Varyings Vert(Attributes IN)
            {
                Varyings OUT = (Varyings)0;
                VertexPositionInputs pos = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionCS = pos.positionCS;
                OUT.positionWS = pos.positionWS;
                OUT.positionOS = IN.positionOS.xyz;
                OUT.normalWS   = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv         = IN.uv;
                OUT.viewZ      = -pos.positionVS.z;
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                float3 toCam   = _WorldSpaceCameraPos - IN.positionWS;
                float  camDist = length(toCam);
                float3 V       = toCam / max(camDist, 1e-5);
                float3 N       = normalize(IN.normalWS);

                // Fake volume thickness: grazing angles read as "more cone" -> soft edges.
                float rim  = 1.0 - saturate(abs(dot(N, V)));
                float edge = pow(rim, _EdgeSoftness);

                // Along-axis fades. uv.y is 0 at the apex, 1 at the mouth.
                float lengthT  = saturate(IN.uv.y);
                float lenFade  = saturate(pow(1.0 - lengthT, _LengthFade));
                float tipFade  = smoothstep(0.0, _TipFade, lengthT);

                // Forward scattering along the cone's local +Z, expressed in world space.
                float3 axisWS = normalize(TransformObjectToWorldDir(float3(0, 0, 1)));
                float  cosT   = dot(-V, axisWS);
                float  phase  = HenyeyGreenstein(cosT, _Scattering) * _ScatterBoost + 0.25;

                // Animated attenuation - dust / flicker moving through the shaft.
                float3 np    = IN.positionOS * _NoiseScale + _NoiseSpeed.xyz * _Time.y;
                float  atten = lerp(1.0, fbm(np), _NoiseStrength);

                // Soft-particle fade against the scene, plus don't pop when the camera is inside.
                float2 screenUV     = IN.positionCS.xy / _ScreenParams.xy;
                float  sceneEyeZ    = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams);
                float  depthFade    = saturate((sceneEyeZ - IN.viewZ) / _DepthFade);
                float  cameraFade   = saturate(camDist / _CameraFade);

                float a = edge * lenFade * tipFade * phase * atten * depthFade * cameraFade;
                a = saturate(a);

                float3 rgb = _Color.rgb * _Intensity * a;
                return half4(rgb, a);
            }
            ENDHLSL
        }
    }

    Fallback Off
}
