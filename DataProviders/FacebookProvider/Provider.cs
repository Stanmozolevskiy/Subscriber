using DataModels;
using DataProviderInterfaces;
using PuppeteerSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FacebookProvider
{
    public class Provider : IFacebookProvider
    {
        public async Task<Page> GetBrowserPage()
        {
            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions()
            {
                Headless = true,
                ExecutablePath = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"
            });
            return await browser.NewPageAsync();
        }

        public async Task<List<Data>> GetDataFromFacebook(Page page, string subject)
        {

            //Go to the page 
            await page.GoToAsync($"https://www.facebook.com/marketplace/112739992073844/search?sortBy=best_match&query=1%20bedroom%20apartment%20rent&exact=false");
                
            //set the distance for future
            //await page.WaitForNetworkIdleAsync();
            //await page.Mouse.ClickAsync(100, 150);
            //await page.WaitForNetworkIdleAsync();
            //await page.Mouse.ClickAsync(200, 250);
            //await page.WaitForNetworkIdleAsync();
            //await page.Keyboard.TypeAsync("10");
            //await page.WaitForNetworkIdleAsync();
            //await page.Keyboard.PressAsync("Enter");
            //await page.WaitForNetworkIdleAsync();
            //await page.Mouse.WheelAsync(0, 1000);
            //await page.WaitForNetworkIdleAsync();
            //await page.Mouse.ClickAsync(600, 500);

            //Scroll throught the page and wait to load, take a screenshot
            for (int i = 0; i < 2; i++)
            {
                await page.Mouse.WheelAsync(0, 1000);
                await page.WaitForNetworkIdleAsync();

                await page.ScreenshotAsync($"{System.IO.Directory.CreateDirectory("Attachments").Name}/marketplaceimage.jpg");
            }

            List <Data> DataList = new List<Data>();

            string jsCode = @"() => {
                const selectors = Array.from(document.querySelectorAll('a'));
                return selectors.map(t=> {return { body: t.innerHTML, url: t.href}})
                                .filter(data=> data.url.includes('https://www.facebook.com/marketplace/item'));
                }";

            foreach (Data result in await page.EvaluateFunctionAsync<Data[]>(jsCode))
                DataList.Add(result);

            await page.CloseAsync();
            return DataList;
        }

    }
}
