﻿Shader "Eagle/StencilRainbow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Stencil {
            Ref 1
            Comp Always
            Pass Keep
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

//                fixed4 col = tex2D(_MainTex, i.uv);
//                return col;

                fixed3 hsb = fixed3(i.uv.x, 1.0, 1.0);
                fixed3 rgb = clamp(
                                abs(
                                    (hsb.x*6.0+fixed3(0.0,4.0,2.0)) % 6.0-3.0)-1.0, 0.0, 1.0 );  
                rgb = rgb*rgb*(3.0-2.0*rgb);
                rgb = hsb.z * lerp(fixed3(1.0,1.0,1.0), rgb, hsb.y);

                fixed4 rainbowCol = fixed4(rgb.r,rgb.g,rgb.b, 0.0f);
                
                return rainbowCol;

            }
            ENDCG
        }
    }
}
