Shader "MansionMap/FogOfWar"
{
    // Blends a greyscale fog mask over the map background.
    // White pixels in _FogTex = visible, Black = hidden (dark overlay).
    // Assign this shader to a Material, then set that Material on the FogOverlay RawImage.

    Properties
    {
        _MainTex  ("Map Texture (unused by RawImage, kept for compatibility)", 2D) = "white" {}
        _FogTex   ("Fog Mask (RFloat RenderTexture)", 2D) = "black" {}
        _FogColor ("Fog Color", Color) = (0.08, 0.06, 0.03, 0.88)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _FogTex;
            fixed4    _FogColor;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f    { float4 pos : SV_POSITION;  float2 uv : TEXCOORD0; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // revealed = 1 → fully transparent (show map)
                // revealed = 0 → fully opaque fog
                float revealed = tex2D(_FogTex, i.uv).r;
                float fogAlpha = _FogColor.a * (1.0 - revealed);
                return fixed4(_FogColor.rgb, fogAlpha);
            }
            ENDCG
        }
    }
}
