Shader "Xffect/PerObj/mix" {
Properties {
	_MainTex ("", 2D) = "" {}
}
Subshader {
	ZTest Always Cull Off ZWrite Off Fog { Mode Off }

CGINCLUDE
#include "UnityCG.cginc"
sampler2D _MainTex;
float _Strength;
ENDCG


	//0
	// outline glow
	 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

	  CGPROGRAM
	  #pragma fragmentoption ARB_precision_hint_fastest
	  #pragma vertex vert
	  #pragma fragment frag
	  
	  #include "UnityCG.cginc"
		struct v2f {
			float4 pos : POSITION;
			half2 uv : TEXCOORD0;
		};
		

		sampler2D _BlurTex;
		sampler2D _WhiteTex;
		half4 _OutlineColor;

		v2f vert( appdata_img v ) {
			v2f o; 
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord.xy;
			return o;
		}
		half4 frag(v2f i) : COLOR {
			half4 sourceTex = tex2D(_MainTex, i.uv);
			
			half whiteTex = tex2D(_WhiteTex, i.uv).r;
			half4 blurTex = tex2D(_BlurTex, i.uv);
			half3 dif = saturate((blurTex-whiteTex).rgb);
			half lum = saturate(Luminance(dif*_Strength));
			half x = (lum*dif *_Strength);
			
			sourceTex.rgb =(1-lum)*(sourceTex.rgb)+x*_OutlineColor;
			sourceTex.a = sourceTex.a + x;
			return sourceTex;
		}
	  ENDCG
  }
    
	
	//1
	// glow
	 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

	  CGPROGRAM
	  #pragma fragmentoption ARB_precision_hint_fastest
	  #pragma vertex vert
	  #pragma fragment frag
	  
	  #include "UnityCG.cginc"
		struct v2f {
			float4 pos : POSITION;
			half2 uv : TEXCOORD0;
		};
		
		sampler2D _BlurTex;
		sampler2D _WhiteTex;
		half4 _OutlineColor;
		v2f vert( appdata_img v ) {
			v2f o; 
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord.xy;
			return o;
		}
		half4 frag(v2f i) : COLOR {
			half4 sourceTex = tex2D(_MainTex, i.uv);
			
			half whiteTex = tex2D(_WhiteTex, i.uv).r;
			half4 blurTex = tex2D(_BlurTex, i.uv);

			sourceTex.rgb = sourceTex.rgb + blurTex.rgb*_OutlineColor * _Strength;
			sourceTex.a = sourceTex.a + blurTex.a;
			return sourceTex;
		}
	  ENDCG
  }


}

Fallback off
}
