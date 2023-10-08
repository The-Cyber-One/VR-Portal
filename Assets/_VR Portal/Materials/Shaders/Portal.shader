Shader "Custom/Portal"
{
    Properties
    {
        _LeftTex ("LeftEye", 2D) = "white" {}
        _RightTex ("RightEye", 2D) = "white" {}
        _BorderSize ("BorderSize", Range(0, 1)) = 0.1
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalRenderPipeline"
        }
        Cull Off
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
            };

            struct v2f
            {
                float4 screenPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
                float4 objectCoord : TEXTCOORD2;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO //Insert
            };

            sampler2D _LeftTex, _RightTex;
            float4 _LeftTex_ST;
            float _BorderSize;
            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.objectCoord = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _LeftTex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                const float2 uv = i.screenPos.xy / i.screenPos.w;
                const fixed4 portalScreen = tex2D(_LeftTex, uv) * ((unity_StereoEyeIndex + 1) % 2) + tex2D(
                        _RightTex, uv) *
                    unity_StereoEyeIndex;

                const float3 scale = float3(
                    length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)),
                    length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)),
                    length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))
                );
                const fixed2 border = abs(i.objectCoord) * scale > scale / 2 - _BorderSize;
                const fixed borderMask = border.x || border.y;

                return portalScreen * (1 - borderMask) + borderMask * _Color;
            }
            ENDHLSL
        }
    }
}