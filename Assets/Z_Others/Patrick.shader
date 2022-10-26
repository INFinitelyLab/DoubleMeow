Shader "Mobile/AIR_Standart" {
    Properties{
        [PowerSlider(1)] _Specularess("Specularess", Range(0, 1)) = 0
        [PowerSlider(1)] _Glossness("Glossness", Range(0, 1)) = 0

        _MainTex("Albedo", 2D) = "white" {}

        [Space][Toggle] _IsEmmision("Enable Emmision", Float) = 0
            _Emmision("Emmision", 2D) = "black" {}
        [Space][Toggle] _UseCurve("Enable Curvaturation", Float) = 0
            _CurvatureDirection("Curvature Direction", Vector) = (0,0,0,0)
    }
        SubShader{
            Tags{"RenderType" = "Opaque"}
            LOD 250

        CGPROGRAM
        #pragma surface surf MobileBlinnPhong exclude_path:deferred nolightmap noforwardadd halfasview vertex:vert

        struct INFSurfaceOutput
        {
            half3 Albedo;  // diffuse color
            half3 Normal;  // tangent space normal, if written
            half3 Emission;
            half Specular;  // specular power in 0..1 range
            half Gloss;    // specular intensity
            half Alpha;    // alpha for transparencies
        };


        inline fixed4 LightingMobileBlinnPhong(INFSurfaceOutput s, fixed3 lightDir, fixed3 halfDir, fixed atten)
        {
            fixed4 c;
            c.rgb = (s.Albedo * _LightColor0.rgb) * atten;
            UNITY_OPAQUE_ALPHA(c.a);
            return c;
        }

        sampler2D _MainTex;
        sampler2D _Emmision;
        half _Glossness;
        half _Specularess;
        float _IsEmmision;
        float _UseCurve;
        float _CurvatureIntensive = 0;
        uniform float4 _CurvatureDirection;

        struct Input {
            float2 uv_MainTex;
            float2 uv_Specular;
            float2 uv_Gloss;
        };

        void surf(Input IN, inout INFSurfaceOutput o) {
            fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = tex.rgb;
            o.Gloss = _Glossness;
            o.Alpha = tex.a;
            o.Specular = _Specularess;
            if(_IsEmmision == 1) o.Emission = tex2D(_Emmision, IN.uv_MainTex).rgb;
        }

        void vert(inout appdata_full v)
        {
            if (_UseCurve == 1)
            {
                float4 vv = mul(unity_ObjectToWorld, v.vertex);

                vv.xyz -= _WorldSpaceCameraPos.xyz;
                vv = float4((vv.z * vv.z) * -_CurvatureDirection.x * _CurvatureIntensive, (vv.z * vv.z) * -_CurvatureDirection.y * _CurvatureIntensive, 0.0f, 0.0f);
                v.vertex += mul(unity_WorldToObject, vv);
            }
        }

        ENDCG
    }

        FallBack "Mobile/VertexLit"
}