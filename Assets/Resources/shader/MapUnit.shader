Shader "Unlit/MapUnit"
{
	Properties
	{
	}
	
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 tangent : TANGENT;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};
			
			float4 colors[100];

			v2f vert (appdata v)
			{
				v2f o;
				
				o.color = colors[round(v.tangent.x)];
				
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return i.color;
			}
			ENDCG
		}
	}
}
