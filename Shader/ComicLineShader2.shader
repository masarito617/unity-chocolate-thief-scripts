// 集中線描画
// 元々ShaderGraphで作成したが、UIに適用するとGameSceneに描画されないUnityのバグがあったため転記して不要パスを削除した
// iPadで描画されない問題の対処として、ZTestもAlwaysに変更
Shader "ComicLineShader"
    {
        Properties
        {
            _Speed("Speed", Int) = 10
            _CircleRatio("CircleRatio", Float) = 0.1
            _Randomize("Randomize", Int) = 20
            _Size("Size", Float) = 1.25
            [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
            [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
        }
        SubShader
        {
            Tags
            {
                "RenderPipeline"="UniversalPipeline"
                "RenderType"="Transparent"
                "UniversalMaterialType" = "Unlit"
                "Queue"="Transparent"
                "ShaderGraphShader"="true"
                "ShaderGraphTargetId"=""
            }
            Pass
            {
                Name "Sprite Unlit"
                Tags
                {
                    "LightMode" = "Universal2D"
                }

                // Render State
                Cull Off
                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                ZTest Always // bugfix: AlwaysにしないとiPadで表示されなかった
                ZWrite Off

                // Debug
                // <None>

                // --------------------------------------------------
                // Pass

                HLSLPROGRAM

                // Pragmas
                #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag

                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>

                // Keywords
                #pragma multi_compile_fragment _ DEBUG_DISPLAY
                // GraphKeywords: <None>

                // Defines
                #define _SURFACE_TYPE_TRANSPARENT 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define ATTRIBUTES_NEED_COLOR
                #define VARYINGS_NEED_POSITION_WS
                #define VARYINGS_NEED_TEXCOORD0
                #define VARYINGS_NEED_COLOR
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_SPRITEUNLIT
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

                // Includes
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */

                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                // --------------------------------------------------
                // Structs and Packing

                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                     float4 uv0 : TEXCOORD0;
                     float4 color : COLOR;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 positionWS;
                     float4 texCoord0;
                     float4 color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                     float4 uv0;
                     float4 VertexColor;
                     float3 TimeParameters;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 interp0 : INTERP0;
                     float4 interp1 : INTERP1;
                     float4 interp2 : INTERP2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };

                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    output.interp1.xyzw =  input.texCoord0;
                    output.interp2.xyzw =  input.color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }

                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    output.texCoord0 = input.interp1.xyzw;
                    output.color = input.interp2.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }


                // --------------------------------------------------
                // Graph

                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float _Speed;
                float _CircleRatio;
                float _Randomize;
                float _Size;
                CBUFFER_END

                // Object and Global properties

                // Graph Includes
                // GraphIncludes: <None>

                // -- Property used by ScenePickingPass
                #ifdef SCENEPICKINGPASS
                float4 _SelectionID;
                #endif

                // -- Properties used by SceneSelectionPass
                #ifdef SCENESELECTIONPASS
                int _ObjectId;
                int _PassValue;
                #endif

                // Graph Functions

                void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
                {
                    float2 delta = UV - Center;
                    float radius = length(delta) * 2 * RadialScale;
                    float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
                    Out = float2(radius, angle);
                }

                void Unity_Preview_float(float In, out float Out)
                {
                    Out = In;
                }


                inline float Unity_SimpleNoise_RandomValue_float (float2 uv)
                {
                    float angle = dot(uv, float2(12.9898, 78.233));
                    #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                        // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                        angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                    #endif
                    return frac(sin(angle)*43758.5453);
                }

                inline float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
                {
                    return (1.0-t)*a + (t*b);
                }


                inline float Unity_SimpleNoise_ValueNoise_float (float2 uv)
                {
                    float2 i = floor(uv);
                    float2 f = frac(uv);
                    f = f * f * (3.0 - 2.0 * f);

                    uv = abs(frac(uv) - 0.5);
                    float2 c0 = i + float2(0.0, 0.0);
                    float2 c1 = i + float2(1.0, 0.0);
                    float2 c2 = i + float2(0.0, 1.0);
                    float2 c3 = i + float2(1.0, 1.0);
                    float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                    float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                    float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                    float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                    float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                    float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                    float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                    return t;
                }
                void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                {
                    float t = 0.0;

                    float freq = pow(2.0, float(0));
                    float amp = pow(0.5, float(3-0));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

                    freq = pow(2.0, float(1));
                    amp = pow(0.5, float(3-1));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

                    freq = pow(2.0, float(2));
                    amp = pow(0.5, float(3-2));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

                    Out = t;
                }

                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }

                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }

                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }

                void Unity_Lerp_float(float A, float B, float T, out float Out)
                {
                    Out = lerp(A, B, T);
                }

                void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                {
                    Out = smoothstep(Edge1, Edge2, In);
                }

                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };

                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }

                #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float Alpha;
                };

                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_c98c562980e340f59de8f8aae25f45f2_R_1 = IN.VertexColor[0];
                    float _Split_c98c562980e340f59de8f8aae25f45f2_G_2 = IN.VertexColor[1];
                    float _Split_c98c562980e340f59de8f8aae25f45f2_B_3 = IN.VertexColor[2];
                    float _Split_c98c562980e340f59de8f8aae25f45f2_A_4 = IN.VertexColor[3];
                    float _Property_a11c59c049bd45598cd76b5377e4901d_Out_0 = _Size;
                    float2 _PolarCoordinates_f92b319949834151a338b832260343b9_Out_4;
                    Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1, 0.8, _PolarCoordinates_f92b319949834151a338b832260343b9_Out_4);
                    float _Split_0e762d18143a42259e3cf0a85c3382ed_R_1 = _PolarCoordinates_f92b319949834151a338b832260343b9_Out_4[0];
                    float _Split_0e762d18143a42259e3cf0a85c3382ed_G_2 = _PolarCoordinates_f92b319949834151a338b832260343b9_Out_4[1];
                    float _Split_0e762d18143a42259e3cf0a85c3382ed_B_3 = 0;
                    float _Split_0e762d18143a42259e3cf0a85c3382ed_A_4 = 0;
                    float _Preview_2f253c917b4a4ce5957d5490570c7a68_Out_1;
                    Unity_Preview_float(_Split_0e762d18143a42259e3cf0a85c3382ed_R_1, _Preview_2f253c917b4a4ce5957d5490570c7a68_Out_1);
                    float _Preview_258e86ce07284dd08bb9706f01490e77_Out_1;
                    Unity_Preview_float(_Split_0e762d18143a42259e3cf0a85c3382ed_G_2, _Preview_258e86ce07284dd08bb9706f01490e77_Out_1);
                    float _SimpleNoise_77b627f039034c029aaf9468def86665_Out_2;
                    Unity_SimpleNoise_float((_Preview_258e86ce07284dd08bb9706f01490e77_Out_1.xx), 240, _SimpleNoise_77b627f039034c029aaf9468def86665_Out_2);
                    float _Property_a27ea4ef314341d7b0758018449aecfe_Out_0 = _Randomize;
                    float _Multiply_faa0e73b4dea444a9217375f3ed797d6_Out_2;
                    Unity_Multiply_float_float(_SimpleNoise_77b627f039034c029aaf9468def86665_Out_2, _Property_a27ea4ef314341d7b0758018449aecfe_Out_0, _Multiply_faa0e73b4dea444a9217375f3ed797d6_Out_2);
                    float _Property_df0042895ef14293b458957632d1c90a_Out_0 = _Speed;
                    float _Multiply_f27bb138a4004da3a2e275f2661e2e3e_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_df0042895ef14293b458957632d1c90a_Out_0, _Multiply_f27bb138a4004da3a2e275f2661e2e3e_Out_2);
                    float _Add_0c68c13af29243a9803cddfa3f3c4325_Out_2;
                    Unity_Add_float(_Multiply_faa0e73b4dea444a9217375f3ed797d6_Out_2, _Multiply_f27bb138a4004da3a2e275f2661e2e3e_Out_2, _Add_0c68c13af29243a9803cddfa3f3c4325_Out_2);
                    float _Sine_eca73931cfaa4e9cb7ba6c0cb2cc884a_Out_1;
                    Unity_Sine_float(_Add_0c68c13af29243a9803cddfa3f3c4325_Out_2, _Sine_eca73931cfaa4e9cb7ba6c0cb2cc884a_Out_1);
                    float _Property_b9554d33698f4b3ea6a42dc4850b398a_Out_0 = _CircleRatio;
                    float _Lerp_73ef5f64b3ee4f3abd1f3c1e51694989_Out_3;
                    Unity_Lerp_float(_Preview_2f253c917b4a4ce5957d5490570c7a68_Out_1, _Sine_eca73931cfaa4e9cb7ba6c0cb2cc884a_Out_1, _Property_b9554d33698f4b3ea6a42dc4850b398a_Out_0, _Lerp_73ef5f64b3ee4f3abd1f3c1e51694989_Out_3);
                    float _Smoothstep_1c42b20d82e14347b45f0f334f404805_Out_3;
                    Unity_Smoothstep_float(0.8, _Property_a11c59c049bd45598cd76b5377e4901d_Out_0, _Lerp_73ef5f64b3ee4f3abd1f3c1e51694989_Out_3, _Smoothstep_1c42b20d82e14347b45f0f334f404805_Out_3);
                    float _Multiply_edf7e2ddc03e4cf1bde968268253a15a_Out_2;
                    Unity_Multiply_float_float(_Split_c98c562980e340f59de8f8aae25f45f2_A_4, _Smoothstep_1c42b20d82e14347b45f0f334f404805_Out_3, _Multiply_edf7e2ddc03e4cf1bde968268253a15a_Out_2);
                    surface.BaseColor = (IN.VertexColor.xyz);
                    surface.Alpha = _Multiply_edf7e2ddc03e4cf1bde968268253a15a_Out_2;
                    return surface;
                }

                // --------------------------------------------------
                // Build Graph Inputs

                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;

                    return output;
                }

                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);







                    output.uv0 =                                        input.texCoord0;
                    output.VertexColor =                                input.color;
                    output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                    return output;
                }


                // --------------------------------------------------
                // Main

                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

                ENDHLSL
            }
            Pass
            {
                Name "Sprite Unlit"
                Tags
                {
                    "LightMode" = "UniversalForward"
                }

                // Render State
                Cull Off
                Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
                ZTest Always // bugfix: AlwaysにしないとiPadで表示されなかった
                ZWrite Off

                // Debug
                // <None>

                // --------------------------------------------------
                // Pass

                HLSLPROGRAM

                // Pragmas
                #pragma target 2.0
                #pragma exclude_renderers d3d11_9x
                #pragma vertex vert
                #pragma fragment frag

                // DotsInstancingOptions: <None>
                // HybridV1InjectedBuiltinProperties: <None>

                // Keywords
                #pragma multi_compile_fragment _ DEBUG_DISPLAY
                // GraphKeywords: <None>

                // Defines
                #define _SURFACE_TYPE_TRANSPARENT 1
                #define ATTRIBUTES_NEED_NORMAL
                #define ATTRIBUTES_NEED_TANGENT
                #define ATTRIBUTES_NEED_TEXCOORD0
                #define ATTRIBUTES_NEED_COLOR
                #define VARYINGS_NEED_POSITION_WS
                #define VARYINGS_NEED_TEXCOORD0
                #define VARYINGS_NEED_COLOR
                #define FEATURES_GRAPH_VERTEX
                /* WARNING: $splice Could not find named fragment 'PassInstancing' */
                #define SHADERPASS SHADERPASS_SPRITEFORWARD
                /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */

                // Includes
                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreInclude' */

                #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

                // --------------------------------------------------
                // Structs and Packing

                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

                struct Attributes
                {
                     float3 positionOS : POSITION;
                     float3 normalOS : NORMAL;
                     float4 tangentOS : TANGENT;
                     float4 uv0 : TEXCOORD0;
                     float4 color : COLOR;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : INSTANCEID_SEMANTIC;
                    #endif
                };
                struct Varyings
                {
                     float4 positionCS : SV_POSITION;
                     float3 positionWS;
                     float4 texCoord0;
                     float4 color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };
                struct SurfaceDescriptionInputs
                {
                     float4 uv0;
                     float4 VertexColor;
                     float3 TimeParameters;
                };
                struct VertexDescriptionInputs
                {
                     float3 ObjectSpaceNormal;
                     float3 ObjectSpaceTangent;
                     float3 ObjectSpacePosition;
                };
                struct PackedVaryings
                {
                     float4 positionCS : SV_POSITION;
                     float3 interp0 : INTERP0;
                     float4 interp1 : INTERP1;
                     float4 interp2 : INTERP2;
                    #if UNITY_ANY_INSTANCING_ENABLED
                     uint instanceID : CUSTOM_INSTANCE_ID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                     uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                     uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                     FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                    #endif
                };

                PackedVaryings PackVaryings (Varyings input)
                {
                    PackedVaryings output;
                    ZERO_INITIALIZE(PackedVaryings, output);
                    output.positionCS = input.positionCS;
                    output.interp0.xyz =  input.positionWS;
                    output.interp1.xyzw =  input.texCoord0;
                    output.interp2.xyzw =  input.color;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }

                Varyings UnpackVaryings (PackedVaryings input)
                {
                    Varyings output;
                    output.positionCS = input.positionCS;
                    output.positionWS = input.interp0.xyz;
                    output.texCoord0 = input.interp1.xyzw;
                    output.color = input.interp2.xyzw;
                    #if UNITY_ANY_INSTANCING_ENABLED
                    output.instanceID = input.instanceID;
                    #endif
                    #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                    output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                    #endif
                    #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                    output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                    #endif
                    #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                    output.cullFace = input.cullFace;
                    #endif
                    return output;
                }


                // --------------------------------------------------
                // Graph

                // Graph Properties
                CBUFFER_START(UnityPerMaterial)
                float _Speed;
                float _CircleRatio;
                float _Randomize;
                float _Size;
                CBUFFER_END

                // Object and Global properties

                // Graph Includes
                // GraphIncludes: <None>

                // -- Property used by ScenePickingPass
                #ifdef SCENEPICKINGPASS
                float4 _SelectionID;
                #endif

                // -- Properties used by SceneSelectionPass
                #ifdef SCENESELECTIONPASS
                int _ObjectId;
                int _PassValue;
                #endif

                // Graph Functions

                void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
                {
                    float2 delta = UV - Center;
                    float radius = length(delta) * 2 * RadialScale;
                    float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
                    Out = float2(radius, angle);
                }

                void Unity_Preview_float(float In, out float Out)
                {
                    Out = In;
                }


                inline float Unity_SimpleNoise_RandomValue_float (float2 uv)
                {
                    float angle = dot(uv, float2(12.9898, 78.233));
                    #if defined(SHADER_API_MOBILE) && (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3) || defined(SHADER_API_VULKAN))
                        // 'sin()' has bad precision on Mali GPUs for inputs > 10000
                        angle = fmod(angle, TWO_PI); // Avoid large inputs to sin()
                    #endif
                    return frac(sin(angle)*43758.5453);
                }

                inline float Unity_SimpleNnoise_Interpolate_float (float a, float b, float t)
                {
                    return (1.0-t)*a + (t*b);
                }


                inline float Unity_SimpleNoise_ValueNoise_float (float2 uv)
                {
                    float2 i = floor(uv);
                    float2 f = frac(uv);
                    f = f * f * (3.0 - 2.0 * f);

                    uv = abs(frac(uv) - 0.5);
                    float2 c0 = i + float2(0.0, 0.0);
                    float2 c1 = i + float2(1.0, 0.0);
                    float2 c2 = i + float2(0.0, 1.0);
                    float2 c3 = i + float2(1.0, 1.0);
                    float r0 = Unity_SimpleNoise_RandomValue_float(c0);
                    float r1 = Unity_SimpleNoise_RandomValue_float(c1);
                    float r2 = Unity_SimpleNoise_RandomValue_float(c2);
                    float r3 = Unity_SimpleNoise_RandomValue_float(c3);

                    float bottomOfGrid = Unity_SimpleNnoise_Interpolate_float(r0, r1, f.x);
                    float topOfGrid = Unity_SimpleNnoise_Interpolate_float(r2, r3, f.x);
                    float t = Unity_SimpleNnoise_Interpolate_float(bottomOfGrid, topOfGrid, f.y);
                    return t;
                }
                void Unity_SimpleNoise_float(float2 UV, float Scale, out float Out)
                {
                    float t = 0.0;

                    float freq = pow(2.0, float(0));
                    float amp = pow(0.5, float(3-0));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

                    freq = pow(2.0, float(1));
                    amp = pow(0.5, float(3-1));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

                    freq = pow(2.0, float(2));
                    amp = pow(0.5, float(3-2));
                    t += Unity_SimpleNoise_ValueNoise_float(float2(UV.x*Scale/freq, UV.y*Scale/freq))*amp;

                    Out = t;
                }

                void Unity_Multiply_float_float(float A, float B, out float Out)
                {
                    Out = A * B;
                }

                void Unity_Add_float(float A, float B, out float Out)
                {
                    Out = A + B;
                }

                void Unity_Sine_float(float In, out float Out)
                {
                    Out = sin(In);
                }

                void Unity_Lerp_float(float A, float B, float T, out float Out)
                {
                    Out = lerp(A, B, T);
                }

                void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
                {
                    Out = smoothstep(Edge1, Edge2, In);
                }

                /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

                // Graph Vertex
                struct VertexDescription
                {
                    float3 Position;
                    float3 Normal;
                    float3 Tangent;
                };

                VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
                {
                    VertexDescription description = (VertexDescription)0;
                    description.Position = IN.ObjectSpacePosition;
                    description.Normal = IN.ObjectSpaceNormal;
                    description.Tangent = IN.ObjectSpaceTangent;
                    return description;
                }

                #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

                // Graph Pixel
                struct SurfaceDescription
                {
                    float3 BaseColor;
                    float Alpha;
                };

                SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
                {
                    SurfaceDescription surface = (SurfaceDescription)0;
                    float _Split_c98c562980e340f59de8f8aae25f45f2_R_1 = IN.VertexColor[0];
                    float _Split_c98c562980e340f59de8f8aae25f45f2_G_2 = IN.VertexColor[1];
                    float _Split_c98c562980e340f59de8f8aae25f45f2_B_3 = IN.VertexColor[2];
                    float _Split_c98c562980e340f59de8f8aae25f45f2_A_4 = IN.VertexColor[3];
                    float _Property_a11c59c049bd45598cd76b5377e4901d_Out_0 = _Size;
                    float2 _PolarCoordinates_f92b319949834151a338b832260343b9_Out_4;
                    Unity_PolarCoordinates_float(IN.uv0.xy, float2 (0.5, 0.5), 1, 0.8, _PolarCoordinates_f92b319949834151a338b832260343b9_Out_4);
                    float _Split_0e762d18143a42259e3cf0a85c3382ed_R_1 = _PolarCoordinates_f92b319949834151a338b832260343b9_Out_4[0];
                    float _Split_0e762d18143a42259e3cf0a85c3382ed_G_2 = _PolarCoordinates_f92b319949834151a338b832260343b9_Out_4[1];
                    float _Split_0e762d18143a42259e3cf0a85c3382ed_B_3 = 0;
                    float _Split_0e762d18143a42259e3cf0a85c3382ed_A_4 = 0;
                    float _Preview_2f253c917b4a4ce5957d5490570c7a68_Out_1;
                    Unity_Preview_float(_Split_0e762d18143a42259e3cf0a85c3382ed_R_1, _Preview_2f253c917b4a4ce5957d5490570c7a68_Out_1);
                    float _Preview_258e86ce07284dd08bb9706f01490e77_Out_1;
                    Unity_Preview_float(_Split_0e762d18143a42259e3cf0a85c3382ed_G_2, _Preview_258e86ce07284dd08bb9706f01490e77_Out_1);
                    float _SimpleNoise_77b627f039034c029aaf9468def86665_Out_2;
                    Unity_SimpleNoise_float((_Preview_258e86ce07284dd08bb9706f01490e77_Out_1.xx), 240, _SimpleNoise_77b627f039034c029aaf9468def86665_Out_2);
                    float _Property_a27ea4ef314341d7b0758018449aecfe_Out_0 = _Randomize;
                    float _Multiply_faa0e73b4dea444a9217375f3ed797d6_Out_2;
                    Unity_Multiply_float_float(_SimpleNoise_77b627f039034c029aaf9468def86665_Out_2, _Property_a27ea4ef314341d7b0758018449aecfe_Out_0, _Multiply_faa0e73b4dea444a9217375f3ed797d6_Out_2);
                    float _Property_df0042895ef14293b458957632d1c90a_Out_0 = _Speed;
                    float _Multiply_f27bb138a4004da3a2e275f2661e2e3e_Out_2;
                    Unity_Multiply_float_float(IN.TimeParameters.x, _Property_df0042895ef14293b458957632d1c90a_Out_0, _Multiply_f27bb138a4004da3a2e275f2661e2e3e_Out_2);
                    float _Add_0c68c13af29243a9803cddfa3f3c4325_Out_2;
                    Unity_Add_float(_Multiply_faa0e73b4dea444a9217375f3ed797d6_Out_2, _Multiply_f27bb138a4004da3a2e275f2661e2e3e_Out_2, _Add_0c68c13af29243a9803cddfa3f3c4325_Out_2);
                    float _Sine_eca73931cfaa4e9cb7ba6c0cb2cc884a_Out_1;
                    Unity_Sine_float(_Add_0c68c13af29243a9803cddfa3f3c4325_Out_2, _Sine_eca73931cfaa4e9cb7ba6c0cb2cc884a_Out_1);
                    float _Property_b9554d33698f4b3ea6a42dc4850b398a_Out_0 = _CircleRatio;
                    float _Lerp_73ef5f64b3ee4f3abd1f3c1e51694989_Out_3;
                    Unity_Lerp_float(_Preview_2f253c917b4a4ce5957d5490570c7a68_Out_1, _Sine_eca73931cfaa4e9cb7ba6c0cb2cc884a_Out_1, _Property_b9554d33698f4b3ea6a42dc4850b398a_Out_0, _Lerp_73ef5f64b3ee4f3abd1f3c1e51694989_Out_3);
                    float _Smoothstep_1c42b20d82e14347b45f0f334f404805_Out_3;
                    Unity_Smoothstep_float(0.8, _Property_a11c59c049bd45598cd76b5377e4901d_Out_0, _Lerp_73ef5f64b3ee4f3abd1f3c1e51694989_Out_3, _Smoothstep_1c42b20d82e14347b45f0f334f404805_Out_3);
                    float _Multiply_edf7e2ddc03e4cf1bde968268253a15a_Out_2;
                    Unity_Multiply_float_float(_Split_c98c562980e340f59de8f8aae25f45f2_A_4, _Smoothstep_1c42b20d82e14347b45f0f334f404805_Out_3, _Multiply_edf7e2ddc03e4cf1bde968268253a15a_Out_2);
                    surface.BaseColor = (IN.VertexColor.xyz);
                    surface.Alpha = _Multiply_edf7e2ddc03e4cf1bde968268253a15a_Out_2;
                    return surface;
                }

                // --------------------------------------------------
                // Build Graph Inputs

                VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
                {
                    VertexDescriptionInputs output;
                    ZERO_INITIALIZE(VertexDescriptionInputs, output);

                    output.ObjectSpaceNormal =                          input.normalOS;
                    output.ObjectSpaceTangent =                         input.tangentOS.xyz;
                    output.ObjectSpacePosition =                        input.positionOS;

                    return output;
                }

                SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
                {
                    SurfaceDescriptionInputs output;
                    ZERO_INITIALIZE(SurfaceDescriptionInputs, output);







                    output.uv0 =                                        input.texCoord0;
                    output.VertexColor =                                input.color;
                    output.TimeParameters =                             _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN                output.FaceSign =                                   IS_FRONT_VFACE(input.cullFace, true, false);
                #else
                #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
                #endif
                #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                    return output;
                }


                // --------------------------------------------------
                // Main

                #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

                ENDHLSL
            }
        }
        CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
        FallBack "Hidden/Shader Graph/FallbackError"
    }