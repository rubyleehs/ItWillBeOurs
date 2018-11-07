Shader "Custom/ChromaticAberration" {

    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }

    SubShader {

        Pass {
       
	        CGPROGRAM
	        #pragma vertex vert_img
	        #pragma fragment frag
	        #pragma fragmentoption ARB_precision_hint_fastest
	        #include "UnityCG.cginc"
	 
	        uniform sampler2D _MainTex;
	        uniform float _AberrationOffsetX;
	        uniform float _AberrationOffsetY;
	        uniform float _redAberrationDirection;
	        uniform float _greenAberrationDirection;
	        uniform float _blueAberrationDirection;
	 
	        float4 frag(v2f_img i) : COLOR {
	 
	            float2 coords = i.uv.xy;

	            float4 red = tex2D(_MainTex , coords.xy - float2(_AberrationOffsetX * _redAberrationDirection,_AberrationOffsetY * _redAberrationDirection));
	            float4 green = tex2D(_MainTex, coords.xy + float2(_AberrationOffsetX * _greenAberrationDirection,_AberrationOffsetY * _greenAberrationDirection));
	            float4 blue = tex2D(_MainTex, coords.xy + float2(_AberrationOffsetX * _blueAberrationDirection,_AberrationOffsetY * _blueAberrationDirection));
	           
	            float4 finalColor = float4(red.r, green.g, blue.b, 1.0f);
	            return finalColor;
	       
	        }
 
        	ENDCG

        }
    }
}