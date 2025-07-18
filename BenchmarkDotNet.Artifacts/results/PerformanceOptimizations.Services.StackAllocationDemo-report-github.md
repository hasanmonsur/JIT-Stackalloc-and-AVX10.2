```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4652/24H2/2024Update/HudsonValley)
Intel Core i7-8750H CPU 2.20GHz (Max: 2.21GHz) (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK 10.0.100-preview.6.25358.103
  [Host]     : .NET 10.0.0 (10.0.25.35903), X64 RyuJIT AVX2
  DefaultJob : .NET 10.0.0 (10.0.25.35903), X64 RyuJIT AVX2


```
| Method               | Mean     | Error    | StdDev   |
|--------------------- |---------:|---------:|---------:|
| HeapAllocatedBuffer  | 67.12 ns | 1.352 ns | 1.129 ns |
| StackAllocatedBuffer | 64.95 ns | 0.880 ns | 0.823 ns |
