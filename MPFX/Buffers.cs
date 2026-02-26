using Brutal.VulkanApi.Abstractions;
using KSA;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShaderExtensions
{
    [AttributeUsage(AttributeTargets.Struct)]
    internal class SxUniformBufferAttribute(string xmlElement) : Attribute;


    [AttributeUsage(AttributeTargets.Field)]
    internal class SxUniformBufferLookupAttribute() : Attribute;

    public delegate BufferEx MPFXBufferLookup(KeyHash hash);
    public delegate MappedMemory MPFXMemoryLookup(KeyHash hash);
    public delegate Span<T> MPFXSpanLookup<T>(KeyHash hash) where T : unmanaged;
    public unsafe delegate T* MPFXPtrLookup<T>(KeyHash hash) where T : unmanaged;
}
