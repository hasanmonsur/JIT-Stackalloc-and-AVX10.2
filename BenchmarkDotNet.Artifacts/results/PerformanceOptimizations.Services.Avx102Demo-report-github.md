```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
Intel Core i7-8750H CPU 2.20GHz (Max: 2.21GHz) (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.100-preview.6.25358.103
  [Host]     : .NET 10.0.0 (10.0.25.35903), X64 RyuJIT AVX2
  DefaultJob : .NET 10.0.0 (10.0.25.35903), X64 RyuJIT AVX2


```
| Method                 | Mean     | Error     | StdDev    | Ratio | RatioSD |
|----------------------- |---------:|----------:|----------:|------:|--------:|
| StandardVectorMultiply | 1.352 μs | 0.0254 μs | 0.0272 μs |  1.00 |    0.03 |
| Avx102VectorMultiply   | 1.303 μs | 0.0259 μs | 0.0387 μs |  0.96 |    0.03 |
