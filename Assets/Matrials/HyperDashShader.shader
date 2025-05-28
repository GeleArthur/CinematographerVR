Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2DArray) = "" {}
        _LightTex ("Light", 2D) = "" {}
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1: TEXCOORD1;
                float2 uv2: TEXCOORD2;
            };

            struct v2f
            {
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _LightTex;
            float4 _LightTex_ST;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = v.uv0;
                o.uv1 = v.uv1;
                o.uv2 = v.uv2;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            UNITY_DECLARE_TEX2DARRAY(_MainTex);

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                // fixed4 col = fixed4(i.uv2.y,0, 0, 1);
                fixed4 col = UNITY_SAMPLE_TEX2DARRAY(_MainTex, float3(i.uv0, i.uv2.x));
                col *= tex2D(_LightTex, i.uv1 * _LightTex_ST.xy + _LightTex_ST.zw);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
