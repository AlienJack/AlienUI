Shader "Custom/UI/Fast Gaussian Blur(Optimized)"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0

        _BlurSize("Blur Size", Range(0,0.5)) = 0
        _Interval("Gaussian Interval", Float) = 0.1
		[KeywordEnum(Low, Medium, High)] _Samples ("Sample amount", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

		Pass
		{
            Name "BlurH"
			CGPROGRAM
			#include "UnityCG.cginc"
            #include "UnityUI.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

			sampler2D _MainTex;
            fixed4 _Color;
			float _BlurSize;
			float _StandardDeviation;
            float _Interval;


			#define PI 3.14159265359
			#define E 2.71828182846

			#if _SAMPLES_LOW
			#define SAMPLES 2
			#elif _SAMPLES_MEDIUM
			#define SAMPLES 3
			#else
			#define SAMPLES 5
			#endif

            #define _StandardDeviation 0.3f
			#define stDevSquared _StandardDeviation*_StandardDeviation
			#define gauseV1 0.5f/(PI*stDevSquared)
			#define gauseV2 -0.5f/stDevSquared

			struct appdata{
				float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f{
				float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float4 texcoord  : TEXCOORD0;
                float4 blurGassian : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v){
				v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                float4 vPosition = UnityObjectToClipPos(v.vertex);
                OUT.vertex = vPosition;
				OUT.texcoord = float4(v.texcoord, 0, gauseV1 * pow(E, gauseV2 * 16 * _Interval * _Interval));
                OUT.color = v.color * _Color;
                OUT.blurGassian = float4(gauseV1,
                    gauseV1 * pow(E, gauseV2 * _Interval * _Interval),
                    gauseV1 * pow(E, gauseV2 * 4 * _Interval * _Interval),
                    gauseV1 * pow(E, gauseV2 * 9 * _Interval * _Interval));
                return OUT;
			}

			fixed4 frag(v2f i) : SV_TARGET{
				float4 col = 0;
				float sum = 0;
                float gassianVal[5] = {
                    i.blurGassian.x, 
                    i.blurGassian.y, 
                    i.blurGassian.z, 
                    i.blurGassian.w, 
                    i.texcoord.w
                };

				for(int index = 0; index < SAMPLES; index++)
				{
                    float indexPlus0P5 = index + 0.5f;
					float2 offset = float2((indexPlus0P5 / SAMPLES) * _BlurSize, 0);
					float gauss = gassianVal[index];
                    float2 uv1 = i.texcoord + offset;
                    float2 uv2 = i.texcoord - offset;
					col += (tex2D(_MainTex, uv1) + tex2D(_MainTex, uv2)) * gauss;
                    sum += gauss * 2;
				}

				col = col / sum;
				return col;
			}

			ENDCG
		}

		GrabPass
        {
            "_GrabUI"
        }

		Pass
        {
			Name "BlurV"
            CGPROGRAM
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma multi_compile _SAMPLES_LOW _SAMPLES_MEDIUM _SAMPLES_HIGH
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

			float _BlurSize;
			float _StandardDeviation;
            float _Interval;

            sampler2D _GrabUI;

			#define PI 3.14159265359
			#define E 2.71828182846

			#if _SAMPLES_LOW
			#define SAMPLES 2
			#elif _SAMPLES_MEDIUM
			#define SAMPLES 3
			#else
			#define SAMPLES 5
			#endif

            #define _StandardDeviation 0.3f
			#define stDevSquared _StandardDeviation*_StandardDeviation
			#define gauseV1 0.5f/(PI*stDevSquared)
			#define gauseV2 -0.5f/stDevSquared

            struct v2f
            {
                float4 grabPos : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 blurGassian : TEXCOORD1;
                float2 blurGassian2 : TEXCOORD2;
            };

            v2f vert(appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.grabPos = ComputeGrabScreenPos(o.pos);
                o.blurGassian = float4(gauseV1,
                    gauseV1 * pow(E, gauseV2 * _Interval * _Interval),
                    gauseV1 * pow(E, gauseV2 * 4 * _Interval * _Interval),
                    gauseV1 * pow(E, gauseV2 * 9 * _Interval * _Interval));
                o.blurGassian2 = float2(gauseV1 * pow(E, gauseV2 * 16 * _Interval * _Interval), 0);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
				float4 col = 0;
				float sum = 0;
                float gassianVal[5] = {
                    i.blurGassian.x, 
                    i.blurGassian.y, 
                    i.blurGassian.z, 
                    i.blurGassian.w, 
                    i.blurGassian2.x
                };
                for(int index = 0; index < SAMPLES; index++)
				{
                    float indexPlus0P5 = index + 0.5f;
					float4 offset = float4(0, (indexPlus0P5 / SAMPLES) * _BlurSize, 0, 0);
					float gauss = gassianVal[index];
                    float4 uv1 = i.grabPos + offset;
                    float4 uv2 = i.grabPos - offset;
					col += (tex2Dproj(_GrabUI, uv1) + tex2Dproj(_GrabUI, uv2)) * gauss;
                    sum += gauss * 2;
				}
				col = col / sum;
				return col;
            }
            ENDCG
        }
    }
}