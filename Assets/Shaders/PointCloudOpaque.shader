Shader "PointCloud/SpherePointOpaque" {
	Properties{
		//_Radius("Sphere Radius", float) = 0.01
		_ScaleFactor("Scale Factor", Range(0,1)) = 1
		_NumPolys("Number of Polygons", Range(3,16)) = 16
		_DistFromSol("Distance from Sol", float) = 0
		_Brightness("Brightness", float) = 1
	}
	SubShader{
		LOD 200
		Tags{ "RenderType" = "Opaque" }

		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

			#pragma target 3.0                  
			#include "UnityCG.cginc"

			struct vertexIn {
				float4 pos : POSITION;
				float4 color : COLOR;
			};

			struct vertexOut {
				float4 pos : SV_POSITION;
				float4 color : COLOR0;
				float3 normal : NORMAL;
				float d : TEXCOORD0; // Not actually a texcoord, just moving data
				float4 posWorld : TEXCOORD1;
			};

			struct geomOut {
				float4 pos : POSITION;
				float4 color : COLOR0;
				float3 normal : NORMAL;
				float4 posWorld : TEXCOORD1;
			};


			vertexOut vert(vertexIn inputObj, uint vertexID : SV_VertexID) {
				vertexOut outputObj;
				outputObj.posWorld = inputObj.pos;
				outputObj.pos = UnityObjectToClipPos(inputObj.pos);
				outputObj.color = inputObj.color;
				outputObj.normal = ObjSpaceViewDir(outputObj.pos);
				outputObj.d = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, inputObj.pos));

				return outputObj;
			}

			float _Radius;
			float _ScaleFactor;
			float _NumPolys;
			float _Rotation;
			[maxvertexcount(48)]
			void geom(point vertexOut inputObj[1], inout TriangleStream<geomOut> OutputStream)
			{

				int nTriangles = floor(_NumPolys);

				float dist = inputObj[0].d;

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
						outputObj.pos = inputObj[0].pos + float4(p[i], 0, 0) * dist;
						OutputStream.Append(outputObj);
					}

				}

			}

			float _Brightness;
			float4 frag(geomOut inputObj) : COLOR
			{
				// A greater distance means a darker color
				float4 tintColor = float4(_Brightness,_Brightness,_Brightness, 1);
				float dist = distance(_WorldSpaceCameraPos, inputObj.posWorld);

				float4 newColor = inputObj.color * tintColor;

				return lerp(inputObj.color, newColor, (dist/400));
			}
			ENDCG
		}
	}
		FallBack "Diffuse"
}