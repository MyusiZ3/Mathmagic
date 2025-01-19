Shader "UI/RoundedCornersRect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Corner Radius", Range(0, 0.5)) = 0.1 // Radius untuk sudut membulat
        _AspectRatio ("Aspect Ratio (Width/Height)", Float) = 1.0 // Rasio aspek gambar
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha // Membuat background transparan
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Radius; // Besar radius sudut membulat
            float _AspectRatio; // Rasio aspek untuk gambar persegi panjang

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Offset UV agar 0.5 menjadi pusat kotak
                float2 offsetUV = uv - 0.5;

                // Skala UV berdasarkan rasio aspek agar rounding proporsional
                offsetUV.x *= _AspectRatio;

                // Menghitung jarak ke tepi sudut
                float2 absUV = abs(offsetUV) - (0.5 - _Radius);

                // Hitung hanya bagian sudut yang perlu di-rounding
                float dist = length(max(absUV, 0.0));

                // Jika di luar radius rounded, discard pixel
                if (max(absUV.x, absUV.y) > 0 && dist > _Radius)
                {
                    discard;
                }

                // Kembalikan tekstur utama dengan transparansi
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
