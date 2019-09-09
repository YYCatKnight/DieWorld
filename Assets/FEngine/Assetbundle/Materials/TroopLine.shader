// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FEngine/TroopLine"
{
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
				
			#include "UnityCG.cginc"
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed3 color2 : NORMAL;
				fixed4 color : COLOR;
			};
	
			sampler2D _MainTex;
			float4 _MainTex_ST;
				
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;

				if (abs(o.color.g) < 0.01)
				{
					o.color2.x = 1;
					o.color2.y = 1;
					o.color2.z = 1;
				}
				else if (abs(o.color.g - 0.1) < 0.01)
				{
					o.color2.x = 0.33;
					o.color2.y = 0.92;
					o.color2.z = 1;
				}
				else if (abs(o.color.g - 0.2) < 0.01)
				{
					o.color2.x = 1;
					o.color2.y = 0.7;
					o.color2.z = 0.26;
				}
				else if (abs(o.color.g - 0.3) < 0.01)
				{
					o.color2.x = 0.33;
					o.color2.y = 1;
					o.color2.z = 0.34;
				}
				else
				{
					o.color2.x = 0.98;
					o.color2.y = 0.22;
					o.color2.z = 0.22;
				}
				return o;
			}
				
			fixed4 frag (v2f i) : COLOR
			{					
				float a = i.color.a;

				float2 uv = i.texcoord;

				uv.x = uv.x * 2 / i.color.r;

				i.color.r = i.color2.x;
				i.color.g = i.color2.y;
				i.color.b = i.color2.z;
				i.color.a = 1;

				fixed4 col = tex2D(_MainTex, uv - float2(_Time.y * 5 * a + i.color.r, 0)) * i.color;
				return col;
			}
			ENDCG 
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
    
	 FallBack "Diffuse"
}
