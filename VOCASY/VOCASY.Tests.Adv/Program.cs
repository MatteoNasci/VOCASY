using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VOCASY.Utility;
namespace VOCASY.Tests.Adv
{
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
                GamePacket p = GamePacket.CreatePacket(40000);
                GamePacket p2 = GamePacket.CreatePacket(40000);

                watch.Reset();
                watch.Start();
                for (int z = 0; z < n; z++)
                {
                    Utils.Write(p.Data, 0, p2.Data, 0, 40000);
                }
                watch.Stop();



                long time = watch.ElapsedMilliseconds;
                d1 += time;
                c1++;



                float f = 5f;
                float a;
                watch.Reset();
                watch.Start();
                for (int z = 0; z < n; z++)
                {

                }
                watch.Stop();



                long time2 = watch.ElapsedMilliseconds;
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
