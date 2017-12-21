using System;
using System.Threading;
using System.Threading.Tasks;

namespace RegisterSnapshot
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var rnd = new Random();
      //2 for Writers and 1 for Reader
      var tasks = new Task[3];
      var regSnap = new RegisterSnapshot();

      for (var i = 0; i < 16; ++i)
      {
        var pid = i % 2;
        var value = rnd.Next(1024);

        //Starting task for updating register value
        tasks[pid] = Task.Run(() =>
        {
          Console.WriteLine("Update value ({0}) of register #{1}", value, pid);
          regSnap.Update(pid, value);
        });

        if (i % 3 == 0)
        {
          //Checking when scan started and when we get actual snapshot
          tasks[2] = Task.Run(() =>
          {
            System.Console.WriteLine("Start Scan()");
            var shapshot = regSnap.Scan(pid);
            Console.WriteLine("Snapshot: ({0}, {1})", shapshot[0], shapshot[1]);
          });
        }
      }
      Task.WaitAll(tasks);
    }
  }
}