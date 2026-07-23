using ShaderExtensions;
using System.Runtime.InteropServices;

namespace MPFX.Buffers
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [SxUniformBuffer("MPFXGaussianBlurBufferAsset")]
    public unsafe struct MPFXGaussianBlurBuffer
    {
        public int Radius;
        public int Padding0;
        public int Padding1;
        public int Padding2;
        public fixed float Weights[24];

        // lookup delegate fields must be static fields on the buffer element type
        [SxUniformBufferLookup] public static MPFXSpanLookup<MPFXGaussianBlurBuffer> LookupSpan; // gives a Span<T> of length Size
    }
}
