using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GENUtility;
namespace VOCASY.Tests.Adv
{
    [Serializable]
    [Flags]
    public enum Boh : long
    {
        None = 0,
        Item = 1,
        Item1 = 1 << 1,
        Item2 = 1 << 2,
        Item3 = 1 << 3,
        Item4 = 1 << 4,
        Item5 = 1 << 5,
        Item6 = 1 << 6,
        Item7 = 1 << 7,
        Item8 = 1 << 8,
        Item9 = 1 << 9,
        Item10 = 1 << 10,
        Item11 = 1 << 11,
        Item12 = 1 << 12,
        Item13 = 1 << 13,
        Item14 = 1 << 14,
        Item15 = 1 << 15,
        Item16 = 1 << 16,
        Item17 = 1 << 17,
        Item18 = 1 << 18,
        Item19 = 1 << 19,
        Item20 = 1 << 20,
        Item21 = 1 << 21,
        Item22 = 1 << 22,
        Item23 = 1 << 23,
        Item24 = 1 << 24,
        Item25 = 1 << 25,
        Item26 = 1 << 26,
        Item27 = 1 << 27,
        Item28 = 1 << 28,
        Item29 = 1 << 29,
        Item30 = 1 << 30,
        Item31 = 1 << 31,
        Item32 = 1 << 32,
        Item33 = 1 << 33,
        Item34 = 1 << 34,
        Item35 = 1 << 35,
        Item36 = 1 << 36,
        Item37 = 1 << 37,
        Item38 = 1 << 38,
        Item39 = 1 << 39,
        Item40 = 1 << 40,
        Item41 = 1 << 41,
        Item42 = 1 << 42,
        Item43 = 1 << 43,
        Item44 = 1 << 44,
        Item45 = 1 << 45,
        Item46 = 1 << 46,
        Item47 = 1 << 47,
        Item48 = 1 << 48,
        Item49 = 1 << 49,
        Item50 = 1 << 50,
        Item51 = 1 << 51,
        Item52 = 1 << 52,
        Item53 = 1 << 53,
        Item54 = 1 << 54,
        Item55 = 1 << 55,
        Item56 = 1 << 56,
        Item57 = 1 << 57,
        Item58 = 1 << 58,
        Item59 = 1 << 59,
        Item60 = 1 << 60,
        Item61 = 1 << 61,
        Item62 = 1 << 62,
        Item63 = 1 << 63,
        All = long.MaxValue,
    }
    class Program
    {
        static Stopwatch watch = new Stopwatch();
        static double AverageFirst;
        static double AverageSecond;
        static int False;
        static int True;
        static double d1;
        static double d2;
        static uint c1;
        static uint c2;
        static uint cycles = 1000;
        static void Main(string[] args)
        {

            int n = 200000;

            for (int i = 0; i < cycles; i++)
            {
                BytePacket p = new BytePacket(40000);
                BytePacket p2 = new BytePacket(40000);
                Boh value = Boh.Item1 | Boh.Item10 | Boh.Item11;
                watch.Reset();
                watch.Start();
                bool res = false;
                bool res1 = false;
                bool res2 = false;
                for (int z = 0; z < n; z++)
                {
                    if (!res && !res1)
                        res2 = true;
                }
                watch.Stop();



                long time = watch.ElapsedTicks;
                d1 += time;
                c1++;



                float f = 5f;
                float a;
                watch.Reset();
                watch.Start();
                for (int z = 0; z < n; z++)
                {
                    if (!res)
                        if (!res1)
                            res2 = true;
                }
                watch.Stop();



                long time2 = watch.ElapsedTicks;
                d2 += time2;
                c2++;

                if (time > time2)
                    False++;
                else
                    True++;

                AverageFirst = d1 / c1;
                AverageSecond = d2 / c2;

                Console.WriteLine("Avrg first : {0} , win frames : {1} .   Avrg second : {2} , win frames : {3}", AverageFirst, True, AverageSecond, False);
            }

            Console.WriteLine("End");
            Console.ReadLine();

        }
    }
}
