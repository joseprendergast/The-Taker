// Anti-aliased "Sharp" text shader. For nicer quality hi-res fonts.
Shader "Powerhoof/Sharp Text Shader AA" {
	Properties {
		_MainTex ("Font Texture", 2D) = "white" {}
		_Color ("Text Color", Color) = (1,1,1,1)
		_Offset ("Offset", Vector) = (0,0,0,0)
	}

	SubShader {

		Tags {
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
			"PreviewType"="Plane"
		}
		Lighting Off Cull Off ZTest Always ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform fixed4 _Color;
			uniform float4 _Offset;			
			uniform float4 _MainTex_TexelSize;			


			// From TylerGlaiel
			float4 texture2DAA(sampler2D tex, float2 uv)
			{
				float2 texsize = float2(_MainTex_TexelSize.z, _MainTex_TexelSize.w);
				float2 uv_texspace = uv * texsize;
				float2 seam = floor(uv_texspace + .5);
				uv_texspace = (uv_texspace - seam) / fwidth(uv_texspace) + seam;
				uv_texspace = clamp(uv_texspace, seam - .5, seam + .5);

				float4 result = tex2D(tex, uv_texspace / texsize);

				// Since tranparent parts will have a color of "black" multiply any color with inverse of the color to get that back
				float ratio = result.a;
				result /= ratio;
				result.a = ratio;
				return result;
			}

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(unity_ObjectToWorld, v.vertex);				
				o.vertex = mul(UNITY_MATRIX_VP, o.vertex + _Offset);
				o.color = v.color * _Color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}

			float invLerp(float from, float to, float value)
			{
				return (value - from) / (to - from);
			}


			fixed4 frag (v2f i) : SV_Target
			{ 
				
				fixed4 col = i.color;
				//col.a = col.a * texture2DAA(_MainTex, i.texcoord).a;									
				col.a = col.a * (invLerp(0.3f,0.7f, texture2DAA(_MainTex, i.texcoord).a));				
				return col;
			}
			ENDCG
		}
	}
}
