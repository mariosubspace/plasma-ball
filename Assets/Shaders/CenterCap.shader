Shader "Custom/CenterCap" {
	Properties {
		[HDR] _BaseColor ("Base Color", Color) = (1,1,1,1)
		[HDR] _EdgeColor ("Edge Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="AlphaTest" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 viewDir : TEXCOORD1;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			float4 _BaseColor;
			float4 _EdgeColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.viewDir = WorldSpaceViewDir(v.vertex);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float n = 1 - saturate(dot(normalize(i.normal), normalize(i.viewDir)));
				n = n*n*n*n;
				fixed4 col = lerp(_BaseColor, _EdgeColor, n);
				col.a = 1.0 - pow(max(0, abs(1.0 - i.uv.y) * 3.0 - 2.0), 2.0);
				return col;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
