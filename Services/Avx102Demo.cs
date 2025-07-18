using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace PerformanceOptimizations.Services
{
    public class Avx102Demo
    {
        private readonly float[] _vectorA = new float[1024];
        private readonly float[] _vectorB = new float[1024];
        private readonly float[] _result = new float[1024];

        public Avx102Demo()
        {
            var rand = new Random();
            for (int i = 0; i < _vectorA.Length; i++)
            {
                _vectorA[i] = (float)rand.NextDouble();
                _vectorB[i] = (float)rand.NextDouble();
            }
        }

        [Benchmark(Baseline = true)]
        public void StandardVectorMultiply()
        {
            for (int i = 0; i < _vectorA.Length; i++)
                _result[i] = _vectorA[i] * _vectorB[i];
        }

        [Benchmark]
        public void Avx102VectorMultiply()
        {
            if (Avx10v2.IsSupported)
            {
                int vectorSize = Vector256<float>.Count;
                int i = 0;

                for (; i <= _vectorA.Length - vectorSize; i += vectorSize)
                {
                    var va = Vector256.LoadUnsafe(ref _vectorA[i]);
                    var vb = Vector256.LoadUnsafe(ref _vectorB[i]);
                    var vr = Avx10v2.Multiply(va, vb);
                    vr.StoreUnsafe(ref _result[i]);
                }

                // Remainder
                for (; i < _vectorA.Length; i++)
                    _result[i] = _vectorA[i] * _vectorB[i];
            }
            else
            {
                StandardVectorMultiply();
            }
        }
    }
}
