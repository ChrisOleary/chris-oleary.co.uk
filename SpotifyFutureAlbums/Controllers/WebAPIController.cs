using SpotifyFutureAlbums.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SpotifyFutureAlbums
{
    public class WebAPIController : Controller
    {
        public ActionResult Index()
        {

            HttpClient client = new HttpClient();

            async Task RunAsync()
            {
                // Update port # in the following line.
                client.BaseAddress = new Uri("http://localhost:64195/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

            async Task<Product> GetProductAsync(string path)
            {
                Product product = null;
                HttpResponseMessage response = await client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                {
                    product = await response.Content.ReadAsAsync<Product>();
                }
                return product;
            }


                return View();
        }
    }
}