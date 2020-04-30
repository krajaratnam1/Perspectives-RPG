
Shader "Jaytt/PostRendering/OutLine/Contrast based"
{

	Properties
	{
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}
		_Dx("Thickness", range(0.0, 10)) = 1
		_ContrastON("Intensity correction", range(0, 0.3)) = 0.3
		_OutLineOnly("OutLine Only", range(0.0, 1.0)) = 1.0
		_OutLineColor("Outline Color", Color) = (0,0,0,1)
	}

		SubShader
	{
		//Tags{ "RenderType" = "Transparent" }
		ZTest Always
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 3.0
#include "UnityCG.cginc"

	sampler2D _MainTex;
	sampler2D _CameraDepthTexture;
	sampler2D _CameraDepthNormalsTexture;
	half _ContrastON;
	half _OutLineOnly;
	half _Dx;
	half4 _OutLineColor;

	struct v2f {
		half4 pos : SV_POSITION;
		half4 screenuv : TEXCOORD0;
	};

	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.screenuv = ComputeScreenPos(o.pos);
		return o;
	}

	fixed4 getColor(half4 pos)
	{
		return tex2D(_MainTex, pos);
	}

	float DOT(half4 v1, half4 v2) {
		return dot(normalize(v1), normalize(v2));
	}

	bool test(half color1, half color2) {

		return abs(color1 - color2) > 0.1;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		half4 normals = tex2D(_CameraDepthNormalsTexture, i.screenuv.xy); normals.w = 0;

		half2 pixels = half2(_Dx / _ScreenParams.x, _Dx / _ScreenParams.y);
		half4 dx = half4(pixels.x + i.screenuv.x, i.screenuv.y, i.screenuv.z, i.screenuv.w);
		half4 dy = half4(i.screenuv.x, pixels.y + i.screenuv.y, i.screenuv.z, i.screenuv.w);
		half4 dx1 = half4(-pixels.x + i.screenuv.x, i.screenuv.y, i.screenuv.z, i.screenuv.w);
		half4 dy1 = half4(i.screenuv.x, -pixels.y + i.screenuv.y, i.screenuv.z, i.screenuv.w);

		half4 Colour = getColor(i.screenuv);

		half4 Colorx = getColor(dx);
		half4 Colory = getColor(dy);
		half4 Colorx1 = getColor(dx1);
		half4 Colory1 = getColor(dy1);

		half deltaIntensity = (4*length(Colour.xyz) - length(Colorx.xyz) - length(Colorx1.xyz) - length(Colory.xyz) - length(Colory1.xyz));
		if ((deltaIntensity) > _ContrastON ) {
			return _OutLineColor;
		}
		else if (test(Colorx.x, Colour.x) || test(Colorx.y, Colour.y)  || test(Colorx.z, Colour.z) ||
			test(Colorx1.x, Colour.x)|| test(Colorx1.y , Colour.y)|| test(Colorx1.z, Colour.z)||
			test(Colory.x, Colour.x) || test(Colory.y, Colour.y)  || test(Colory.z, Colour.z) ||
			test(Colory1.x, Colour.x)|| test(Colory1.y, Colour.y) || test(Colory1.z, Colour.z) ) 
		{
			return _OutLineColor;
		}
		else if (_OutLineOnly == 1) {
			return fixed4(1, 1, 1, 1);
		}
		else {
			return Colour * abs(_OutLineOnly - 1);
		}
	}

		ENDCG
	}
	}
}
