Shader "Xffect/PerObj/whitepass" {
	Properties {
	_MainTex ("", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
	Pass {
		CGPROGRAM
		#pragma vertex vert_surf
		#pragma fragment frag_surf
		#include "UnityCG.cginc"


		struct v2f_surf {
		  float4 pos : SV_POSITION;
		  float2 tex : TEXCOORD0;
		};

		v2f_surf vert_surf (appdata_full v) {
		  v2f_surf o;
		  o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
		  o.tex = v.texcoord;
		  return o;
		}

		fixed4 frag_surf (v2f_surf IN) : COLOR {
			fixed4 c = fixed4(1,1,1,1);
			return c;
		}

		ENDCG
		}
	}
}