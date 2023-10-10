Shader "Custom/Portal"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)

        [Header(Border)]
        [Space]
        _BorderSize ("Border size", Range(0, 1)) = 0.1
        _BorderSmoothing ("Border smoothing", Range(0,1)) = 0.5

        [Header(Squares)]
        [Space]
        _SquareSize ("Square size", Float) = 0.1
        _SquareBorder ("Square border size", Range(0, 1)) = 0.01
        _SquareSpeed ("Square speed", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100
        Cull Off

        Pass // Portal
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            // #include "FastNoiseLite.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                // UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                float4 objectPosition : TEXCOORD1;
                // UNITY_VERTEX_OUTPUT_STEREO
            };

            // UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
            sampler2D _LeftTex, _RightTex;
            float4 _Color;
            float _BorderSize, _BorderSmoothing;
            float _SquareSize, _SquareBorder, _SquareSpeed;

            float3 inv_lerp(const float3 from, const float3 to, const float3 value)
            {
                return (value - from) / (to - from);
            }

            v2f vert(appdata v)
            {
                v2f o;

                // UNITY_SETUP_INSTANCE_ID(v);
                // UNITY_INITIALIZE_OUTPUT(v2f, o);
                // UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.objectPosition = v.vertex;
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                // return lerp(float4(1, 0, 0, 1), float4(0, 1, 0, 1), unity_StereoEyeIndex);
                // Portal cams
                float2 uv = i.screenPos.xy / i.screenPos.w;
                const fixed3 portalScreen = lerp(tex2D(_LeftTex, uv), tex2D(_RightTex, uv), unity_StereoEyeIndex);
                // return fixed4(portalScreen, 1);

                // Border
                const float3 scale = float3(
                    length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)),
                    length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)),
                    length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))
                );

                const float3 objectInWorld = i.objectPosition * scale;
                const float verticalGradient = 1 - (objectInWorld.y + scale.y / 2) / scale.y;
                fixed3 borderColor = _Color * verticalGradient;
                // return fixed4(borderColor, 1);

                float2 border = inv_lerp(scale / 2 - _BorderSize, scale / 2, abs(objectInWorld));
                border = clamp(border, 0, 1);
                const float borderGradient = clamp(inv_lerp(0, _BorderSmoothing, border.x + border.y), 0, 1);
                // return borderGradient;

                // Squares
                const float2 squareOffset = objectInWorld + _SquareSize / 2;
                fixed2 squareBorder2D = abs(squareOffset % _SquareSize);
                squareBorder2D = lerp(_SquareSize - squareBorder2D, squareBorder2D, squareOffset > 0);
                squareBorder2D = squareBorder2D < _SquareBorder;
                const bool squareMask = !(squareBorder2D.x || squareBorder2D.y);
                // return squareMask;
                // Rotation
                fixed2 squares2D = floor(squareOffset / _SquareSize);
                const float theta = atan2(squares2D.y, squares2D.x);
                const float angle = frac(theta * 2 / UNITY_TWO_PI - _Time.x * _SquareSpeed);
                const float squares = angle * squareMask;
                // Clamp here to get rid of center pixel that has a weired value
                borderColor = clamp(borderColor * squares, 0, 1);
                // return squares;

                // Create and configure noise state
                // fnl_state noise = fnlCreateState();
                // noise.noise_type = FNL_NOISE_OPENSIMPLEX2;
                // float2 noiseValue = fnlGetNoise2D(noise, i.screenPos.x * 500, i.screenPos.y * 500) * (1 / length(
                //     i.screenPos.xy) * 0.07);

                fixed4 outColor = fixed4(lerp(portalScreen, borderColor, borderGradient), 1);
                return outColor;
            }
            ENDCG
        }
    }
}