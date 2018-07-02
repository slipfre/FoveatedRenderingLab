Shader "Unlit/Combine Texture"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_PeripheryTex("Periphery Texture", 2D) = "white" {}
		_peripheryArea("Pheriphery Area", Vector) = (0, 0, 0, 0)
		_InnerTex("Inner Texture", 2D) = "black" {}
		_InnerArea("Inner Area", Vector) = (0, 0, 0, 0)
		_MediumTex("Medium Texture", 2D) = "red" {}
		_MediumArea("Medium Area", Vector) = (0, 0, 0, 0)
	}

	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			
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

			sampler2D _PeripheryTex;
			sampler2D _MediumTex;
			Vector _MediumArea;
			sampler2D _InnerTex;
			Vector _InnerArea;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                if (_ProjectionParams.x < 0)
                    o.vertex.y = 1 - o.vertex.y;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{             
				if( 
					i.vertex.x >= _InnerArea.x && i.vertex.x <= _InnerArea.z && 
					i.vertex.y >= _InnerArea.y && i.vertex.y <= _InnerArea.w
				){
					float2 innerTexUV = float2(
						(i.vertex.x - _InnerArea.x + 0.5) / (_InnerArea.z - _InnerArea.x + 1), 
						(i.vertex.y - _InnerArea.y + 0.5) / (_InnerArea.w - _InnerArea.y + 1)
					);
                    
					return tex2D(_InnerTex, innerTexUV);
				}

				if( 
					i.vertex.x >= _MediumArea.x && i.vertex.x <= _MediumArea.z && 
					i.vertex.y >= _MediumArea.y && i.vertex.y <= _MediumArea.w
				){
					float2 mediumTexUV = float2(
						(i.vertex.x - _MediumArea.x + 0.5) / (_MediumArea.z - _MediumArea.x + 1), 
						(i.vertex.y - _MediumArea.y + 0.5) / (_MediumArea.w - _MediumArea.y + 1)
					);
                    
					return tex2D(_MediumTex, mediumTexUV);
				}

				return tex2D(_PeripheryTex, i.uv);
			}
			ENDCG
		}
	}
}
