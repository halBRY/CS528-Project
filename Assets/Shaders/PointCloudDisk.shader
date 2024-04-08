Shader "PointCloud/DiskPoint" {
	Properties{
        _ParsecScaleFactor("Parsec Scale Factor", Float) = 0
        _frameNumber("Frame Number", Float) = 0
		_isSpect("Color by Spectral Type", float) = 1
		_Brightness("Brightness", float) = 1
	}
	SubShader{
		LOD 200
		Tags{ "RenderType" = "Opaque" }

		Pass{
			CGPROGRAM
			#pragma require geometry
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

			#pragma target 3.0                  
			#include "UnityCG.cginc"

			// Attributes
			struct vertexIn {
				float4 pos : POSITION;
				float4 color : COLOR;
				float4 normal : NORMAL; // holding velocity
				float4 uv : TEXCOORD0; // holding RGB of exoColor, and radius
			};

			// Varyings
			struct vertexOut {
				float4 pos : SV_POSITION;
				float4 color : COLOR0;
				float3 normal : NORMAL;
				float2 data : TEXCOORD0;  // holds distance and radius
				float4 posWorld : TEXCOORD1;
			};

			struct geomOut {
				float4 pos : POSITION;
				float4 color : COLOR0;
				float3 normal : NORMAL;
				float4 posWorld : TEXCOORD1;
			};

			// Uniforms
			float _ParsecScaleFactor;
			float _frameNumber;
			float _isSpect;

			vertexOut vert(vertexIn inputObj, uint vertexID : SV_VertexID) {
				// Define output
				vertexOut outputObj;

				// Position calculations
				float3 scaledPos = inputObj.pos * _ParsecScaleFactor; // Scale from meters to feet
				float3 velocityPos = scaledPos + (inputObj.normal * _frameNumber); // Add velocity based offset depending on animation step number. 

				// Pass vertex position
				outputObj.posWorld = float4(velocityPos, 0);
				outputObj.pos = UnityObjectToClipPos(velocityPos); // in screen space

				float3 exoColor = inputObj.uv.xyz;

				// Color passthrough
				if(_isSpect == 1)
				{
					outputObj.color = inputObj.color;
				}
				else
				{
					outputObj.color = float4(exoColor, 1);
				}


				// Calculate new normal
				outputObj.normal = ObjSpaceViewDir(outputObj.pos);

				// Pass distance and radius for geom calculations
				outputObj.data.x = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, inputObj.pos));
				outputObj.data.y = inputObj.uv.w; //Get radius

				return outputObj;
			}

			float _Radius;
			float _ScaleFactor;
			float _NumPolys;
			float _Rotation;
			[maxvertexcount(48)]
			void geom(point vertexOut inputObj[1], inout TriangleStream<geomOut> OutputStream)
			{
				_Radius = inputObj[0].data.y;
				_ScaleFactor = 1;
				_NumPolys = 14;

				int nTriangles = floor(_NumPolys);

				float dist = inputObj[0].data.x;

				dist = (_ScaleFactor + (1 - _ScaleFactor) * dist) / 2.0;

				geomOut outputObj;
				outputObj.posWorld = inputObj[0].posWorld;
				outputObj.color = inputObj[0].color;
				outputObj.normal = inputObj[0].normal;

				float2 p[3];
				p[0] = float2(0, 0);
				float xyscale = _ScreenParams.y / _ScreenParams.x;
				float angleOffset = _Rotation / 180 * 3.14159;

				p[2] = float2(_Radius* xyscale*cos(angleOffset), _Radius*sin(angleOffset));
				float twopion = 2.0 * 3.14159 / nTriangles;
				for (int it = 1; it <= nTriangles; it++) {
					float theta = (float(it)) * twopion + angleOffset;

					if (it == nTriangles) {
						p[1] = float2(_Radius * cos(theta) * xyscale, _Radius * sin(theta));
					}
					else {
						p[1] = p[2];
						p[2] = float2(_Radius * cos(theta) * xyscale, _Radius * sin(theta));
					}

					for (int i = 0; i < 3; i++) {
						outputObj.pos = inputObj[0].pos + float4(p[i], 0, 0) * 0.5;// * dist;
						OutputStream.Append(outputObj);
					}

				}

			}

			float _Brightness;
			float4 frag(geomOut inputObj) : COLOR
			{
				_Brightness = 0.2;
				// A greater distance means a darker color
				float4 tintColor = float4(_Brightness,_Brightness,_Brightness, 1);
				float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld,inputObj.posWorld));

				float4 newColor = inputObj.color * tintColor;

				return lerp(inputObj.color, newColor, (dist/1500));
			}
			ENDCG
		}
	}
		FallBack "Diffuse"
}