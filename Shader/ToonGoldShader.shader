// ボーナスキャラの女の子用
Shader "Unlit/ToonOutlineGoldShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _RampTex ("Ramp", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (0, 0, 0, 1)
        _OutlineWidth("Outline Width", Float) = 0.04
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        UsePass "Standard/ShadowCaster" // 影を落とす

        // 背面
        Pass
        {
            Tags{ "LightMode" = "UniversalForward"} // URPでマルチパス描画するのに必要
            Cull Front

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float _OutlineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex += float4(v.normal * _OutlineWidth, 0); // 法線方向に膨張
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return fixed4(0, 0, 0, 1); // 黒色で固定
            }
            ENDCG
        }

        // 前面
        Pass
        {
            Cull Back

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv_MainTex : TEXCOORD0;
                float2 uv_RampTex : TEXCOORD1;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv_MainTex : TEXCOORD0;
                float2 uv_RampTex : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _RampTex;
            float4 _RampTex_ST;
            fixed4 _MainColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.uv_MainTex = TRANSFORM_TEX(v.uv_MainTex, _MainTex);
                o.uv_RampTex = TRANSFORM_TEX(v.uv_RampTex, _RampTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // RampMapから取り出して乗算
                const half nl = dot(i.normal, _WorldSpaceLightPos0.xyz) * 0.5 + 0.5;
                const fixed3 ramp = tex2D(_RampTex, fixed2(nl, 0.5)).rgb + fixed3(0.25,0.1,0); // 金色感を出すため少しオレンジを足す
                // fixed4 col = tex2D(_MainTex, i.uv_MainTex);
                fixed4 col = _MainColor;
                col.rgb *= ramp;
                return col;
            }
            ENDCG
        }
    }
}