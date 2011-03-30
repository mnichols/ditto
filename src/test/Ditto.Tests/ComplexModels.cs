namespace Ditto.Tests
{
    public class SourceWithComponentArray
    {
        public IntegerSource[] IntegerComponents { get; set; }
    }
    public class DestinationWithComponentArray
    {
        public IntegerDest[] IntegerComponents { get; set; }
    }
    public class DestinationWithUnmappedMember
    {
        public string UnmappedMember { get; set; }
        public string Name { get; set; }

    }
    public class DestinationWrapper
    {
        public DestinationWithUnmappedMember Component { get; set; }
    }
    
    public class SourceWithIncompleteMembers
    {
        public string Name { get; set; }
    }
    public class NestedPropsSource
    {
        public NestedPropsSource()
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

        public class Int1
        {
            public Int1()
            {
                str1 = "1";
                str2 = null;
                i = 10;
            }

            public string str1 { get; set; }
            public string str2 { get; set; }
            public int i { get; set; }
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

    }
    public class NestedPropsDestination
    {
        public NestedPropsDestination()
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

        public class Int1
        {
            public string str1 { get; set; }
            public string str2 { get; set; }
            public int i { get; set; }
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
    }
}