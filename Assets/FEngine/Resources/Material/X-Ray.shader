// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "X-Ray"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_ChangeColor("Change color", COLOR) = (1,1,1,0.5)
	}

		SubShader
	{
		Tags{ "RenderType" = "Transparent+100" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
	{
		ZWrite Off
		ZTest Off
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
	float4 _ChangeColor;
	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		fixed4 c = tex2D(_MainTex, i.uv)*_ChangeColor;
	return c;
	}
		ENDCG
	}

		Pass
	{
		ZWrite On
		ZTest On
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
	float4 _ChangeColor;
	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture
		fixed4 c = tex2D(_MainTex, i.uv);
	return c;
	}
		ENDCG
	}

	}
}