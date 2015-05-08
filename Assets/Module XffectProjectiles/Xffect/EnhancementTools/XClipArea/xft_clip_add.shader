Shader "Xffect/clip/additive" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Main Texture", 2D) = "white" {}
}

Category {
 Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
 Blend SrcAlpha One
 Cull Off Lighting Off ZWrite Off
 BindChannels {
     Bind "Color", color
     Bind "Vertex", vertex
     Bind "TexCoord", texcoord
 }
 
 // ---- Fragment program cards
 SubShader {
     Pass {
     
         CGPROGRAM
         #pragma vertex vert
         #pragma fragment frag
         #pragma fragmentoption ARB_precision_hint_fastest
         #pragma multi_compile_particles
         
         #include "UnityCG.cginc"

         sampler2D _MainTex;
         fixed4 _TintColor;
		 uniform float4 _ClipParam;
         
         struct appdata_t {
             float4 vertex : POSITION;
             fixed4 color : COLOR;
             float2 texcoord : TEXCOORD0;
         };

         struct v2f {
             float4 vertex : POSITION;
             fixed4 color : COLOR;
             float2 texcoord : TEXCOORD0;
			 float3 wpos : TEXCOORD1;
         };
         
         v2f vert (appdata_t v)
         {
             v2f o;
			 o.wpos = mul (_Object2World, v.vertex);
			 o.color = v.color;
             o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
             o.texcoord = v.texcoord;
             return o;
         }
         fixed4 frag (v2f i) : COLOR
         {
			float dist = distance(i.wpos, float4(_ClipParam.x,_ClipParam.y,_ClipParam.z,1.0));
			fixed4 mainColor = tex2D(_MainTex, i.texcoord);
			if (dist < _ClipParam.w) 
				mainColor.a = 0.0;

			return 2.0f * i.color * _TintColor * mainColor;
         }
         ENDCG
     }
 }   
}
}