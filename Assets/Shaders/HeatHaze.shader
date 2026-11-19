// MiniSlayers - efecto calorímo (heat haze) para el fondo de lava.
// Distorsion ondulada, mas fuerte cuanto mas abajo (donde estan los personajes).
// Uso: crear un Material con este shader y asignarlo al SpriteRenderer del Fondo.
Shader "MiniSlayers/HeatHaze"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        [Header(Distorsion)]
        _Strength ("Fuerza", Range(0, 0.05)) = 0.012
        _Speed ("Velocidad", Range(0, 10)) = 2.5
        _Frequency ("Frecuencia", Range(0, 60)) = 18
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "CanUseSpriteAtlas" = "True"
            "PreviewType" = "Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            float _Strength;
            float _Speed;
            float _Frequency;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata_t IN)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(IN.vertex);
                o.uv = IN.uv;
                o.color = IN.color * _Color;
                return o;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // bajo = 0 arriba, 1 abajo: la distorsion crece hacia la lava
                float bajo = 1.0 - IN.uv.y;
                float peso = _Strength * bajo * bajo;

                // ondas horizontales y una vertical suave, animadas con el tiempo
                float ondaX = sin(IN.uv.y * _Frequency + _Time.y * _Speed);
                float ondaY = cos(IN.uv.x * _Frequency * 0.5 + _Time.y * _Speed * 0.8);

                float2 uv = IN.uv + float2(ondaX * peso, ondaY * peso * 0.5);

                fixed4 c = tex2D(_MainTex, uv) * IN.color;
                c.rgb *= c.a;
                return c;
            }
            ENDCG
        }
    }
}
