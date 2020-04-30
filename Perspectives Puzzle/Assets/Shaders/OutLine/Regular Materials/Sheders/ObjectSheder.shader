// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Jaytt/Toon/LightToon"
{
	Properties
	{
		_Color("Color(RGBA)", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_CutAngle("CutAngle", range(-1.0, 1.0)) = 0.1
		_Shadow("Shadow intensity", range(0, 1.0)) = 0.6

	}

		SubShader
	{
		// Settings
		Tags{ "LightMode" = "ForwardBase" "RenderType" = "Opaque" }
			Pass {
				

			// Surface Shader Pass ( Front )
			Cull Back
			//ZTest
			ZWrite On
		    Lighting On

			//AlphaTest Equal 1
			//Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
            #pragma multi_compile_fwdbase



			float4    _Color;
			sampler2D _MainTex;


			    float _CutAngle; 
				float _Shadow;

			struct appdata {
				float4 vertex : POSITION;
				float4 normals : NORMAL;
				half3  uv : TEXCOORD0;

			};

			struct v2f {
				float4 pos  : SV_POSITION;
				half3 normal : TEXCOORD1;
				half3  uv : TEXCOORD0;
				fixed3     viewDirection : TEXCOORD2;
		
				LIGHTING_COORDS(4, 6)
			};

			v2f vert(appdata v)
			{
				v2f o;
				half4 posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normals);
				o.uv = v.uv;
				o.viewDirection = normalize(_WorldSpaceCameraPos.xyz - posWorld.xyz);

				TRANSFER_VERTEX_TO_FRAGMENT(o);
				return o;
			}


			fixed4 frag(v2f i) : SV_Target
			{
				float lightIntencity = 0;
			    half atten = LIGHT_ATTENUATION(i);
				float4 light = dot(i.normal, _WorldSpaceLightPos0);

				if (light.x > _CutAngle && atten  > 0.5) {
					lightIntencity = 1;
				}
				else {
					lightIntencity = _Shadow;
				}


			    fixed4 col = tex2D(_MainTex, i.uv);

				return lightIntencity *col*_Color;
			}

		ENDCG
		}
	}
				Fallback "VertexLit"
}
