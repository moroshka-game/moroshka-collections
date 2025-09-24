using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;

namespace Moroshka.Collections.Benchmark;

[MemoryDiagnoser]
[RankColumn]
[SuppressMessage("Performance", "CA1822")]
public sealed class Benchmark
{
}
