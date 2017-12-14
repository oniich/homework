using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Async_Parser
{
  public class Parser
  {
    public async Task<Page[]> Parse(Page[] all_pages)
    {
      all_pages[0].html_code = await all_pages[0].Get_html_code(all_pages[0].url);

      for (int i = 0; i < 2; ++i)
      {
        foreach (var page in all_pages)
        {
          var next_pages = await Get_pages(page);
          all_pages = all_pages.Concat(next_pages).ToArray();
        }
      }
      return all_pages;
    }

    public async Task<Page[]> Get_pages(Page input)
    {
      Regex reg = new Regex(@"<a.*? href=""(?<url>https?[\w\.:?&-_=#/]*)""+?");

      var urls = reg.Matches(input.html_code);
      var pages = new Page[urls.Count];

      for (var i = 0; i < urls.Count; ++i)
      {
        pages[i] = new Page(urls[i].Groups["url"].Value);
        pages[i].html_code = await pages[i].Get_html_code(pages[i].url);
      }
      return pages;
    }
  }   
}
