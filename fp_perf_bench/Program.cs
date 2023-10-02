using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Numerics;

namespace fp_perf_bench;

internal class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<FloatingPointBenchmark>();
        Console.WriteLine(summary);
        Console.ReadLine();
    }
}


// No difference between fp32 and fp64 for regular .Net/C#
// The SIMD enabled Vector3 is faster, but difference gets smaller with more iterations

//| Method       | N   | Mean        | Error      | StdDev     |
//|------------- |---- |------------:|-----------:|-----------:|
//| FP32         | 1   |   6.1526 ns |  0.0533 ns |  0.0499 ns |
//| FP64         | 1   |   6.4726 ns |  0.0740 ns |  0.0618 ns |
//| SIMD_Vector3 | 1   |   0.8304 ns |  0.0092 ns |  0.0082 ns |
//| FP32         | 10  |  60.6196 ns |  0.4866 ns |  0.4063 ns |
//| FP64         | 10  |  65.4759 ns |  0.3650 ns |  0.3235 ns |
//| SIMD_Vector3 | 10  |  17.8671 ns |  0.1339 ns |  0.1252 ns |
//| FP32         | 100 | 731.4939 ns |  3.8856 ns |  3.4445 ns |
//| FP64         | 100 | 795.0813 ns | 14.9072 ns | 11.6386 ns |
//| SIMD_Vector3 | 100 | 412.1174 ns |  1.2826 ns |  1.1370 ns |
public class FloatingPointBenchmark
{
    [Params(1, 10, 100)]
    public int N;

    [GlobalSetup]
    public void Setup()
    {
    }

    [Benchmark]
    public Vector3 FP32()
    {
        Vector3 v = new(3.1f, 3.2f, 3.3f);
        float pi = (float)Math.PI;

        for (int i = 0; i < N; i++)
        {
            v.X *= pi;
            v.Y *= pi;
            v.Z *= pi;

            v.X += 1f;
            v.Y += 1f;
            v.Z += 1f;

            v.X /= 5f;
            v.Y /= 5f;
            v.Z /= 5f;
        }

        return v;
    }

    public struct Vector3d
    {
        public double X, Y, Z;
    }

    [Benchmark]
    public Vector3d FP64()
    {
        Vector3d v = new Vector3d { X = 3.1d, Y = 3.2d, Z = 3.3d };
        double pi = Math.PI;

        for (int i = 0; i < N; i++)
        {
            v.X *= pi;
            v.Y *= pi;
            v.Z *= pi;

            v.X += 1d;
            v.Y += 1d;
            v.Z += 1d;

            v.X /= 5d;
            v.Y /= 5d;
            v.Z /= 5d;
        }

        return v;
    }

    [Benchmark]
    public Vector3 SIMD_Vector3()
    {
        Vector3 v = new(3.1f, 3.2f, 3.3f);
        Vector3 addVec = new(1, 1, 1);
        float pi = (float)Math.PI;

        for (int i = 0; i < N; i++)
        {
            v *= pi;
            v = Vector3.Add(v, addVec);
            v /= 5f;
        }

        return v;
    }
}