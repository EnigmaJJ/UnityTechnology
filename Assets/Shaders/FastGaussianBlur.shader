/******************************************************************************
 * DESCRIPTION: 优化的高斯模糊效果
 * 
 *     Copyright (c) 2018, 谭伟俊 (TanWeijun)
 *     All rights reserved
 * 
 * COMPANY:
 * CREATED: 2018.06.22, 19:07, CST
*******************************************************************************/

Shader "Unity Technology/Fast Gaussian Blur"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}
    }
    
    CGINCLUDE
    
        #include "UnityCG.cginc"
    
        struct appdata
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct v2f_5tap
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 blurUV : TEXCOORD1;
        };
        
        struct v2f_9tap
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 blurUV[2] : TEXCOORD1;
        };
        
        struct v2f_13tap
        {
            float4 vertex : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 blurUV[3] : TEXCOORD1;
        };
        
        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;
        
        float _BlurRadius;
    
        //
        // Small kernel
        //
        v2f_5tap vert5Horizontal (appdata v)
        {
            v2f_5tap o;
            
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            
            float2 offset = float2(_MainTex_TexelSize.x * _BlurRadius * 1.33333333, 0.0);
            o.blurUV.xy = o.uv + offset;
            o.blurUV.zw = o.uv - offset;
            
            return o;
        }
        
        v2f_5tap vert5Vertical (appdata v)
        {
            v2f_5tap o;
            
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            
            float2 offset = float2(0.0, _MainTex_TexelSize.x * _BlurRadius * 1.33333333);
            o.blurUV.xy = o.uv + offset;
            o.blurUV.zw = o.uv - offset;
            
            return o;
        }
        
        fixed4 frag5Blur (v2f_5tap i) : SV_Target
        {
        #if GAMMA_CORRECTION
            fixed3 sum = GammaToLinearSpace(tex2D(_MainTex, i.uv).rgb) * 0.29411764;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV.xy).rgb) * 0.35294117;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV.zw).rgb) * 0.35294117;
            return fixed4(LinearToGammaSpace(sum), 1.0);
        #else
            fixed3 sum = tex2D(_MainTex, i.uv).rgb * 0.29411764;
            sum += tex2D(_MainTex, i.blurUV.xy).rgb * 0.35294117;
            sum += tex2D(_MainTex, i.blurUV.zw).rgb * 0.35294117;
            return fixed4(sum, 1.0);
        #endif
        }
        
        //
        // Medium kernel
        //
        v2f_9tap vert9Horizontal (appdata v)
        {
            v2f_9tap o;
            
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            
            float2 offset_1 = float2(_MainTex_TexelSize.x * _BlurRadius * 1.38461538, 0.0);
            float2 offset_2 = float2(_MainTex_TexelSize.x * _BlurRadius * 3.23076923, 0.0);
            o.blurUV[0].xy = o.uv + offset_1;
            o.blurUV[0].zw = o.uv - offset_1;
            o.blurUV[1].xy = o.uv + offset_2;
            o.blurUV[1].zw = o.uv - offset_2;
            
            return o;
        }
        
        v2f_9tap vert9Vertical (appdata v)
        {
            v2f_9tap o;
            
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            
            float2 offset_1 = float2(0.0, _MainTex_TexelSize.y * _BlurRadius * 1.38461538);
            float2 offset_2 = float2(0.0, _MainTex_TexelSize.y * _BlurRadius * 3.23076923);
            o.blurUV[0].xy = o.uv + offset_1;
            o.blurUV[0].zw = o.uv - offset_1;
            o.blurUV[1].xy = o.uv + offset_2;
            o.blurUV[1].zw = o.uv - offset_2;
            
            return o;
        }
        
        fixed4 frag9Blur (v2f_9tap i) : SV_Target
        {
        #if GAMMA_CORRECTION
            fixed3 sum = GammaToLinearSpace(tex2D(_MainTex, i.uv).rgb) * 0.22702702;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV[0].xy).rgb) * 0.31621621;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV[0].zw).rgb) * 0.31621621;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV[1].xy).rgb) * 0.07027027;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV[1].zw).rgb) * 0.07027027;
            return fixed4(LinearToGammaSpace(sum), 1.0);
        #else
            fixed3 sum = tex2D(_MainTex, i.uv).rgb * 0.22702702;
            sum += tex2D(_MainTex, i.blurUV[0].xy).rgb * 0.31621621;
            sum += tex2D(_MainTex, i.blurUV[0].zw).rgb * 0.31621621;
            sum += tex2D(_MainTex, i.blurUV[1].xy).rgb * 0.07027027;
            sum += tex2D(_MainTex, i.blurUV[1].zw).rgb * 0.07027027;
            return fixed4(sum, 1.0);
        #endif
        }
        
        //
        // Big kernel
        //
        v2f_13tap vert13Horizontal (appdata v)
        {
            v2f_13tap o;
            
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            
            float2 offset_1 = float2(_MainTex_TexelSize.x * _BlurRadius * 1.41176470, 0.0);
            float2 offset_2 = float2(_MainTex_TexelSize.x * _BlurRadius * 3.29411764, 0.0);
            float2 offset_3 = float2(_MainTex_TexelSize.x * _BlurRadius * 5.17647058, 0.0);
            o.blurUV[0].xy = o.uv + offset_1;
            o.blurUV[0].zw = o.uv - offset_1;
            o.blurUV[1].xy = o.uv + offset_2;
            o.blurUV[1].zw = o.uv - offset_2;
            o.blurUV[2].xy = o.uv + offset_3;
            o.blurUV[2].zw = o.uv - offset_3;
            
            return o;
        }
        
        v2f_13tap vert13Vertical (appdata v)
        {
            v2f_13tap o;
            
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            
            float2 offset_1 = float2(0.0, _MainTex_TexelSize.x * _BlurRadius * 1.41176470);
            float2 offset_2 = float2(0.0, _MainTex_TexelSize.x * _BlurRadius * 3.29411764);
            float2 offset_3 = float2(0.0, _MainTex_TexelSize.x * _BlurRadius * 5.17647058);
            o.blurUV[0].xy = o.uv + offset_1;
            o.blurUV[0].zw = o.uv - offset_1;
            o.blurUV[1].xy = o.uv + offset_2;
            o.blurUV[1].zw = o.uv - offset_2;
            o.blurUV[2].xy = o.uv + offset_3;
            o.blurUV[2].zw = o.uv - offset_3;
            
            return o;
        }
        
        fixed4 frag13Blur (v2f_13tap i) : SV_Target
        {
        #if GAMMA_CORRECTION
            fixed3 sum = GammaToLinearSpace(tex2D(_MainTex, i.uv).rgb) * 0.19648255;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV[0].xy).rgb) * 0.29690696;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV[0].zw).rgb) * 0.29690696;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV[1].xy).rgb) * 0.09447039;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV[1].zw).rgb) * 0.09447039;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV[2].xy).rgb) * 0.01038136;
            sum += GammaToLinearSpace(tex2D(_MainTex, i.blurUV[2].zw).rgb) * 0.01038136;
            return fixed4(LinearToGammaSpace(sum), 1.0);
        #else
            fixed3 sum = tex2D(_MainTex, i.uv).rgb * 0.19648255;
            sum += tex2D(_MainTex, i.blurUV[0].xy).rgb * 0.29690696;
            sum += tex2D(_MainTex, i.blurUV[0].zw).rgb * 0.29690696;
            sum += tex2D(_MainTex, i.blurUV[1].xy).rgb * 0.09447039;
            sum += tex2D(_MainTex, i.blurUV[1].zw).rgb * 0.09447039;
            sum += tex2D(_MainTex, i.blurUV[2].xy).rgb * 0.01038136;
            sum += tex2D(_MainTex, i.blurUV[2].zw).rgb * 0.01038136;
            return fixed4(sum, 1.0);
        #endif
        }
        
    ENDCG
    
    SubShader
    {
        ZTest Always
        Cull Off
        ZWrite Off
        
        //
        // 5 tap gaussian blur
        //
        Pass
        {
            CGPROGRAM
                #pragma multi_compile _ GAMMA_CORRECTION
                #pragma vertex vert5Horizontal
                #pragma fragment frag5Blur
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma multi_compile _ GAMMA_CORRECTION
                #pragma vertex vert5Vertical
                #pragma fragment frag5Blur
            ENDCG
        }
        
        //
        // 9 tap gaussian blur
        //
        Pass
        {
            CGPROGRAM
                #pragma multi_compile _ GAMMA_CORRECTION
                #pragma vertex vert9Horizontal
                #pragma fragment frag9Blur
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma multi_compile _ GAMMA_CORRECTION
                #pragma vertex vert9Vertical
                #pragma fragment frag9Blur
            ENDCG
        }
        
        //
        // 13 tap gaussian blur
        //
        Pass
        {
            CGPROGRAM
                #pragma multi_compile _ GAMMA_CORRECTION
                #pragma vertex vert13Horizontal
                #pragma fragment frag13Blur
            ENDCG
        }

        Pass
        {
            CGPROGRAM
                #pragma multi_compile _ GAMMA_CORRECTION
                #pragma vertex vert13Vertical
                #pragma fragment frag13Blur
            ENDCG
        }
    }
}
