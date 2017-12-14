using System;

namespace Async_Parser
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.Write("URL: ");
      string url = Console.ReadLine();

      Parser parser = new Parser();

      var all_pages = parser.Parse(new[] { new Page(url) }).Result;

      foreach (var page in all_pages)
        Console.WriteLine("{0} ({1})", page.url, page.html_code.Length);

      Console.WriteLine("If you're reading this, you have dozen of patiences.");
      Console.ReadKey();
    }
  }   
}
