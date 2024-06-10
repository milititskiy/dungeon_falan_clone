Shader "Custom/ChessboardShader"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1, 1, 1, 1)
        _Color2 ("Color 2", Color) = (0, 0, 0, 1)
        _TileSize ("Tile Size", Float) = 1.0
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
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Color1;
            float4 _Color2;
            float _TileSize;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _TileSize;
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float2 coord = floor(i.uv);
                float check = fmod(coord.x + coord.y, 2.0);
                return check < 1.0 ? _Color1 : _Color2;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
