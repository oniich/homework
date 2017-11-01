using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PrimeNumber
{
    class Program
    {
        static void Main(string[] args)
        {
            String a;
            Console.WriteLine("> Set \"from\" \"to\" range.");
            do
            {
                Console.Write("> From:\n> ");
                if (Int32.TryParse(Console.ReadLine(), out int Rfrom))
                {
                    Console.Write("> To:\n> ");
                    if (Int32.TryParse(Console.ReadLine(), out int Rto))
                    {
                        if (Rto < Rfrom)
                        {
                            Console.Write("> \"From\" value greater than \"To\" value. I will reverse that.\n> -_-");
                            Rto += Rfrom;
                            Rfrom = Rto - Rfrom;
                            Rto -= Rfrom;
                        }

                        int amount;
                        Console.WriteLine(">\n> Results with NO parallelling:");
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        amount = AmountOfPrime(Rfrom, Rto);
                        stopwatch.Stop();
                        Console.WriteLine("> Amount: {0} | Elapsed time: {1}", amount, stopwatch.Elapsed);

                        Console.WriteLine(">\n> Results with THREAD parallelling:");
                        for(int i = 1; i < 9; ++i)
                        {
                            stopwatch.Restart();
                            amount = ThreadChecker(Rfrom, Rto, i);
                            Console.WriteLine("> Amount: {0} | Elapsed time: {1} | Threads: {2}", amount, stopwatch.Elapsed, i);
                        }

                        Console.WriteLine(">\n> Results with TASK parallelling:");
                        for (int i = 1; i < 9; ++i)
                        {
                            stopwatch.Restart();
                            amount = TaskChecker(Rfrom, Rto, i);
                            Console.WriteLine("> Amount: {0} | Elapsed time: {1} | Tasks: {2}", amount, stopwatch.Elapsed, i);
                        }

                        Console.WriteLine(">\n> Results with THREAD POOL parallelling:");
                        for (int i = 1; i < 9; ++i)
                        {
                            stopwatch.Restart();
                            amount = ThreadPoolChecker(Rfrom, Rto, i);
                            Console.WriteLine("> Amount: {0} | Elapsed time: {1} | Threads: {2}", amount, stopwatch.Elapsed, i);
                        }

                        Console.Write("> Try Again? (Y/N):\n> ");
                        a = Console.ReadLine();
                    }
                    else
                    {
                        Console.Write("> Input incorrect. Try Again? (Y/N):\n> ");
                        a = Console.ReadLine();
                    }
                }
                else
                {
                    Console.Write("> Input incorrect. Try Again? (Y/N):\n> ");
                    a = Console.ReadLine();
                }
            } while (a != "N" && a != "n");
        }

        private static bool IsPrime(int num)
        {
            if (num == 0 || num == 1 || num > 2 && num % 2 == 0 || num > 5 && num % 10 == 5)
                return false;

            for (int i = 3; i <= Math.Round(Math.Sqrt(num)); i += 2)
                if (num % i == 0)
                    return false;

            return true;
        }

        private static int AmountOfPrime(int Rfrom, int Rto)
        {
            int amount = 0;
            for (; Rfrom <= Rto; ++Rfrom)
                if (IsPrime(Rfrom))
                    ++amount;

            return amount;
        }

        static Object Locker = new Object();

        private static int ThreadChecker(int Rfrom, int Rto, int ThreadAmount)
        {
            List<Thread> threads = new List<Thread>();
            int amount = 0;
            int step = (Rto - Rfrom + 1) / ThreadAmount;

            for (; ThreadAmount > 0; Rfrom += step, --ThreadAmount)
            {
                int left = Rfrom;
                int right = (ThreadAmount == 1)? Rto : Rfrom + step - 1;

                Thread thread = new Thread(() =>
                {
                    int cur = AmountOfPrime(left, right);
                    lock(Locker)
                    {
                        amount += cur;
                    }
                });
                threads.Add(thread);
                thread.Start();
            }

            foreach (Thread thread in threads)
                thread.Join();

            return amount;
        }

        private static int TaskChecker(int Rfrom, int Rto, int TaskAmount)
        {
            List<Task> tasks = new List<Task>();
            int amount = 0;
            int step = (Rto - Rfrom + 1) / TaskAmount;

            for (; TaskAmount > 0; Rfrom += step, --TaskAmount)
            {
                int left = Rfrom;
                int right = (TaskAmount == 1) ? Rto : Rfrom + step - 1;

                Task task = new Task(() =>
                {
                    int cur = AmountOfPrime(left, right);
                    lock (Locker)
                    {
                        amount += cur;
                    }
                });
                tasks.Add(task);
                task.Start();
            }

            foreach (Task task in tasks)
                task.Wait();

            return amount;
        }

        private static int ThreadPoolChecker(int Rfrom, int Rto, int ThreadAmount)
        {
            ManualResetEvent[] handles = new ManualResetEvent[ThreadAmount];
            int amount = 0;
            int HandleID = 0;
            int step = (Rto - Rfrom + 1) / ThreadAmount;

            for (; ThreadAmount > 0; Rfrom += step, --ThreadAmount)
            {
                int left = Rfrom;
                int right = Rfrom + step - 1;
                int CurHandleID = HandleID;
                handles[HandleID++] = new ManualResetEvent(false);

                ThreadPool.QueueUserWorkItem(o => 
                {
                    int cur = AmountOfPrime(left, right);
                    lock (Locker)
                    {
                        amount += cur;
                    }
                    handles[CurHandleID].Set();
                }
                );
            }

            WaitHandle.WaitAll(handles);
            return amount;
        }
    }
}
