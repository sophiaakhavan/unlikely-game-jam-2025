Shader "Custom/MaskStencil"
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
            "Queue" = "Transparent-1"
        }

        Pass
        {
            // Make invisible
            Blend Zero One

            // Modify the given bit (R = 001, G = 010, B = 100) in the stencil buffer
            // EX: If red and green overlap, the stencil buffer value = 011, or yellow
            Stencil
            {
                Ref [_StencilID]
                WriteMask [_StencilID]
                Comp Always
                Pass Replace
            }
        }
    }
}
