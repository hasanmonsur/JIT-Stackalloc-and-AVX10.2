using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PerformanceOptimizations.Services
{
    public class JitOptimizations
    {
        // Scenario: Processing large arrays with interface patterns
        private readonly int[] _data = new int[10_000];

        public JitOptimizations()
        {
            Random.Shared.NextBytes(MemoryMarshal.AsBytes(_data.AsSpan()));
        }

        [Benchmark]
        public int SumWithIEnumerable() => SumIEnumerable(_data);

        [Benchmark]
        public int SumWithArray() => SumArray(_data);

        // Virtual call pattern (old)
        private static int SumIEnumerable(IEnumerable<int> values)
        {
            int sum = 0;
            foreach (var item in values) sum += item;
            return sum;
        }

        // Direct array access (new JIT optimization)
        private static int SumArray(int[] values)
        {
            int sum = 0;
            foreach (var item in values) sum += item;
            return sum;
        }
    }
}
