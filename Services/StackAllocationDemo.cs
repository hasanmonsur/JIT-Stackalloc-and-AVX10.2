using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PerformanceOptimizations.Services
{
    public unsafe class StackAllocationDemo
    {
        // Scenario: Small, temporary buffers for cryptographic operations
        [Benchmark]
        public void HeapAllocatedBuffer()
        {
            byte[] buffer = new byte[128];
            // Simulate hash computation
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)(buffer[i] ^ 0xFF);
        }

        [Benchmark]
        public void StackAllocatedBuffer()
        {
            Span<byte> buffer = stackalloc byte[128];
            // Same operation, no GC pressure
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = (byte)(buffer[i] ^ 0xFF);
        }
    }
}
