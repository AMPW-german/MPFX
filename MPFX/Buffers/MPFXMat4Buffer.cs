using Brutal.Numerics;
using KSA;
using System.Numerics;
using System.Runtime.InteropServices;
using ShaderExtensions;

#pragma warning disable CS9113

namespace MPFX.Buffers
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    [SxUniformBuffer("MPFXMat3BufferAsset")]
    public struct MPFXMat4Buffer
    {
        public float4x4 a;
        public float4x4 b;

        // lookup delegate fields must be static fields on the buffer element type
        // the names and specific types of these are not relevant, as long as the delegate signature matches
        // these are not all required, but you will need at least one to be able to set the uniform data
        [SxUniformBufferLookup] public static MPFXBufferLookup LookupBuffer;
        [SxUniformBufferLookup] public static MPFXMemoryLookup LookupMemory;
        [SxUniformBufferLookup] public static MPFXSpanLookup<MPFXMat4Buffer> LookupSpan; // gives a Span<T> of length Size
        [SxUniformBufferLookup] public static MPFXPtrLookup<MPFXMat4Buffer> LookupPtr; // gives T* to first element
    }
}
