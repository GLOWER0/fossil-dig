Shader "Custom/DirtShader"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.4, 0.25, 0.1, 1)
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            fixed4 _BaseColor;
            sampler2D _MaskTex;
            float _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed mask = tex2D(_MaskTex, i.uv).r;
                clip(mask - _Cutoff);
                return _BaseColor;
            }
            ENDCG
        }
    }
}