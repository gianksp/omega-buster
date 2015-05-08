Shader "Hidden/PP/Xffect/glow_per_obj/replacement" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	Category {
	Tags { "XftEffect"="GlowPerObj" "RenderType"="GlowPerObj" }
	Blend SrcAlpha  One 
	AlphaTest Greater .01
	ColorMask RGB
	Cull Off Lighting Off ZWrite Off Fog { Mode off }
	BindChannels {
	Bind "Color", color
	Bind "Vertex", vertex
	Bind "TexCoord", texcoord
	}
	
	SubShader {
		Pass
		{
			CGPROGRAM
			#pragma exclude_renderers xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles

			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			fixed4 _TintColor;
			sampler2D _CameraDepthTexture;

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 projPos : TEXCOORD1;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.projPos = ComputeScreenPos (o.pos);
				COMPUTE_EYEDEPTH(o.projPos.z);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return o;
			}
			
			//note here we do the ztest manually.
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 main = tex2D(_MainTex, i.texcoord);


				float sceneZ = LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
				float partZ = i.projPos.z;

				fixed4 ret = 2.0f * i.color * _TintColor * main;
				if (partZ > sceneZ)
				{
					clip ( -1.0 );
					ret = fixed4(0,0,0,0);
				}
					

				return ret;
			}
			ENDCG
		}
	} 
	
	// ---- Dual texture cards
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				constantColor [_TintColor]
				combine constant * primary
			}
			SetTexture [_MainTex] {
				combine texture * previous DOUBLE
			}
		}
	}
	
	// ---- Single texture cards (does not do color tint)
	SubShader {
		Pass {
			SetTexture [_MainTex] {
				combine texture * primary
			}
		}
	}
}
}
