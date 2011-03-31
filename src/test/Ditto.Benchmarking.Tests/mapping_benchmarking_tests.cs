using System;
using Ditto.Internal;

namespace Ditto.Benchmarking.Tests
{
    public class benchmarking_tests
    {
        private static BenchSource source;
        private static BenchDestination dest;
        private static BenchSourceProps propsSource;
        private static BenchDestinationProps propsDest;
        private static DefaultMapCommand mapper;
        private static DefaultMapCommand propsMapper;
        private static IExecuteMapping executableMapping;
        private static TestContextualizer contextualizer;

        public static void Init(string[] args)
        {
            source = new BenchSource();
            dest = new BenchDestination();
//            mapper = GetMapper();
            propsSource = new BenchSourceProps();
            propsDest = new BenchDestinationProps();
            SetPropsMapper();
        }

        private static BenchDestination.Int1 Map(BenchSource.Int1 s, BenchDestination.Int1 d)
        {
            if (s == null)
            {
                return null;
            }
            if (d == null)
            {
                d = new BenchDestination.Int1();
            }
            d.i = s.i;
            d.str1 = s.str1;
            d.str2 = s.str2;
            return d;
        }

        private static BenchDestination.Int2 Map(BenchSource.Int2 s, BenchDestination.Int2 d)
        {
            if (s == null)
            {
                return null;
            }

            if (d == null)
            {
                d = new BenchDestination.Int2();
            }
            d.i1 = Map(s.i1, d.i1);
            d.i2 = Map(s.i2, d.i2);
            d.i3 = Map(s.i3, d.i3);
            d.i4 = Map(s.i4, d.i4);
            d.i5 = Map(s.i5, d.i5);
            d.i6 = Map(s.i6, d.i6);
            d.i7 = Map(s.i7, d.i7);

            return d;
        }

        private static BenchDestinationProps.Int1 Map(BenchSourceProps.Int1 s, BenchDestinationProps.Int1 d)
        {
            if (s == null)
            {
                return null;
            }
            if (d == null)
            {
                d = new BenchDestinationProps.Int1();
            }
            d.i = s.i;
            d.str1 = s.str1;
            d.str2 = s.str2;
            return d;
        }

        private static BenchDestinationProps.Int2 Map(BenchSourceProps.Int2 s, BenchDestinationProps.Int2 d)
        {
            if (s == null)
            {
                return null;
            }

            if (d == null)
            {
                d = new BenchDestinationProps.Int2();
            }
            d.i1 = Map(s.i1, d.i1);
            d.i2 = Map(s.i2, d.i2);
            d.i3 = Map(s.i3, d.i3);
            d.i4 = Map(s.i4, d.i4);
            d.i5 = Map(s.i5, d.i5);
            d.i6 = Map(s.i6, d.i6);
            d.i7 = Map(s.i7, d.i7);

            return d;
        }

        private static BenchDestination Map(BenchSource s, BenchDestination d)
        {
            if (s == null)
            {
                return null;
            }
            if (d == null)
            {
                d = new BenchDestination();
            }
            d.i1 = Map(s.i1, d.i1);
            d.i2 = Map(s.i2, d.i2);
            d.i3 = Map(s.i3, d.i3);
            d.i4 = Map(s.i4, d.i4);
            d.i5 = Map(s.i5, d.i5);
            d.i6 = Map(s.i6, d.i6);
            d.i7 = Map(s.i7, d.i7);
            d.i8 = Map(s.i8, d.i8);

            d.n2 = s.n2;
            d.n3 = s.n3;
            d.n4 = s.n4;
            d.n5 = s.n5;
            d.n6 = s.n6;
            d.n7 = s.n7;
            d.n8 = s.n8;
            d.n9 = s.n9;

            d.s1 = s.s1;
            d.s2 = s.s2;
            d.s3 = s.s3;
            d.s4 = s.s4;
            d.s5 = s.s5;
            d.s6 = s.s6;
            d.s7 = s.s7;

            return d;
        }

        private static BenchDestinationProps Map(BenchSourceProps s, BenchDestinationProps d)
        {
            if (s == null)
            {
                return null;
            }
            if (d == null)
            {
                d = new BenchDestinationProps();
            }
            d.i1 = Map(s.i1, d.i1);
            d.i2 = Map(s.i2, d.i2);
            d.i3 = Map(s.i3, d.i3);
            d.i4 = Map(s.i4, d.i4);
            d.i5 = Map(s.i5, d.i5);
            d.i6 = Map(s.i6, d.i6);
            d.i7 = Map(s.i7, d.i7);
            d.i8 = Map(s.i8, d.i8);

            d.n2 = s.n2;
            d.n3 = s.n3;
            d.n4 = s.n4;
            d.n5 = s.n5;
            d.n6 = s.n6;
            d.n7 = s.n7;
            d.n8 = s.n8;
            d.n9 = s.n9;

            d.s1 = s.s1;
            d.s2 = s.s2;
            d.s3 = s.s3;
            d.s4 = s.s4;
            d.s5 = s.s5;
            d.s6 = s.s6;
            d.s7 = s.s7;

            return d;
        }

        private static DefaultMapCommand GetMapper()
        {
            try
            {
                if (mapper != null) return mapper;
                var cfg = new DestinationConfiguration(typeof (BenchDestination));
                cfg.From(typeof (BenchSource));
                cfg.Assert();

                var executable = cfg.CreateExecutableMapping(typeof (BenchSource));
                return new DefaultMapCommand(executable, new TestContextualizer());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static void SetPropsMapper()
        {
            try
            {
                var int1 = new DestinationConfiguration(typeof (BenchDestinationProps.Int1));
                int1.From(typeof (BenchSourceProps.Int1));
                var int2 = new DestinationConfiguration(typeof (BenchDestinationProps.Int2));
                int2.From(typeof (BenchSourceProps.Int2));
                int2.Bind(int1);
                var cfg = new DestinationConfiguration(typeof (BenchDestinationProps));
                cfg.From(typeof (BenchSourceProps));
                cfg.Bind(int2);

                contextualizer = new TestContextualizer();
                var cacher = new CacheInitializer(contextualizer);
                foreach (var configuration in new[] {int1, int2, cfg})
                {
                    ((ICacheable) configuration).Accept(cacher);
                }
                cfg.Assert();

                executableMapping = cfg.CreateExecutableMapping(typeof (BenchSourceProps));
                propsMapper = new DefaultMapCommand(executableMapping, new TestContextualizer());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [Benchmark]
        public static void handwritten_benchmark()
        {
            for (var i = 0; i < 10000; i++)
                Map(propsSource, propsDest);
        }

        [Benchmark]
        public static void props_mapper_benchmark()
        {
            for (var i = 0; i < 10000; i++)
                propsMapper.Map(propsSource, propsDest);
        }

        public class BenchDestination
        {
            public long n2 = 2;
            public long n3 = 3;
            public long n4 = 4;
            public long n5 = 5;
            public long n6 = 6;
            public long n7 = 7;
            public long n8 = 8;
            public long n9 = 9;

            public string s1;
            public string s2;
            public string s3;
            public string s4;
            public string s5;
            public string s6;
            public string s7;
            public Int2 i1 { get; set; }
            public Int2 i2 { get; set; }
            public Int2 i3 { get; set; }
            public Int2 i4 { get; set; }
            public Int2 i5 { get; set; }
            public Int2 i6 { get; set; }
            public Int2 i7 { get; set; }
            public Int2 i8 { get; set; }

            public class Int1
            {
                public int i;
                public string str1;
                public string str2;
            }

            public class Int2
            {
                public Int1 i1;
                public Int1 i2;
                public Int1 i3;
                public Int1 i4;
                public Int1 i5;
                public Int1 i6;
                public Int1 i7;
            }
        }

        public class BenchDestinationProps
        {
            public BenchDestinationProps()
            {
                n2 = 2;
                n3 = 3;
                n4 = 4;
                n5 = 5;
                n6 = 6;
                n7 = 7;
                n8 = 8;
                n9 = 9;
            }

            public Int2 i1 { get; set; }
            public Int2 i2 { get; set; }
            public Int2 i3 { get; set; }
            public Int2 i4 { get; set; }
            public Int2 i5 { get; set; }
            public Int2 i6 { get; set; }
            public Int2 i7 { get; set; }
            public Int2 i8 { get; set; }

            public long n2 { get; set; }
            public long n3 { get; set; }
            public long n4 { get; set; }
            public long n5 { get; set; }
            public long n6 { get; set; }
            public long n7 { get; set; }
            public long n8 { get; set; }
            public long n9 { get; set; }

            public string s1 { get; set; }
            public string s2 { get; set; }
            public string s3 { get; set; }
            public string s4 { get; set; }
            public string s5 { get; set; }
            public string s6 { get; set; }
            public string s7 { get; set; }

            public class Int1
            {
                public int i { get; set; }
                public string str1 { get; set; }
                public string str2 { get; set; }
            }

            public class Int2
            {
                public Int1 i1 { get; set; }
                public Int1 i2 { get; set; }
                public Int1 i3 { get; set; }
                public Int1 i4 { get; set; }
                public Int1 i5 { get; set; }
                public Int1 i6 { get; set; }
                public Int1 i7 { get; set; }
            }
        }

        public class BenchSource
        {
            public Int2 i1 = new Int2();
            public Int2 i2 = new Int2();
            public Int2 i3 = new Int2();
            public Int2 i4 = new Int2();
            public Int2 i5 = new Int2();
            public Int2 i6 = new Int2();
            public Int2 i7 = new Int2();
            public Int2 i8 = new Int2();

            public int n2;
            public long n3;
            public byte n4;
            public short n5;
            public uint n6;
            public int n7;
            public int n8;
            public int n9;

            public string s1 = "1";
            public string s2 = "2";
            public string s3 = "3";
            public string s4 = "4";
            public string s5 = "5";
            public string s6 = "6";
            public string s7 = "7";

            public class Int1
            {
                public int i = 10;
                public string str1 = "1";
                public string str2 = null;
            }

            public class Int2
            {
                public Int1 i1 = new Int1();
                public Int1 i2 = new Int1();
                public Int1 i3 = new Int1();
                public Int1 i4 = new Int1();
                public Int1 i5 = new Int1();
                public Int1 i6 = new Int1();
                public Int1 i7 = new Int1();
            }
        }

        public class BenchSourceProps
        {
            public BenchSourceProps()
            {
                s1 = "1";
                s2 = "2";
                s3 = "3";
                s4 = "4";
                s5 = "5";
                s6 = "6";
                s7 = "7";


                i1 = new Int2();
                i2 = new Int2();
                i3 = new Int2();
                i4 = new Int2();
                i5 = new Int2();
                i6 = new Int2();
                i7 = new Int2();
                i8 = new Int2();
            }

            public Int2 i1 { get; set; }
            public Int2 i2 { get; set; }
            public Int2 i3 { get; set; }
            public Int2 i4 { get; set; }
            public Int2 i5 { get; set; }
            public Int2 i6 { get; set; }
            public Int2 i7 { get; set; }
            public Int2 i8 { get; set; }

            public int n2 { get; set; }
            public long n3 { get; set; }
            public byte n4 { get; set; }
            public short n5 { get; set; }
            public uint n6 { get; set; }
            public int n7 { get; set; }
            public int n8 { get; set; }
            public int n9 { get; set; }

            public string s1 { get; set; }
            public string s2 { get; set; }
            public string s3 { get; set; }
            public string s4 { get; set; }
            public string s5 { get; set; }
            public string s6 { get; set; }
            public string s7 { get; set; }

            public class Int1
            {
                public Int1()
                {
                    str1 = "1";
                    str2 = null;
                    i = 10;
                }

                public int i { get; set; }

                public string str1 { get; set; }
                public string str2 { get; set; }
            }

            public class Int2
            {
                public Int2()
                {
                    i1 = new Int1();
                    i2 = new Int1();
                    i3 = new Int1();
                    i4 = new Int1();
                    i5 = new Int1();
                    i6 = new Int1();
                    i7 = new Int1();
                }

                public Int1 i1 { get; set; }
                public Int1 i2 { get; set; }
                public Int1 i3 { get; set; }
                public Int1 i4 { get; set; }
                public Int1 i5 { get; set; }
                public Int1 i6 { get; set; }
                public Int1 i7 { get; set; }
            }
        }
    }
}