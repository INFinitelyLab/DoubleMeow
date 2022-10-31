Shader "Mobile/Squidward" {
    Properties{
        [PowerSlider(5.0)] _Shininess("Shininess", Range(0.03, 1)) = 0.078125
        _MainTex("Base (RGB) Gloss (A)", 2D) = "white" {}
        _Emission("Emission", 2D) = "black" {}
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 250

        CGPROGRAM
        #pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview interpolateview

        inline fixed4 LightingMobileBlinnPhong(SurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
        {
            fixed diff = max(0, dot(s.Normal, lightDir));
            fixed nh = max(0, dot(s.Normal, halfDir));
            fixed spec = pow(nh, s.Specular * 128) * s.Gloss;

            fixed4 c;
            c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
            c.rgb += s.Emission;
            UNITY_OPAQUE_ALPHA(c.a);
            return c;
        }

        sampler2D _MainTex;
        sampler2D _Emission;
        half _Shininess;

        struct Input {
            float2 uv_MainTex;
            float2 uv_Emission;
        };

        void surf(Input IN, inout SurfaceOutput o) {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 emi = tex2D(_Emission, IN.uv_Emission);
            o.Albedo = tex.rgb;
            o.Gloss = tex.a;
            o.Alpha = tex.a;
            o.Emission = emi.rgb;
            o.Specular = _Shininess;
        }
        ENDCG
    }

        FallBack "Mobile/VertexLit"
}