using BenchmarkDotNet.Running;
using PerformanceOptimizations.Services;
using System;
using System.Diagnostics;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

Console.WriteLine("=== .NET 10 Performance Demo ===");
Console.WriteLine($"AVX10.2 Supported: {Avx10v2.IsSupported}");
Console.WriteLine($"Vector256<float>.Count: {Vector256<float>.Count}");

// Run benchmarks
BenchmarkRunner.Run<JitOptimizations>();
BenchmarkRunner.Run<StackAllocationDemo>();
BenchmarkRunner.Run<Avx102Demo>();

// Real-world use case demo
RunImageProcessingDemo();

static void RunImageProcessingDemo()
{
    Console.WriteLine("\n=== Image Convolution Demo ===");

    const int width = 1024;
    const int height = 768;
    const int kernelSize = 5;

    // Validate kernel size is odd
    if (kernelSize % 2 == 0)
        throw new ArgumentException("Kernel size must be odd");

    float[,] kernel = CreateGaussianKernel(kernelSize);

    // Allocate image buffers
    var inputImage = new float[width * height];
    var outputImage = new float[width * height];

    // Simulate image data
    var rand = new Random();
    for (int i = 0; i < inputImage.Length; i++)
        inputImage[i] = (float)rand.NextDouble();

    // Process with optimizations
    var sw = Stopwatch.StartNew();
    ApplyConvolutionOptimized(inputImage, outputImage, width, height, kernel);
    Console.WriteLine($"Processed {width}x{height} image in {sw.ElapsedMilliseconds}ms");
}

static unsafe void ApplyConvolutionOptimized(
    float[] input, float[] output, int width, int height, float[,] kernel)
{
    int kernelSize = kernel.GetLength(0);
    int radius = kernelSize / 2;

    // Use FMA where available (separate from AVX10.2)
    bool useFma = Fma.IsSupported;
    bool useAvx = Avx.IsSupported;
    int vectorSize = Vector256<float>.Count;

    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            float sum = 0f;

            if (useAvx && x + vectorSize <= width)
            {
                var sumVector = Vector256<float>.Zero;

                for (int ky = 0; ky < kernelSize; ky++)
                {
                    for (int kx = 0; kx < kernelSize; kx += vectorSize)
                    {
                        int index = (y + ky - radius) * width + (x + kx - radius);
                        var pixelVector = Vector256.LoadUnsafe(ref input[index]);
                        var kernelVector = Vector256.Create(kernel[ky, kx]);

                        if (useFma)
                        {
                            sumVector = Fma.MultiplyAdd(pixelVector, kernelVector, sumVector);
                        }
                        else
                        {
                            sumVector = Avx.Add(sumVector, Avx.Multiply(pixelVector, kernelVector));
                        }
                    }
                }
                sum = Vector256.Sum(sumVector);
            }
            else
            {
                // Scalar fallback
                for (int ky = 0; ky < kernelSize; ky++)
                {
                    for (int kx = 0; kx < kernelSize; kx++)
                    {
                        int index = (y + ky - radius) * width + (x + kx - radius);
                        if (index >= 0 && index < input.Length)
                        {
                            sum += input[index] * kernel[ky, kx];
                        }
                    }
                }
            }

            output[y * width + x] = sum;
        }
    }
}

static float[,] CreateGaussianKernel(int size)
{
    if (size <= 0)
        throw new ArgumentOutOfRangeException(nameof(size), "Kernel size must be positive");

    float[,] kernel = new float[size, size];
    float sum = 0f;
    int radius = size / 2;

    for (int y = -radius; y <= radius; y++)
    {
        for (int x = -radius; x <= radius; x++)
        {
            float value = (float)Math.Exp(-(x * x + y * y) / (2 * radius * radius));
            kernel[y + radius, x + radius] = value;
            sum += value;
        }
    }

    // Normalize kernel
    if (sum == 0) sum = 1; // Prevent division by zero
    for (int y = 0; y < size; y++)
    {
        for (int x = 0; x < size; x++)
        {
            kernel[y, x] /= sum;
        }
    }

    return kernel;
}