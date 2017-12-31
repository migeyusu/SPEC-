using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var blockingCollection = new BlockingCollection<string>(){"s","sdf","dsf","sdd"};
            var pipeline = Pipeline.Create<string, string>((s => {
                    Thread.Sleep(100);
                    return s;
                }))
                .Next(s => {
                    Console.WriteLine(s);
                    blockingCollection.Add(s);
                    Thread.Sleep(100);
                    return s;
                });
            var enumerable = pipeline.Process(blockingCollection);
            Console.WriteLine(enumerable.Count());
            Console.ReadLine();
        }
    }
}
