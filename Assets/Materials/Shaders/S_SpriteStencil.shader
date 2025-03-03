Shader "Custom/SpriteStencil"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [IntRange] _StencilID ("Stencil ID", Range(0, 255)) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent"
        }

        Pass
        {
            Stencil
            {
                Ref [_StencilID]
                Comp Equal
            }

            // Alpha blending
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            fixed4 _Color;

			struct appdata
			{
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
				float4 color : COLOR;     
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f output;
				output.pos = UnityObjectToClipPos(v.vertex);
				output.uv = v.uv;
				output.color = v.color;
				return output;
			}
			
			float4 frag(v2f i) : COLOR
			{
				float4 texcolor = tex2D(_MainTex, i.uv);    // color from texture pixel
				float4 vertexcolor = i.color;               // color from UnityEngine.UI.Image.Color
                return texcolor * vertexcolor;
			}

            ENDCG
        }
    }
}
