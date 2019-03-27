Shader "Eagle/EagleRainbow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        
        _Color ("Tint", Color) = (1,1,1,1)
		[HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        
        
		_Outline("Outline", Float) = 0
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_OutlineSize("Outline Size", int) = 1        
       
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
		
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha	
		
        Pass
        {
            Stencil 
            {
                Ref 1
                Comp Always
                Pass Replace
            }		
        
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
            float4 _MainTex_TexelSize;
            
			float _Outline;
			fixed4 _OutlineColor;
			int _OutlineSize;
                        
            static const fixed PI = 3.1415926;

            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 col = SampleSpriteTexture(IN.texcoord) * IN.color;
                

				if (_Outline > 0 && col.a != 0) {
					float totalAlpha = 1.0;

					[unroll(16)]
					for (int i = 1; i < _OutlineSize + 1; i++) {
						fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y));
						fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0,i *  _MainTex_TexelSize.y));
						fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0));
						fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0));

						totalAlpha = totalAlpha * pixelUp.a * pixelDown.a * pixelRight.a * pixelLeft.a;
					}

					if (totalAlpha == 0) {
						col.rgba = fixed4(1, 1, 1, 1) * _OutlineColor;
						discard;
					}
				}   

                col.rgb *= col.a;    
                
                if (col.a == 0)
                    discard;         
                
                return col;
            }
            ENDCG
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
            float4 _MainTex_TexelSize;
            
			float _Outline;
			fixed4 _OutlineColor;
			int _OutlineSize;
        
            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 col = SampleSpriteTexture(IN.texcoord) * IN.color;            
                
				if (_Outline > 0 && col.a != 0) {
					float totalAlpha = 1.0;

					[unroll(16)]
					for (int i = 1; i < _OutlineSize + 1; i++) {
						fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y));
						fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0,i *  _MainTex_TexelSize.y));
						fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0));
						fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0));

						totalAlpha = totalAlpha * pixelUp.a * pixelDown.a * pixelRight.a * pixelLeft.a;
					}

					if (totalAlpha == 0) {
						col.rgba = fixed4(1, 1, 1, 1) * _OutlineColor;
					}
				}   

                col.rgb *= col.a;    
                
                if (col.a == 0)
                    discard;         
                
                return col;
            }        
        ENDCG
        }
    }
}
