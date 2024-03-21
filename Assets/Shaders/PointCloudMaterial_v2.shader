Shader "PointCloud/PointCloudMaterial_v2"
{
    Properties
    {
        _scaleFactor("Scale Factor", Float) = 0
        _frameNumber("Frame Number", Float) = 0
    }
    Subshader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vertex_shader
            #pragma fragment fragment_shader
            #pragma geometry geometry_shader
            #pragma target 5.0

            #include "UnityCG.cginc"

            //Attributes
            //Life changing thread 
            //https://forum.unity.com/threads/cg-hlsl-sematics-for-custom-values.579499/
            struct attributes
            {
                float4 position: POSITION;
                float4 VaC: TEXCOORD0; // Velocity and ColorID. VaC.xyz = velocity.xyz. VaC.w = colorID
            };

            //Varyings
            struct varyings
            {
              	float4 pos : SV_POSITION;
				float4 color : COLOR0;
				float3 normal : NORMAL;
				float d : TEXCOORD0; // Not actually a texcoord, just moving data
				float4 posWorld : TEXCOORD1;  
            };
 
            //Uniforms
            uniform StructuredBuffer<attributes> data; // : register(t1);
            float _scaleFactor;
            float _frameNumber;

            varyings vertex_shader(attributes input, uint vertexID : SV_VertexID)
            {
                varyings output;

                float colorID = data[vertexID].VaC.w;
                float4 velocity = float4(data[vertexID].VaC.xyz, 0);

                output.posWorld = data[vertexID].position + (_frameNumber * velocity);

                output.pos = UnityObjectToClipPos(data[vertexID].position);

                //Later, set up a check to match color ID with actual color value and radius. 
				output.color = float4(colorID/10, 0,0,0);
				output.normal = ObjSpaceViewDir(output.pos);
				output.d = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, data[vertexID].position));

                return output;
            }

            // Geometry phase
            float _Radius = 1.0;
            [maxvertexcount(36)]
            void geometry_shader(point varyings input[1], inout TriangleStream<varyings> outStream)
            {
                float4 origin = input[0].pos;
                float2 extent = abs(UNITY_MATRIX_P._11_22 * _Radius);

                // Copy the basic information.
                varyings output = input[0];

                // Determine the number of slices based on the radius of the
                // point on the screen.
                float radius = extent.y / origin.w * _ScreenParams.y;
                uint slices = min((radius + 1) / 5, 4) + 2;

                // Slightly enlarge quad points to compensate area reduction.
                // Hopefully this line would be complied without branch.
                if (slices == 2) extent *= 1.2;

                // Top vertex
                output.pos.y = origin.y + extent.y;
                output.pos.xzw = origin.xzw;
                outStream.Append(output);

                UNITY_LOOP for (uint i = 1; i < slices; i++)
                {
                    float sn, cs;
                    sincos(UNITY_PI / slices * i, sn, cs);

                    // Right side vertex
                    output.pos.xy = origin.xy + extent * float2(sn, cs);
                    outStream.Append(output);

                    // Left side vertex
                    output.pos.x = origin.x - extent.x * sn;
                    outStream.Append(output);
                }

                // Bottom vertex
                output.pos.x = origin.x;
                output.pos.y = origin.y - extent.y;
                outStream.Append(output);

                //outStream.RestartStrip();
            }
 
            float4 fragment_shader(varyings input) : COLOR
            {
                return input.color;
            }
 
            ENDCG
        }
    }
}