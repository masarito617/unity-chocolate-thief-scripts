// タイトル画面の背景シェーダー
Shader "Hidden/TitleScrollShader"
{
Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color1("Color 1",Color) = (0,0,0,1)
        _Color2("Color 2",Color) = (1,1,1,1)
    }

    SubShader
    {
        // bugfix: これ付けないとiPadで表示されなかった
        Cull Off ZWrite Off ZTest Always
        Tags
        {
            "RenderType"="Opaque"
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 worldPos : WORLD_POS;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color1;
            float4 _Color2;

            v2f vert(appdata v)
            {
                v2f o;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                const float dotResult = dot(i.worldPos/200,normalize(float2(1,1))); // 200: UI用に調整
                const float repeat = abs(dotResult - (_Time.w/5.0+20)); // 5:速度を遅める  +20:time0付近がボーダーが太い箇所があったためoffset
                const float interpolation = step(fmod(repeat,1),0.2);   // 0.2: ボーダーの太さ
                fixed4 col = lerp(_Color1, _Color2, interpolation);
                return col;
            }
            ENDCG
        }
    }
}
