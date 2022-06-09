using DataModels;
using PuppeteerSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataProviderInterfaces
{
    public interface IFacebookProvider
    {
         Task<List<Data>> GetDataFromFacebook(Page page, string subject);
         Task<Page> GetBrowserPage();
    }
   
}
