// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Jaytt/Toon/LightToon(no shadow)"
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
		Pass{


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


	};

	v2f vert(appdata v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.normal = UnityObjectToWorldNormal(v.normals);
		o.uv = v.uv;

		return o;
	}


	fixed4 frag(v2f i) : SV_Target
	{
		float lightIntencity = 0;
	float4 light = dot(i.normal, _WorldSpaceLightPos0);

	if (light.x > _CutAngle) {
		lightIntencity = 1;
	}
	else {
		lightIntencity = _Shadow;
	}


	fixed4 col = tex2D(_MainTex, i.uv);

	if (col.w < 0.3) {
		discard;
	}

	return lightIntencity * col;
	}

		ENDCG
	}
	}
		Fallback "VertexLit"
}
