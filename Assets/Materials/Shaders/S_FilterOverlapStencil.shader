Shader "Custom/FilterOverlapStencil"
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
            BlendOp Max
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref [_StencilID]
                Comp Equal
            }
        }
    }
}
