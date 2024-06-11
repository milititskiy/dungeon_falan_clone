Shader "Custom/SelectionBoxShader"
{
    Properties
    {
        _Color("Main Color", Color) = (1,1,1,1)
        _GlowColor("Glow Color", Color) = (1,1,0,1)
        _GlowStrength("Glow Strength", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _GlowColor;
            float _GlowStrength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;

                float edgeThickness = 0.1; // Thickness of the glowing frame
                float glow = max(smoothstep(0.5 - edgeThickness, 0.5, abs(i.uv.x - 0.5)),
                                 smoothstep(0.5 - edgeThickness, 0.5, abs(i.uv.y - 0.5)));
                col.rgb = lerp(col.rgb, _GlowColor.rgb, glow * _GlowStrength);

                return col;
            }
            ENDCG
        }
    }
}
