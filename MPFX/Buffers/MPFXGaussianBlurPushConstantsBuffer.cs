using ShaderExtensions;
using System.Runtime.InteropServices;

namespace MPFX.Buffers
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [SxPushConstant("MPFXGaussianBlurPushConstantsBufferAsset")]
    public unsafe struct MPFXGaussianBlurPushConstantsBuffer
    {
        public int Radius;
        public fixed float Weights[21];

        // lookup delegate fields must be static fields on the buffer element type
        [SxPushConstantLookup] public static MPFXSpanLookup<MPFXGaussianBlurPushConstantsBuffer> LookupSpan; // gives a Span<T> of length Size
    }
}
