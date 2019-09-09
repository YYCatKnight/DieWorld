//区域画线
Shader "Custom/ZoneBroder"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}
	
	SubShader
	{
		Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
		LOD 100
		
		ZTest Always
		ZWrite Off
		Cull Off
		Blend One OneMinusSrcAlpha
  
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
						
			#include "UnityCG.cginc"
			
			struct appdata
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				
				#ifdef PIXELSNAP_ON
                o.vertex = UnityPixelSnap (o.vertex);
                #endif
				
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.texcoord;
				fixed4 col = tex2D(_MainTex, uv) * i.color;
				fixed4 c; 
				c.w   = col.w;
				c.xyz = col.xyz * col.w;			
				return c;
			}
			
			ENDCG
		}
		

	}
}
