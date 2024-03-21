Shader "Custom/LineShader"
{
    Properties
    {
        _ParsecScaleFactor("Parsec Scale Factor", Float) = 0
        _frameNumber("Frame Number", Float) = 0
        _LineColor ("Line Color", Color) = (0,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            // Attributes
			struct vertexIn {
				float4 pos : POSITION;
				float4 normal : NORMAL; // holding velocity
			};

            // Varyings
			struct vertexOut {
				float4 pos : POSITION;
			};

            // Uniforms
            float4 _LineColor;
            float _ParsecScaleFactor;
			float _frameNumber;

            vertexOut vert (vertexIn input)
            {
                vertexOut output;

                // Position calculations
				float3 scaledPos = input.pos * _ParsecScaleFactor; // Scale from meters to feet
				float3 velocityPos = scaledPos + (input.normal * _frameNumber); // Add velocity based offset depending on animation step number. 

                output.pos = UnityObjectToClipPos(velocityPos);

                return output;
            }

            float4 frag (vertexOut input) : COLOR
            {
                return _LineColor;
            }
            ENDCG
        }
    }
}
