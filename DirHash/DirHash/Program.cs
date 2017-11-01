using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DirHash
{
    class Program
    {
        static void Main(string[] args)
        {
            string a;
            do
            {
                string dir;
                Console.Write("> Write path to the directory:\n> ");
                dir = Console.ReadLine();
                while (!Directory.Exists(dir))
                {
                    Console.Write("> No such directory. Try again:\n> ");
                    dir = Console.ReadLine();
                }

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var hash = GetDirHash(dir);
                stopwatch.Stop();

                Console.WriteLine("> Directory MD5 hash: {0} | Elapsed time: {1}", BitConverter.ToString(hash).Replace("-", "").ToLower(), stopwatch.Elapsed);
                Console.Write("> Try again? (Y/N):");
                a = Console.ReadLine();

            } while (a != "N" && a != "n");
        }

        private static byte[] GetHash(FileStream FilePath)
        {
            var md5 = MD5.Create();
            return md5.ComputeHash(FilePath);
        }

        private static byte[] GetDirHash(string dir)
        {
            var paths = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            List<Byte[]> PathsHash = new List<Byte[]>();
            List<Task<byte[]>> tasks = new List<Task<byte[]>>();

            foreach (var path in paths)
            {
                var NewPath = File.OpenRead(path);
                Task<byte[]> task = Task.Run(() => GetHash(NewPath));
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            foreach (Task<byte[]> task in tasks)
                PathsHash.Add((task.Result));

            byte[] array = PathsHash.SelectMany(a => a).ToArray();

            var md5 = MD5.Create();
            return md5.ComputeHash(array);
        }
    }
}
