using System;
using System.Net;
using System.Threading.Tasks;

namespace Async_Parser
{
  public class Page
  {
    public string url;
    public string html_code;

    public Page(string input)
    {
      url = input;
      html_code = "";
    }

    public async Task<string> Get_html_code(string input)
    {
      using (WebClient client = new WebClient())
      {
        string code = "";
        try
        {
          code = await client.DownloadStringTaskAsync(input);
        }
        catch (Exception e)
        {
          Console.WriteLine("{0} - {1}", input, e.Message);
        }
        return code;
      }
    }
  }
}
