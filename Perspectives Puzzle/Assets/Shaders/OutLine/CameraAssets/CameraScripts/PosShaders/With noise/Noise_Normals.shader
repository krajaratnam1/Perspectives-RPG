Shader "Jaytt/PostRendering/OutLine/With Noise/Noice_Normals"

{

	Properties
	{
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}

		_Noise("Line Noise", 2D) = "black" {}
		_Noise2("Color Noise", 2D) = "black" {}

		_Dx("Thickness", range(0.0, 10)) = 1
	    _Angle("Max Angle", range(0.0, 0.3)) = 0.1
		_DepthOL("Depth outline", range(0, 0.003)) = 0.003
		_OutLineOnly("OutLine Only", range(0.0, 1.0)) = 1.0
		_OutLineColor("Outline Color", Color) = (0,0,0,1)
		_isMobile("Mobile?", range(0,1)) = 1

	}

		SubShader
		{
			ZTest Always
			Pass
		{
			CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma target 3.0
		#include "UnityCG.cginc"
		
		sampler2D _MainTex, _Noise, _Noise2;

		uniform half4 _Noise_ST, _Noise2_ST;
		sampler2D _CameraDepthTexture;
		sampler2D _CameraDepthNormalsTexture;
		half _DepthOL, _OutLineOnly, _Angle, _Dx, _isMobile;

		fixed4 _OutLineColor;

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

	fixed getDepth(half4 pos)
	{
		return tex2D(_CameraDepthTexture, pos).x;
	}

	float DOT(half4 v1, half4 v2) {
		return dot(normalize(v1), normalize(v2));
	}

	bool test(half n1, half n2, half limit)
	{
		return abs(n1.x - n2.x) > limit;
	}
	bool normal_test(half3 b, half3 c)
	{
		return	test(b.x, c.x, _Angle) ||
			test(b.y, c.y, _Angle) ||
			test(b.z, c.z, _Angle);
	}


	fixed4 frag(v2f i) : SV_Target
	{

		half noise = (tex2D(_Noise, half2(i.screenuv.x*_Noise_ST.x + _Noise_ST.z, i.screenuv.y*_Noise_ST.y + _Noise_ST.w)).r*2)-1;
		half4 color_noise = _OutLineColor + tex2D(_Noise2, half2(i.screenuv.x*_Noise2_ST.x + _Noise2_ST.z, i.screenuv.y*_Noise2_ST.y + _Noise2_ST.w));
		half4 normals = (tex2D(_CameraDepthNormalsTexture, i.screenuv.xy) - 0.5)*2;
		half4 Colour = getColor(i.screenuv);

		_Dx = noise * _Dx;
		_OutLineColor =  color_noise;

		fixed deltaDepth ;

		half2 pixels = half2(_Dx / _ScreenParams.x, _Dx / _ScreenParams.y);
		half4 dx = half4(pixels.x+ i.screenuv.x, i.screenuv.y, i.screenuv.z, i.screenuv.w);
		half4 dy = half4(i.screenuv.x, pixels.y + i.screenuv.y,  i.screenuv.z, i.screenuv.w);

		half4 normalsx = getNormal(dx) ;
		half4 normalsy = getNormal(dy) ;

		if (_DepthOL !=0.003 && _isMobile < 1) {
			half Depth = getDepth(i.screenuv);
			half depthx  = abs(getDepth(dx) - Depth);
			half depthy  = abs(getDepth(dy) - Depth);

			deltaDepth = depthx + depthy;

			if (deltaDepth > _DepthOL) {
				return _OutLineColor;
			}else if (normals.z < 0.5 && (normalsx.z >0.5 || normalsy.z >0.5 )) {
				return _OutLineColor;
			}

		}
		else if (normal_test(normalsx, normals)) {
			return _OutLineColor;
		}
		else if (normal_test(normalsy, normals)) {
			return _OutLineColor;
		}
		else if (_OutLineOnly == 1) {
			return half4(1, 1, 1, 1);
		}
		
			return Colour * abs(_OutLineOnly - 1);
		

	}

		ENDCG
	}
	}
}
