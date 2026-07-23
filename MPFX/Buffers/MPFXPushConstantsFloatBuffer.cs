using ShaderExtensions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace MPFX.Buffers
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [SxPushConstant("MPFXPushConstantsFloatBufferAsset")]
    public struct MPFXPushConstantsFloatBuffer
    {
        public float a;

        // lookup delegate fields must be static fields on the buffer element type
        [SxPushConstantLookup] public static MPFXSpanLookup<MPFXPushConstantsFloatBuffer> LookupSpan; // gives a Span<T> of length Size
    }
}
