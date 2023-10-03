Shader "Custom/Portal"
{
    Properties
    {
        _LeftTex ("Texture", 2D) = "white" {}
        _RightTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

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
                float4 vertex : SV_POSITION;
            };

            sampler2D _LeftTex, _RightTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // sample the texture
                const float2 uv = i.screenPos.xy / i.screenPos.w;
                fixed4 col = tex2D(_LeftTex, uv) * ((unity_StereoEyeIndex + 1) % 2) + tex2D(_RightTex, uv) *
                    unity_StereoEyeIndex;
                return col;
            }
            ENDCG
        }
    }
}