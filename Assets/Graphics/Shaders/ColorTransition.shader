Shader "Custom/ColorTransition"
{
    Properties
    {
        _MainTex ("Albedo", 2D) = "white" {}
		_TransTex("Transition", 2D) = "white" {}

		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0

		_Hertz("Frequency (Hertz)", Range(0.01, 1)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _TransTex; 

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;

		half _Hertz; 

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			float2 transUV = float2(fmod(_Time.y, 1/_Hertz) * _Hertz, 0);
			fixed4 c = tex2D(_TransTex, transUV) * tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
