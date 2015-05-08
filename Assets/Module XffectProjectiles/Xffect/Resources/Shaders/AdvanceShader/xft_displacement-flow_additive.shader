Shader "Xffect/displacement-flow/additive" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Main Texture", 2D) = "white" {}
 _DispMap ("Displacement Map (RG)", 2D) = "white" {}
 _FlowMap ("FlowMap (RG)", 2D) = ""{}
 _Mask ("Mask (R)", 2D) = "white" {}

 _DispScrollSpeedX  ("Displacement Map Scroll Speed X", Float) = 0
 _DispScrollSpeedY  ("Displacement Map Scroll Speed Y", Float) = 0
 _DispX  ("Displacement Strength X", Float) = 0
 _DispY  ("Displacement Strength Y", Float) = 0.2
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

         uniform sampler2D _MainTex;
		 uniform sampler2D _DispMap;
         uniform sampler2D _FlowMap;
		 uniform sampler2D _Mask;

		 
         uniform half _DispScrollSpeedX;
         uniform half _DispScrollSpeedY;
		 uniform half _DispX;
         uniform half _DispY;


         uniform fixed4 _TintColor;
         
         struct appdata_t {
             float4 vertex : POSITION;
             fixed4 color : COLOR;
			 float2 texcoord : TEXCOORD0;
             float2 param : TEXCOORD1;
         };

         struct v2f {
             float4 vertex : POSITION;
             fixed4 color : COLOR;
             float2 texcoord : TEXCOORD0;
			 float2 param : TEXCOORD1;
         }; 
         

		 //PARAM X: offset x
		 //PARAM Y: offset y

		 //param.y - param.x = stretch length

         v2f vert (appdata_t v)
         {
             v2f o;
             o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
             o.color = v.color;
             o.texcoord = v.texcoord;
			 o.param = v.param;
             return o;
         }
         fixed4 frag (v2f i) : COLOR
         {

			 //get displacement offset
			 half2 mapoft = half2(_Time.y*_DispScrollSpeedX, _Time.y*_DispScrollSpeedY);
			 half4 dispColor = tex2D(_DispMap, i.texcoord + mapoft);
		     half noise1 =  dispColor.r * _DispX;
			 half noise2 =  dispColor.g * _DispY;


			 //get flow
			 half4 flowMap = tex2D (_FlowMap, i.texcoord);
			 flowMap.r = flowMap.r * 2.0f - 1.011765;
			 flowMap.g = flowMap.g * 2.0f - 1.003922;


			 half s = i.param.x;


			 half s1 = fmod(s, 2.0f * i.param.y);
			 half s2 = fmod(i.param.y + s, 2.0f * i.param.y);

			 half4 t1 = tex2D (_MainTex, i.texcoord + flowMap.rg * s1  + noise1); 		 	
			 half4 t2 = tex2D (_MainTex, i.texcoord + flowMap.rg * s2  + noise2);
			 
			 fixed t = abs (i.param.y - s1) / (i.param.y);
			

			 half4 final = lerp( t1, t2, t);

			 //get mask
             fixed mask = tex2D(_Mask, i.texcoord).r;

			 //final
			 return 2.0f * i.color * _TintColor * final * mask;
         }
         ENDCG
     }
 }   
}
}