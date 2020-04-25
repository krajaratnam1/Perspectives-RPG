
Shader "Jaytt/PostRendering/OutLine/Normals based"

{

	Properties
	{
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}
		_Dx("Thickness", range(0, 10)) = 0.005
	    _Angle("Max Angle", range(0.0, 0.3)) = 0.1
		_Contrast("Intensity correction", range(0, 1)) = 1
		_DepthOL("Depth outline", range(0, 0.0012)) = 0.003
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

			sampler2D _Replace, _MainTex, _CameraDepthTexture, _CameraDepthNormalsTexture;
			half _DepthOL,_OutLineOnly, _Dx, _Angle, _Contrast,_isMobile;
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

		fixed4 getColor(half4 pos){ //return color at position
			return tex2D(_MainTex, pos);
		}

		fixed4 getReplace(half4 pos)//return color at position
		{
			return tex2D(_Replace, pos);
		}

		fixed4 getNormal(half4 pos)//return color at position
		{
			return (tex2D(_CameraDepthNormalsTexture, pos) - 0.5) * 2;
		}

		fixed getDepth(half4 pos)//return color at position
		{
			return tex2D(_CameraDepthTexture, pos).x;
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

		bool color_test(half3 b,half3 c) 
		{
			return	test(b.x, c.x, 0.1) || 
					test(b.y, c.y, 0.1) || 
					test(b.z, c.z, 0.1);
		}

		bool check_cam2(half4 color1, half4 color2) 
		{
			half t =abs(color1.x - color2.x) +
					abs(color1.y - color2.y) +
					abs(color1.z - color2.z) ;
		
			return t < 0.1;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			half4	Colour = getColor(i.screenuv),
					Replace = getReplace(i.screenuv);

			half deltaDepth = 1, Angle = 1;

			half2 pixels = half2(_Dx / _ScreenParams.x, _Dx / _ScreenParams.y);

			if (length(Replace) >0.1 && check_cam2(Replace, Colour)) {

				half2 pixels_ = pixels / _Contrast;
				half4 dx  = half4(pixels_.x + i.screenuv.x, i.screenuv.y, i.screenuv.z, i.screenuv.w);
				half4 dy  = half4(i.screenuv.x, pixels_.y + i.screenuv.y, i.screenuv.z, i.screenuv.w);
				half4 dx1 = half4(-pixels_.x + i.screenuv.x, i.screenuv.y, i.screenuv.z, i.screenuv.w);
				half4 dy1 = half4(i.screenuv.x, -pixels_.y + i.screenuv.y, i.screenuv.z, i.screenuv.w);

				half4 Colorx  = getReplace(dx);
				half4 Colory  = getReplace(dy);
				half4 Colorx1 = getReplace(dx1);
				half4 Colory1 = getReplace(dy1);

				if (color_test(Colorx.xyz,  Replace.xyz)||
					color_test(Colorx1.xyz, Replace.xyz)||
					color_test(Colory.xyz,  Replace.xyz)||
					color_test(Colory1.xyz, Replace.xyz))
				{
					return _OutLineColor;
				}
				else if (_OutLineOnly == 1) {
					return half4(1, 1, 1, 1);
				}
				else {
					return Colour * abs(_OutLineOnly - 1);
				}
			}
	
			half4 dx  = half4(pixels.x + i.screenuv.x, i.screenuv.y, i.screenuv.z, i.screenuv.w);
			half4 dy  = half4(i.screenuv.x, pixels.y + i.screenuv.y, i.screenuv.z, i.screenuv.w);
			half4 dx1 = half4(-pixels.x + i.screenuv.x, i.screenuv.y, i.screenuv.z, i.screenuv.w);
			half4 dy1 = half4(i.screenuv.x, -pixels.y + i.screenuv.y, i.screenuv.z, i.screenuv.w);

			half4 normals = (tex2D(_CameraDepthNormalsTexture, i.screenuv.xy) - 0.5) * 2;

			half4 normalsx  = getNormal(dx)  ;
			half4 normalsy  = getNormal(dy)  ;
			half4 normalsx1 = getNormal(dx1) ;
			half4 normalsy1 = getNormal(dy1) ;

			half Depth = getDepth(i.screenuv);

			if (_DepthOL !=0.003 && _isMobile < 1) 
			{
				half depthx  = abs(getDepth(dx) - Depth);
				half depthy  = abs(getDepth(dy) - Depth);
				half depthx1 = abs(getDepth(dx1) - Depth);
				half depthy1 = abs(getDepth(dy1) - Depth);

				deltaDepth = depthx + depthy + depthx1 + depthy1;

				if (deltaDepth > _DepthOL) {
					return _OutLineColor;
				}
				else {
					deltaDepth = 1;
				}

				if (normals.z < 0.5 && (normalsx.z >0.5 || normalsy.z >0.5 || normalsx1.z >0.5 || normalsy1.z >0.5)) 
				{
					return _OutLineColor;
				}
			}

			if (normal_test(normalsx.xyz, normals.xyz))
			{
				return _OutLineColor;
			}
			else if (normal_test(normalsx1.xyz, normals.xyz))				
			{
				return _OutLineColor;
			}
			else if (normal_test(normalsy.xyz, normals.xyz))
			{
				return _OutLineColor;
			}
			else if (normal_test(normalsy1.xyz, normals.xyz))
			{
				return _OutLineColor;
			}

			if (_OutLineOnly == 1) {
				return half4(1, 1, 1, 1);
			}
			else {
				return Colour * abs(_OutLineOnly - 1);
			}

		}

		ENDCG
	}
	}
}
