Shader "Unlit/Tutorial_Shader" {
	Properties{
		_Colour("Colour", Color) = (1, 1, 1, 1)
		_MainTex("Main Texture", 2D) = "white" {}
	}

		SubShader{
			Cull Off
			Blend One OneMinusSrcAlpha
			Pass {
				CGPROGRAM
					#pragma vertex vertexFunction
					#pragma fragment fragmentFunction

					#include "UnityCG.cginc"

					sampler2D _MainTex;
					float4 _Colour;

					struct appdata {
						float4 vertex : POSITION;
						float2 uv : TEXCOORD0;
						float3 normal : NORMAL;
					};

					struct v2f {
						float4 position : SV_POSITION;
						float2 uv : TEXCOORD0;
					};

					

					v2f vertexFunction(appdata IN) {
						v2f OUT;
						//IN.vertex.xyz += IN.normal.xyz * _ExtrudeAmount * sin(_Time.y);
						OUT.position = UnityObjectToClipPos(IN.vertex);
						OUT.uv = IN.uv;
						return OUT;
					}

					fixed4 fragmentFunction(v2f IN) : COLOR {
						float4 textureColour = tex2D(_MainTex, IN.uv);
						//textureColour.rgb *= textureColour.a;
						//float4 temp = 
						//float4 dissolveColour = tex2D(_DissolveTexture, IN.uv);
						//clip(dissolveColour.rgb - _DissolveCutoff);
						return textureColour;
					}
				ENDCG
			}
		}
}