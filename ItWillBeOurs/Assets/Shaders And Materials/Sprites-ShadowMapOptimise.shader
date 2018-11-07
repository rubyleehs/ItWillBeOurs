Shader "Sprites/ShadowMapOptimise"
{
	Properties
	{
		[PerRendererData] _ShadowMap ("Texture", 2D) = "white" {}
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
		Blend One Zero

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "ShadowMap1D.cginc"
			
			struct appdata_t
			{
				float4 vertex    : POSITION;
				float2 texcoords : TEXCOORD0; 
			};

			appdata_t vert(appdata_t IN)
			{
				return IN;
			}

			sampler2D _ShadowMap;

			fixed4 frag(appdata_t IN) : SV_Target
			{
				float u = IN.texcoords.x * 2.0f / 3.0f;
				float v = IN.texcoords.y;
				float s = tex2D(_ShadowMap, float2(u,v)).r;
				if (u < 1.0f / 3.0f) 
				{
					s = min(s,tex2D(_ShadowMap, float2(u + (2.0f / 3.0f), v)).r);
				}
				return fixed4(s,s,s,s);
			}
		ENDCG
		}
	}
}
