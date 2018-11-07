Shader "Unlit/ShadowMap"
{
	Properties
	{
		[PerRendererData] _LightPosition("LightPosition", Vector) = (0,0,0,0)
		[PerRendererData] _ShadowMapV("ShadowMapParams", Vector) = (0,0,0,0)
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always 
		Blend One One
		BlendOp Min

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "ShadowMap1D.cginc"

			float4 _LightPosition;			// xy is the position, z is the angle in radians, w is half the viewcone in radians
			float4 _ShadowMapParams;		// this is the row to write to in the shadow map. x is used to write, y to read.

			float Intersect(float2 lineOneStart, float2 lineOneEnd, float2 lineTwoStart, float2 lineTwoEnd)
			{
				float2 line2Perp = float2(lineTwoEnd.y - lineTwoStart.y, lineTwoStart.x - lineTwoEnd.x);
				float line1Proj = dot(lineOneEnd - lineOneStart, line2Perp);

				if (abs(line1Proj) < 1e-4)
					return 0.0f;

				float t1 = dot(lineTwoStart-lineOneStart,line2Perp ) / line1Proj;
				return t1;
			}

			struct appdata
			{
				float3 vertex1 : POSITION;
				float2 vertex2 : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;	
				float4 edge   : TEXCOORD0;		// xy=edgeVertex1,yz=edgeVertex2
				float2 data	  : TEXCOORD1;		// x=angle,y=unused
			};

			v2f vert (appdata v)
			{
				float polar1 = ToPolarAngle(v.vertex1.xy,_LightPosition.xy);
				float polar2 = ToPolarAngle(v.vertex2.xy,_LightPosition.xy);

				v2f o;
				o.edge = float4(v.vertex1.xy,v.vertex2.xy);
				o.edge = lerp(o.edge,o.edge.zwxy,step(polar1,polar2));
				
				float diff = abs(polar1-polar2);
				if (diff >= UNITY_PI)
				{ 
					float maxAngle = max(polar1,polar2);
					if (polar1 == maxAngle)
					{
						polar1 = maxAngle + 2 * UNITY_PI - diff;
					}
					else
					{
						polar1 = maxAngle;
					}
				}

				o.vertex = float4(PolarAngleToClipSpace(polar1), _ShadowMapParams.y, 0.0f, 1.0f);
									
				o.data = float2(polar1,0.f); 
				return o;
			}
		
			float4 frag (v2f i) : SV_Target
			{
				float angle = i.data.x;
				if (AngleDiff(angle,_LightPosition.z) > _LightPosition.w)
					return float4(0,0,0,0);
				
				float2 realEnd = _LightPosition.xy + float2(cos(angle) * 10, sin(angle) * 10);
				
				float t = Intersect(_LightPosition.xy, realEnd, i.edge.xy, i.edge.zw);
				
				return float4(t,t,t,t);
			}
			ENDCG
		}
	}
}