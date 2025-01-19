Shader "UI/BlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1.0
        _Alpha ("Alpha", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
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
            float _BlurSize;
            float _Alpha;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 color = float4(0, 0, 0, 0);
                float2 blurUV = i.uv;

                // Sample 9 times for basic Gaussian blur
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        blurUV.x = i.uv.x + x * _BlurSize * 0.001;
                        blurUV.y = i.uv.y + y * _BlurSize * 0.001;
                        color += tex2D(_MainTex, blurUV);
                    }
                }

                color /= 9.0; // Normalize the color
                color.a = _Alpha; // Apply alpha transparency
                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
