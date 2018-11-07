// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Sprites/Light"
{
	Properties
	{
		[PerRendererData] _ShadowTex ("Texture", 2D) = "white" {}
		[PerRendererData] _Color ("Color", Color) = (1,1,1,1)
		[PerRendererData] _LightPosition("LightPosition", Vector) = (0,0,1,0)
		[PerRendererData] _ShadowMapParams("ShadowMapParams", Vector) = (0,0,0,0)
		[PerRendererData] _Params2("Params2", Vector) = (0,0,0,0)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Geometry" 
			"IgnoreProjector"="True" 
			"RenderType"="Opaque" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite On
		Blend Zero Zero, SrcAlpha One

		Pass
		{
		CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "ShadowMap1D.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				float4 modelPos : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
			};
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.modelPos = IN.vertex;
				OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex);
				return OUT;
			}

			sampler2D 	_ShadowTex;
			float4 		_LightPosition;
			float4 		_ShadowMapParams;
			float4 		_Params2;
			fixed4 		_Color;
			
			fixed4 frag(v2f IN) : SV_Target {

				fixed4 c = _Color;

				float2 polar = ToPolar(IN.worldPos.xy,_LightPosition.xy) - float2(0,0.25);

				float shadow = SampleShadowTexturePCF(_ShadowTex,polar,_ShadowMapParams.x);
				if (shadow < 0.5f) {
					clip( -1.0 );
					return c;
				}
				
				float distFalloff = max(0.0f,_Params2.w - polar.y) * _Params2.z;
				distFalloff = clamp(distFalloff,0.0f,1.0f);
				float distFalloffExp = pow(distFalloff,_LightPosition.z);

				float angleFalloff = AngleDiff(polar.x, _Params2.x) / _Params2.y;
				angleFalloff = clamp(angleFalloff, 0.0f, 1.0f);
				angleFalloff = pow(1.0f - angleFalloff, _LightPosition.w);

				c.rgb *= distFalloffExp * angleFalloff;
                c.a = distFalloffExp * angleFalloff;

				return c;
			}
		ENDCG
		}
	}
}
