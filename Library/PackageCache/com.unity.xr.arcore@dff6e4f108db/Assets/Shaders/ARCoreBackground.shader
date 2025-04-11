Shader "Unlit/ARCoreBackground"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _EnvironmentDepth("Texture", 2D) = "black" {}
    }

    SubShader
    {
        Name "ARCore Background (Before Opaques)"
        Tags
        {
            "Queue" = "Background"
            "RenderType" = "Background"
            "ForceNoShadowCasting" = "True"
        }

        Pass
        {
            Name "AR Camera Background (ARCore)"
            Cull Off
            ZTest Always
            ZWrite On
            Lighting Off
            LOD 100
            Tags
            {
                "LightMode" = "Always"
            }

            GLSLPROGRAM

            #pragma multi_compile_local __ ARCORE_ENVIRONMENT_DEPTH_ENABLED
            #pragma multi_compile_local __ ARCORE_IMAGE_STABILIZATION_ENABLED

            #pragma only_renderers gles3

            #include "UnityCG.glslinc"

#ifdef SHADER_API_GLES3
#extension GL_OES_EGL_image_external_essl3 : require
#endif // SHADER_API_GLES3

#ifndef ARCORE_IMAGE_STABILIZATION_ENABLED
#define ARCORE_TEXCOORD_TYPE vec2
#else // ARCORE_IMAGE_STABILIZATION_ENABLED
#define ARCORE_TEXCOORD_TYPE vec3
#endif // !ARCORE_IMAGE_STABILIZATION_ENABLED

            // Device display transform is provided by the AR Foundation camera background renderer.
            uniform mat4 _UnityDisplayTransform;

#ifdef VERTEX
            varying ARCORE_TEXCOORD_TYPE textureCoord;

            void main()
            {
#ifdef SHADER_API_GLES3
                // Transform the position from object space to clip space.
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;

#ifdef ARCORE_IMAGE_STABILIZATION_ENABLED
                textureCoord = gl_MultiTexCoord0.xyz;
#else
                // Remap the texture coordinates based on the device rotation.
                // _UnityDisplayTransform is provided as "Row Major" for all mobile platforms, so use the
                // 'Row Vector * Matrix' operator. The GLSL '*' operator is overloaded to use a row vector
                // when a matrix is left-multiplied by a vector. Refer to this doc for more information:
                // https://en.wikibooks.org/wiki/GLSL_Programming/Vector_and_Matrix_Operations#Operators
                textureCoord = (vec4(gl_MultiTexCoord0.x, gl_MultiTexCoord0.y, 1.0f, 0.0f) * _UnityDisplayTransform).xy;
#endif
#endif // SHADER_API_GLES3
            }
#endif // VERTEX

#ifdef FRAGMENT
            varying ARCORE_TEXCOORD_TYPE textureCoord;
            uniform samplerExternalOES _MainTex;
            uniform float _UnityCameraForwardScale;

#ifdef ARCORE_ENVIRONMENT_DEPTH_ENABLED
            uniform sampler2D _EnvironmentDepth;
#endif // ARCORE_ENVIRONMENT_DEPTH_ENABLED

#if defined(SHADER_API_GLES3) && !defined(UNITY_COLORSPACE_GAMMA)
            float GammaToLinearSpaceExact (float value)
            {
                if (value <= 0.04045F)
                    return value / 12.92F;
                else if (value < 1.0F)
                    return pow((value + 0.055F)/1.055F, 2.4F);
                else
                    return pow(value, 2.2F);
            }

            vec3 GammaToLinearSpace (vec3 sRGB)
            {
                // Approximate version from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
                return sRGB * (sRGB * (sRGB * 0.305306011F + 0.682171111F) + 0.012522878F);

                // Precise version, useful for debugging, but the pow() function is too slow.
                // return vec3(GammaToLinearSpaceExact(sRGB.r), GammaToLinearSpaceExact(sRGB.g), GammaToLinearSpaceExact(sRGB.b));
            }

#endif // SHADER_API_GLES3 && !UNITY_COLORSPACE_GAMMA

            float ConvertDistanceToDepth(float d)
            {
                d = _UnityCameraForwardScale > 0.0 ? _UnityCameraForwardScale * d : d;

                float zBufferParamsW = 1.0 / _ProjectionParams.y;
                float zBufferParamsY = _ProjectionParams.z * zBufferParamsW;
                float zBufferParamsX = 1.0 - zBufferParamsY;
                float zBufferParamsZ = zBufferParamsX * _ProjectionParams.w;

                // Clip any distances smaller than the near clip plane, and compute the depth value from the distance.
                return (d < _ProjectionParams.y) ? 1.0f : ((1.0 / zBufferParamsZ) * ((1.0 / d) - zBufferParamsW));
            }

            void main()
            {
#ifdef SHADER_API_GLES3
#ifdef ARCORE_IMAGE_STABILIZATION_ENABLED
                vec2 tc = textureCoord.xy / textureCoord.z;
#else
                vec2 tc = textureCoord;
#endif
                vec3 result = texture(_MainTex, tc).xyz;
                float depth = 1.0;

#ifdef ARCORE_ENVIRONMENT_DEPTH_ENABLED
                float distance = texture(_EnvironmentDepth, tc).x;
                depth = ConvertDistanceToDepth(distance);
#endif // ARCORE_ENVIRONMENT_DEPTH_ENABLED

#ifndef UNITY_COLORSPACE_GAMMA
                result = GammaToLinearSpace(result);
#endif // !UNITY_COLORSPACE_GAMMA

                gl_FragColor = vec4(result, 1.0);
                gl_FragDepth = depth;
#endif // SHADER_API_GLES3
            }

#endif // FRAGMENT
            ENDGLSL
        }
    }

    FallBack Off
}
