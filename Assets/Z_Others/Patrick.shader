Shader "Mobile/AIR_Standart" {
    Properties{
        [PowerSlider(1)] _Glossness("Glossness", Range(0, 1)) = 0.078125
        [PowerSlider(1)] _Specularess("Specularess", Range(0, 1)) = 0.078125

        _MainTex("Albedo", 2D) = "white" {}

        [Space][Toggle] _IsEmmision("Enable Emmision", Float) = 0
            _Emmision("Emmision", 2D) = "emmi" {}
        [Space][Toggle] _IsSpecgloss("Enable Specular", Float) = 0
            _Specular("Specular", 2D) = "spec" {}
            _Gloss("Gloss", 2D) = "gloss" {}
        [Space][Toggle] _IsNormal("Enable Normal", Float) = 0
            [NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}
        [Space][Toggle] _UseCurve("Enable Curvaturation", Float) = 0
            _CurvatureDirection("Curvature Direction", Vector) = (0,0,0,0)
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            LOD 250

        CGPROGRAM
        #pragma surface surf MobileBlinnPhong exclude_path:prepass nolightmap noforwardadd halfasview interpolview vertex:vert


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
            fixed diff = max(0, dot(s.Normal, lightDir));
            fixed nh = max(0, dot(s.Normal, halfDir));
            fixed3 spec = pow(nh, s.Specular * 128) * s.Gloss;

            fixed4 c;
            c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * atten;
            UNITY_OPAQUE_ALPHA(c.a);
            return c;
        }

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _Emmision;
        sampler2D _Specular;
        sampler2D _Gloss;
        half _Glossness;
        half _Specularess;
        float _IsEmmision;
        float _IsSpecgloss;
        float _IsNormal;
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
            fixed4 spec = tex2D(_Specular, IN.uv_Specular);
            fixed4 gloss = tex2D(_Gloss, IN.uv_Gloss);
            o.Albedo = tex.rgb;
            o.Gloss = spec.a * _Glossness;
            o.Alpha = tex.a;
            if (_IsSpecgloss == 1) o.Specular = (spec.r + spec.g + spec.b) / 3 * _Specularess;
            if(_IsEmmision == 1) o.Emission = tex2D(_Emmision, IN.uv_MainTex).rgb;
            if(_IsNormal == 1) o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
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