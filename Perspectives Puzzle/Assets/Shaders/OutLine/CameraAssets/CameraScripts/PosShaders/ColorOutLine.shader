
Shader "Jaytt/PostRendering/OutLine/Color based"
{

	Properties
	{
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}
		_Dx("Thickness", range(0.0, 10)) = 1

		_Contrast_Check("Contrast", range(-0.1, 1.1)) = 1
		_Angle("Normal's Angle", range(0.0, 0.999)) = 1
	    _Angle2("Color's angle", range(0.6, 0.88)) = 0.88
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
	half _OutLineOnly;
	half _Dx, 
		_Angle,
		_Angle2,
		_Contrast_Check,
		_test;
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

	fixed4 getNormal(half4 pos)
	{
		return (tex2D(_CameraDepthNormalsTexture, pos) - 0.5) * 2;
	}

	float DOT(half3 v1, half3 v2) {
		return dot(normalize(v1), normalize(v2));
	}

	fixed4 frag(v2f i) : SV_Target
	{

		half4 normals = (tex2D(_CameraDepthNormalsTexture, i.screenuv.xy) - 0.5) * 2;; normals.w = 0;

	half2 pixels = half2(_Dx / _ScreenParams.x, _Dx / _ScreenParams.y);
	half4 dx = half4(pixels.x + i.screenuv.x, i.screenuv.y, i.screenuv.z, i.screenuv.w);
	half4 dy = half4(i.screenuv.x, pixels.y + i.screenuv.y, i.screenuv.z, i.screenuv.w);
	half4 dx1 = half4(-pixels.x + i.screenuv.x, i.screenuv.y, i.screenuv.z, i.screenuv.w);
	half4 dy1 = half4(i.screenuv.x, -pixels.y + i.screenuv.y, i.screenuv.z, i.screenuv.w);

	half4 Colour = getColor(i.screenuv);

	half4 Colorx  = getColor(dx);
	half4 Colory  = getColor(dy);
	half4 Colorx1 = getColor(dx1);
	half4 Colory1 = getColor(dy1);

	half4 angle= half4 ( DOT(Colorx.xyz,Colour.xyz), DOT(Colorx1.xyz, Colour.xyz), DOT(Colory.xyz, Colour.xyz), DOT(Colory1.xyz, Colour.xyz));
	
	half Angle = length(angle)*0.25;

	Angle = (Angle - 0.35) *5.88;


	if (Angle < _Angle2)
		return _OutLineColor;

    half check = length(Colorx) +length(Colorx1) +length(Colory)+ length(Colory1)- 4* length(Colour);

	half4 normalsx  = getNormal(dx) ;
	half4 normalsy  = getNormal(dy) ;
	half4 normalsx1 = getNormal(dx1);
	half4 normalsy1 = getNormal(dy1);
		
		half angleX  = abs(dot(normals.xyz, normalsx.xyz))  / (length(normalsx.xyz)*length(normals.xyz));
		half angleY  = abs(dot(normals.xyz, normalsy.xyz))  / (length(normalsy.xyz)*length(normals.xyz));
		half angleX1 = abs(dot(normals.xyz, normalsx1.xyz)) / (length(normalsx1.xyz)*length(normals.xyz));
		half angleY1 = abs(dot(normals.xyz, normalsy1.xyz)) / (length(normalsy1.xyz)*length(normals.xyz));

		if ((angleX < _Angle) || 
			(angleY < _Angle) || 
			(angleX1 < _Angle) || 
			(angleY1 < _Angle) || 
			check > _Contrast_Check) {
			return _OutLineColor;
		}

	if (_OutLineOnly == 1) {
		return  fixed4(1,1,1,1); 
	}

	return Colour * abs(_OutLineOnly - 1);
	}
		ENDCG
	}
	}
}
