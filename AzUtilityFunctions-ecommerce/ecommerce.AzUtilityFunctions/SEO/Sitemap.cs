using Boxed.AspNetCore;
using Libraries.ecommerce.Services.Services;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using UtilityFunctions.Data;

namespace ecommerce.AzUtilityFunctions.SEO
{
    public class Sitemap
    {
        private readonly IService<Data.Product> serviceProduct;
        private readonly string urlProduct = "/api/Product/";
        private readonly IService<Data.Page> servicePage;
        private readonly string urlPage = "/api/page/";
        private readonly IService<Data.Category> serviceCategory;
        private readonly string urlCategory = "/api/Category/";
        private readonly IService<Data.Brand> serviceBrand;
        private readonly string urlBrand = "/api/Brand/";


        public Sitemap(IService<Data.Product> _serviceProduct, IService<Data.Page> _servicePage,
               IService<Data.Category> _serviceCategory, IService<Data.Brand> _serviceBrand)
        {
            serviceProduct = _serviceProduct;
            serviceBrand = _serviceBrand;
            serviceCategory = _serviceCategory;
            servicePage = _servicePage;
        }
        [FunctionName("Sitemap")]
        public async Task RunAsync([TimerTrigger("0 */24 * * *")] TimerInfo myTimer,
        [Blob("sitemap", FileAccess.ReadWrite, Connection = "AzureWebJobsStorage")] CloudBlobContainer sitemapContainer,
         ILogger log)
        {
            log.LogInformation($"Sitemap trigger function executed at: {DateTime.Now}");
            try
            {
                var token = await serviceBrand.GetAccessToken();
                var websiteUrl = Debugger.IsAttached ? "https://localhost:44322" : Environment.GetEnvironmentVariable("Website_Live_URL");
                var website_SA_Url = Debugger.IsAttached ? "https://localhost:44322" : Environment.GetEnvironmentVariable("Website_Live_SA_URL");
                await sitemapContainer.CreateIfNotExistsAsync();
                CloudBlockBlob blob = sitemapContainer.GetBlockBlobReference("sitemap.xml");
                blob.Properties.ContentType = "application/xml";
                //Get Products
                var products = await serviceProduct.GetAll(urlProduct, token);
                //Get Pages
                var pages = await servicePage.GetAll(urlPage, token);
                var categories = await serviceCategory.GetAll(urlCategory, token);
                var brands = await serviceBrand.GetAll(urlBrand, token);
                var sitemapNodes = new List<SitemapNode>();

                XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
                XElement root = new XElement(xmlns + "urlset");

                foreach (var item in products)
                {
                    sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.5, Url = $"{websiteUrl}/product/{item.id.ToString()}/{FriendlyUrlHelper.GetFriendlyTitle(item.name)}", Frequency = SitemapFrequency.Daily });
                    sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.5, Url = $"{website_SA_Url}/product/{item.id.ToString()}/{FriendlyUrlHelper.GetFriendlyTitle(item.name)}", Frequency = SitemapFrequency.Daily });
                }

                foreach (var item in pages)
                {
                    sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.1, Url = $"{websiteUrl}/Home/Page/{item.id.ToString()}", Frequency = SitemapFrequency.Monthly });
                    sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.1, Url = $"{website_SA_Url}/Home/Page/{item.id.ToString()}", Frequency = SitemapFrequency.Monthly });
                }

                foreach (var item in categories)
                {
                    sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.5, Url = $"{websiteUrl}/Shop?categoryId={item.id.ToString()}", Frequency = SitemapFrequency.Daily });
                }

                foreach (var item in brands)
                {
                    sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.8, Url = $"{websiteUrl}/Shop?brandid={item.id.ToString()}", Frequency = SitemapFrequency.Daily });
                }


                //Static pages
                sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.2, Url = websiteUrl, Frequency = SitemapFrequency.Daily });
                sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.2, Url = website_SA_Url, Frequency = SitemapFrequency.Daily });
                sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.3, Url = $"{websiteUrl}/Shop", Frequency = SitemapFrequency.Daily });
                sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.3, Url = $"{website_SA_Url}/Shop", Frequency = SitemapFrequency.Daily });
                sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.4, Url = $"{websiteUrl}/Home/FAQ", Frequency = SitemapFrequency.Daily });
                sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.5, Url = $"{websiteUrl}/Home/AboutUs", Frequency = SitemapFrequency.Daily });
                sitemapNodes.Add(new SitemapNode { LastModified = DateTime.UtcNow, Priority = 0.6, Url = $"{websiteUrl}/Home/ContactUs", Frequency = SitemapFrequency.Daily });


                foreach (SitemapNode sitemapNode in sitemapNodes)
                {
                    XElement urlElement = new XElement(
                        xmlns + "url",
                        new XElement(xmlns + "loc", Uri.EscapeUriString(sitemapNode.Url)),
                        sitemapNode.LastModified == null ? null : new XElement(
                            xmlns + "lastmod",
                            sitemapNode.LastModified.Value.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz")),
                        sitemapNode.Frequency == null ? null : new XElement(
                            xmlns + "changefreq",
                            sitemapNode.Frequency.Value.ToString().ToLowerInvariant()),
                        sitemapNode.Priority == null ? null : new XElement(
                            xmlns + "priority",
                            sitemapNode.Priority.Value.ToString("F1", CultureInfo.InvariantCulture)));
                    root.Add(urlElement);
                }
                XDocument document = new XDocument(root);
                await blob.UploadTextAsync(document.ToString());
                log.LogInformation($"Sitemap update at {DateTime.Now}.");
            }
            catch (Exception ex)
            {
                log.LogError("Sitemap Error", ex);
            }
        }
    }
}
