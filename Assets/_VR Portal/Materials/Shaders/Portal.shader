Shader "Custom/Portal"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _BorderSize ("Border size", Range(0, 1)) = 0.1
        _SquareSize ("Square size", Float) = 0.1
        _SquareBorder ("Square border size", Range(0, 1)) = 0.1
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 screenPos : TEXCOORD0;
                float4 objectCoord : TEXTCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _LeftTex, _RightTex;
            float4 _Color;

            float _BorderSize;
            float _SquareSize, _SquareBorder;

            float3 invLerp(float3 from, float3 to, float3 value)
            {
                return (value - from) / (to - from);
            }

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.objectCoord = v.vertex;
                o.screenPos = ComputeScreenPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Portal cams
                const float2 uv = i.screenPos.xy / i.screenPos.w;
                const fixed4 portalScreen = lerp(tex2D(_LeftTex, uv), tex2D(_RightTex, uv), unity_StereoEyeIndex);

                // Border
                const float3 scale = float3(
                    length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)),
                    length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)),
                    length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))
                );

                const float3 objectInWorld = i.objectCoord * scale;

                float2 border = invLerp(scale / 2 - _BorderSize, scale / 2, abs(objectInWorld));
                border = clamp(border, 0, 1);
                const bool borderMask = border.x > 0 || border.y > 0;
                const float borderGradient = border.x + border.y;
                const float gradient = 1 - (objectInWorld.y + scale.y / 2) / scale.y;
                const float4 borderColor = fixed4((borderGradient + _Time.y) % 2 * borderGradient * _Color) * gradient;

                // Squares
                // const float3 squareOffset = objectInWorld - _SquareSize / 2;
                // fixed3 squares3D = abs(squareOffset % _SquareSize);
                // squares3D = lerp(_SquareSize - squares3D, squares3D, squareOffset > 0);
                // squares3D = squares3D < _SquareBorder;
                // const fixed squares = squares3D.x || squares3D.y || squares3D.z;

                return lerp(portalScreen, borderColor, borderMask);
            }
            ENDCG
        }
    }
}