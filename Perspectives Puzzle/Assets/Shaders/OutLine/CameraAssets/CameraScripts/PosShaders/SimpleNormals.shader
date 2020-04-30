
Shader "Jaytt/PostRendering/OutLine/Simple Normals"

{

	Properties
	{
		[HideInInspector] _MainTex("Texture", 2D) = "white" {}
		_Dx("Thickness", range(0.0, 10)) = 1
	    _Angle("Max Angle", range(0.0, 0.3)) = 0.1
		_DepthOL("Depth outline", range(0, 0.003)) = 0.003
		_OutLineOnly("OutLine Only", range(0.0, 1.0)) = 1.0
		_OutLineColor("Outline Color", Color) = (0,0,0,1)
		_isMobile("Mobile?", range(0,1)) = 1

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
		half _DepthOL;
		half _OutLineOnly;
		half _Angle;
		half _Dx;
		half _isMobile;
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

	fixed4 frag(v2f i) : SV_Target
	{
		//base textures
		half4 normals = getNormal(i.screenuv) ;
		half4 Colour = getColor(i.screenuv);

		fixed deltaDepth ;

		half2 pixels = half2(_Dx / _ScreenParams.x, _Dx / _ScreenParams.y);
		half4 dx = half4(pixels.x+ i.screenuv.x, i.screenuv.y, i.screenuv.z, i.screenuv.w);
		half4 dy = half4(i.screenuv.x, pixels.y + i.screenuv.y,  i.screenuv.z, i.screenuv.w);
		half4 dx1 = half4(-pixels.x + i.screenuv.x, i.screenuv.y, i.screenuv.z, i.screenuv.w);
		half4 dy1 = half4(i.screenuv.x, -pixels.y + i.screenuv.y,  i.screenuv.z, i.screenuv.w);

		

		half4 normalsx = getNormal(dx);
		half4 normalsy = getNormal(dy);
		half4 normalsx1 = getNormal(dx1);
		half4 normalsy1 = getNormal(dy1);

		if (_DepthOL !=0.003 && _isMobile < 1) {
			half Depth = getDepth(i.screenuv);

			deltaDepth = abs(getDepth(dx) - Depth)+
						 abs(getDepth(dy) - Depth)+
						 abs(getDepth(dx1) - Depth)+
						 abs(getDepth(dy1) - Depth);
	
			if (deltaDepth > _DepthOL) {
				return _OutLineColor;
			}

			if (normals.z < 0.5 && (normalsx.z >0.5 || normalsy.z >0.5 || normalsx1.z >0.5 || normalsy1.z >0.5)) {
				return _OutLineColor;
			}

		}

		fixed Angle = 1;

		if (abs(normalsx.x - normals.x) > _Angle || abs(normalsx.y - normals.y) > _Angle || abs(normalsx.z - normals.z) > _Angle) {
			return _OutLineColor;
		}
		else if (abs(normalsx1.x - normals.x) >_Angle || abs(normalsx1.y - normals.y) > _Angle || abs(normalsx1.z - normals.z) > _Angle) {
			return _OutLineColor;
		}
		else if (abs(normalsy.x - normals.x) > _Angle || abs(normalsy.y - normals.y) > _Angle || abs(normalsy.z - normals.z) > _Angle) {
			return _OutLineColor;
		}
		else if (abs(normalsy1.x - normals.x) > _Angle || abs(normalsy1.y - normals.y) > _Angle || abs(normalsy1.z - normals.z) > _Angle) {
			return _OutLineColor;

		}
			
			if (_OutLineOnly == 1) {
				return half4(1, 1, 1, 1);
			}

			Colour = Colour * abs(_OutLineOnly - 1);

		return Colour;
	}

		ENDCG
	}
	}
}
