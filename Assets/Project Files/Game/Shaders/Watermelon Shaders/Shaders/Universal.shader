Shader "WMelon/Universal"
{
    Properties
    {
        [HideInInspector]_Transparent("Transparent", float) = 0

        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap ("Normal", 2D) = "bump" {}
        _LightColorOn("Uses Light Color", float) = 0
        _LightColorInfluence("LightColorinfluence", Range(0, 1)) = 1
        [Toggle(CUSTOM_LIGHT_DIRECTION_ON)] _CustomLightDirectionOn("Custom Light Direction Enabled", float) = 0
        _CustomLightDirection("Custom Light Direction", Vector) = (-0.5, 2, -0.5, 0)
        
        [KeywordEnum(None, Vertex, Pixel)] _Shadows("Receive Shadows", Float) = 2
        _SColor("Shadow Color", Color) = (0.5,0.5,0.5,1)

        [Toggle(EMISSION_ON)]_EmissionOn("Emission", float) = 0
        [HDR]_EmissionColor("Emission Color", Color) = (0,0,0,0)
        _EmissionTex("Emission Texture", 2D) = "white" {}

        [Toggle(RIM_ON)]_RimOn("Rim", float) = 0
        _RimColor("Rim Color", Color) = (1,1,1,1)
        _RimMin("Rim Min", range(0, 1)) = 0.5
        _RimMax("Rim Max", range(0, 1)) = 1
        _DirRim("Directional Rim", range(0, 1)) = 0
        
        [Toggle(SPECULAR_ON)]_SpecularOn("Specular", float) = 0

        _SpecularMap("Specular Map", 2D) = "white" {}
        _Specular("Specular Highlights", range(0, 1)) = 0.1
        _Shiness("Specular shiness", range(0.1, 1)) = 0.1
        _SpecularMin("Specular Min", Color) = (0,0,0,0)
        _SpecularMax("Specular Max", Color) = (1,1,1,1)

        [Toggle(TOON_ON)]_ToonOn("Toon", float) = 0

        _RampTex("Texture", 2D) = "white" {}
        _RampMin("Ramp Min", Color) = (0,0,0,1)
        _RampMax("Ramp Max", Color) = (1,1,1,1)

        [Toggle(OUTLINE_ON)] _OutlineOn("Enable Outline", Int) = 0
        _OutlineWidth("Outline Width", Float) = 1.0
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineScale("Outline Scale", Float) = 1.0
        _OutlineDepthOffset("Outline Depth Offset", Range(0, 1)) = 0.0
        _CameraDistanceImpact("Outline Camera Distance Impact", Range(0, 1)) = 0.0

        [Toggle(CURVE_ON)]_CurveOn("Curve", float) = 0

        [Toggle(FOLIAGE_ON)] _FoliageOn("Foliage", float) = 0
        _FoliageHeightDisplacementMultiplier("Displacement DIstance Multiplier", float) = 1
        _MaxTargetDistance("Max Target Distance", float) = 4
        _FoliageMask("FoliageMask", 2D) = "white" {}

        [Toggle(WIND_ON)] _WindOn("Wind", float) = 0
        _WindTex("Wind Pattern", 2D) = "black" {}
        _WindTexSize("Wind Pattern Size", Vector) = (1, 1, 0, 0)
        _WindVelocity("Wind Velocity", Vector) = (0.1, 0.1, 0, 0)
        _WindPower("Wind Power", Range(0, 1)) = 1

        [Toggle(USE_GLOBAL_MULTIPLIERS)] _UseGlobalMultipliers("Use Global Multipliers", float) = 0

        [HideInInspector]_SrcBlend ("", Float) = 1
        [HideInInspector]_DstBlend ("", Float) = 0
        [HideInInspector]_ZWrite ("", Float) = 1
        [HideInInspector]_Cull ("", Float) = 0
    }

    SubShader
    {
        Cull back

        Tags { 
            "RenderType" = "Opaque" 
    
            "RenderPipeline" = "UniversalPipeline"
            "LightMode" = "UniversalForward"

            "Queue" = "AlphaTest"
            "IgnoreProjector" = "True"

            "UniversalMaterialType" = "Lit" 
            "ShaderModel" = "4.5"
        }

        Pass
        {
            Name "MainPass"

            Tags{"LightMode" = "UniversalForward"}

            // For transparency
            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM

            // Custom features includes
            #include_with_pragmas "Includes/Universal/UniversalLitInit.hlsl"
            #include "Includes/Universal/UniversalLitPass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            Tags{"LightMode" = "SRPDefaultUnlit"}

            Cull Front

            HLSLPROGRAM

            #include_with_pragmas "Includes/Universal/UniversalOutlineInit.hlsl"
            #include "Includes/Universal/UniversalOutlinePass.hlsl"

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags{"LightMode" = "ShadowCaster"}

            ZWrite On
            ZTest LEqual
            Cull[_Cull]

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library

            #include_with_pragmas "Includes/Universal/UniversalShadowsInit.hlsl"
            #include "Includes/Universal/UniversalShadowsPass.hlsl"

            ENDHLSL
        }
    }

    CustomEditor "Watermelon.Shader.UniversalGUI"
}