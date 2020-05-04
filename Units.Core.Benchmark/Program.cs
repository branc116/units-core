using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Units.RealFloat;

namespace Units.Core.Benchmark
{
    [MarkdownExporter, AsciiDocExporter, HtmlExporter, CsvExporter, RPlotExporter]
    public class AddingBenchmark
    {
        [Params(1000)]
        public int N { get; set; }
        private readonly Length lf = Length.FromYard(1f);
        private readonly Units.RealDouble.Length ld = Units.RealDouble.Length.FromYard(1);
        private readonly Numbers.RealFloats.RealFloat rf = 1f;
        private readonly Numbers.RealDoubles.RealDouble rd = 1f;
        private readonly float f = 1f;
        private readonly double d = 1;
        private readonly UnitsNet.Length unl = UnitsNet.Length.FromYards(1);

        [Benchmark]
        public Length UnitsCore_Length_Float()
        {
            var v = lf;
            for (int i = 0; i < N; i++)
            {
                v += lf;
            }
            return v;
        }
        [Benchmark]
        public Units.RealDouble.Length UnitsCore_Length_Double()
        {
            var v = ld;
            for (int i = 0; i < N; i++)
            {
                v += ld;
            }
            return v;
        }
        [Benchmark]
        public Numbers.RealFloats.RealFloat UnitsCore_Scalar_Float()
        {
            var v = rf;
            for (int i = 0; i < N; i++)
            {
                v += rf;
            }
            return v;
        }
        [Benchmark]
        public Numbers.RealDoubles.RealDouble UnitsCore_Scalar_Double()
        {
            var v = rd;
            for (int i = 0; i < N; i++)
            {
                v += rd;
            }
            return v;
        }
        [Benchmark]
        public float System_Single()
        {
            var v = f;
            for (int i = 0; i < N; i++)
            {
                v += f;
            }
            return v;
        }
        [Benchmark]
        public double System_Double()
        {
            var v = d;
            for (int i = 0; i < N; i++)
            {
                v += d;
            }
            return v;
        }
        [Benchmark]
        public UnitsNet.Length UnitsNet_Length()
        {
            var v = unl;
            for (int i = 0; i < N; i++)
            {
                v += unl;
            }
            return v;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            unsafe
            {
                Console.WriteLine(sizeof(Length));
                Console.WriteLine(sizeof(Units.RealDouble.Length));
                Console.WriteLine(sizeof(UnitsNet.Length));
            }
            if (args.Length == 0)
            {
                Console.WriteLine("length or float or scalar or unitsnet or lengthD or double or scalarD");
                BenchmarkRunner.Run<AddingBenchmark>();
            }
            if (args[0] == "length")
            {
                var len = Length.FromLightYear(1f);
                var yard = Length.FromYard(1f);
                for (int i = 0; i < 10000000; i++)
                {
                    len += yard;
                }
                System.Console.WriteLine($"{(float)(len.ToMeter())} meters");
            }
            else if (args[0] == "unitsnet")
            {
                var len = UnitsNet.Length.FromYards(1f);
                var yard = UnitsNet.Length.FromYards(1f);
                for (int i = 0; i < 10000000; i++)
                {
                    len += yard;
                    //list.Add(len);
                }
                System.Console.WriteLine($"{len.Meters} meters");
            }
            else if (args[0] == "float")
            {
                var len = 0.9144f;
                var yard = 0.9144f;
                for (int i = 0; i < 10000000; i++)
                {
                    len += yard;
                }
                System.Console.WriteLine($"{len} meters");
            }
            else if (args[0] == "double")
            {
                var len = 0.9144f;
                var yard = 0.9144f;
                for (int i = 0; i < 10000000; i++)
                {
                    len += yard;
                }
                System.Console.WriteLine($"{len} meters");
            }
            else if (args[0] == "scalar")
            {
                var len = (Numbers.RealFloats.RealFloat)1f;
                var yard = (Numbers.RealFloats.RealFloat)0.9144f;
                for (int i = 0; i < 10000000; i++)
                {
                    len += yard;
                }
                System.Console.WriteLine($"{(float)len} meters");
            }
            else if (args[0] == "scalarD")
            {
                var len = (Numbers.RealDoubles.RealDouble)1f;
                var yard = (Numbers.RealDoubles.RealDouble)0.9144f;
                for (int i = 0; i < 10000000; i++)
                {
                    len += yard;
                }
                System.Console.WriteLine($"{(float)len} meters");
            }
            else if (args[0] == "lengthD")
            {
                var len = Units.RealDouble.Length.FromYard(1f);
                var yard = Units.RealDouble.Length.FromYard(1f);
                for (int i = 0; i < 10000000; i++)
                {
                    len += yard;
                }
                System.Console.WriteLine($"{(float)(len)} meters");
            }
        }
    }
}
