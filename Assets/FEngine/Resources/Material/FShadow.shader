// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FShadow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	    _Strength("_Strength", Range(0, 0.2)) = 0.1
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" }
		LOD 100
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
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
				float4 uvShadow:TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Strength;
			uniform float4x4 ShadowMatrix;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float4x4 matWVP = mul(ShadowMatrix, unity_ObjectToWorld);
				o.uvShadow = mul(matWVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				half2 uv = i.uvShadow.xy / i.uvShadow.w * 0.5 + 0.5;
#if UNITY_UV_STARTS_AT_TOP 
				uv.y = 1 - uv.y;
#endif 
				float pad = 888;
				fixed4 res = fixed4(0, 0, 0, 0);	
				fixed4 texS = tex2D(_MainTex,uv);
				if (texS.a > 0.1f)
				{
					res.a = _Strength;
				}
				texS = tex2D(_MainTex, uv + half2(-0.94201624 / pad, -0.39906216 / pad));
				if (texS.a > 0)
				{
					res.a += _Strength;
				}

				texS = tex2D(_MainTex, uv + half2(0.94558609 / pad, -0.76890725 / pad));
				if (texS.a > 0)
				{
					res.a += _Strength;
				}

				texS = tex2D(_MainTex, uv + half2(-0.094184101 / pad, -0.92938870 / pad));
				if (texS.a > 0)
				{
					res.a += _Strength;
				}
				texS = tex2D(_MainTex, uv + half2(0.34495938 / pad, 0.29387760 / pad));
				if (texS.a > 0)
				{
					res.a += _Strength;
				}
				//return tex2D(_MainTex, i.uv);
				return res;

			}
			ENDCG
		}
	}
}
