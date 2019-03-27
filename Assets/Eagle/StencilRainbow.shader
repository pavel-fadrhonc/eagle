Shader "Eagle/StencilRainbow"
{
    Properties
    {
        _Color ("Tint", Color) = (1,1,1,1)
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }
    SubShader
    {
        Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		//Blend DstAlpha OneMinusDstAlpha

        Stencil {
            Ref 1
            Comp Equal
            Pass Keep
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            
			#pragma target 2.0
			#pragma multi_compile_instancing
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA            

            #include "UnitySprites.cginc"
            #include "UnityCG.cginc"

            float4 _MainTex_ST;

            fixed4 frag (v2f i) : SV_Target
            {

//                fixed4 col = tex2D(_MainTex, i.uv);
//                return col;

                fixed3 hsb = fixed3(i.texcoord.x + _Time.y, 1.0, 1.0);
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
