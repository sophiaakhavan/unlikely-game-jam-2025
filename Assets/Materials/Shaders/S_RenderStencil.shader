Shader "Custom/RenderStencil"
{
    Properties
    {
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
            // Alpha blending
            Blend SrcColor SrcAlpha

            Stencil
            {
                Ref [_StencilID]
                Comp Equal
            }
        }
    }
}
