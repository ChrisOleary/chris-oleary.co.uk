using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using SpotifyAPI.Web; //Base Namespace
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums; //Enums
using SpotifyAPI.Web.Models; //Models for the JSON-responses
using SpotifyFutureAlbums.Models;
using SpotifyFutureAlbums.ViewModels;
using PagedList;

namespace SpotifyFutureAlbums.Controllers
{

    public class HomeController : Controller
    {

        public async Task<ActionResult> Index()
        {
            // <FootballObject> is used as an alias for <T> in FantasyFootball()
            var FootballObject = await FantasyFootball<FootballObject>();

            var AlwaysSunnyQuote = await GetAlwaysSunnyQuote<AlwaysSunnyObject>();


            //return the view model and assign each API
            return View(new AllAPIDetails
            {
                AlwaysSunny = AlwaysSunnyQuote,
                Football = FootballObject
            }

                );
        }

        [HttpGet]
        static async Task<T> FantasyFootball<T>()
        {

            string url = "https://fantasy.premierleague.com/api/entry/186809/";
            var client = new HttpClient();
            var result = await client.GetStringAsync(url);
            var DeserializeObject = JsonConvert.DeserializeObject<T>(result);

            return DeserializeObject;

        }

        [HttpGet]
        static async Task<T> GetAlwaysSunnyQuote<T>()
        {
            string url = "http://sunnyquotes.net/q.php?random";
            var httpclient = new HttpClient();
            var result = await httpclient.GetStringAsync(url);
            var DeserializeObject = JsonConvert.DeserializeObject<T>(result);

            return DeserializeObject;
        }

        [HttpPost]
        public async Task<T> PostFantasyFootballLogins<T>()
        {

            var model = new FFPostLoginDetails()
            {
                login = "chris_oleary@hotmail.co.uk",
                password = "99Vs3C!ND4rpzRQ",
                redirect_uri = "https://fantasy.premierleague.com/a/login",
                app = "plfpl-web"
            };
               
            var httpclient = new HttpClient();
            var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var result = await httpclient.PostAsync("https://fantasy.premierleague.com/a/login", stringContent);

            //TODO THIS
            return result.StatusCode;
        }

        public string GetAccessToken()
        {
            SpotifyTokenModel token = new SpotifyTokenModel();
            string url5 = "https://accounts.spotify.com/api/token";

            var encode_clientid_clientsecret = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", "2de5398af2154649b572315de55900e3", "467401d203114d0bb49fcbf22909998f")));

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url5);

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Accept = "application/json";
            webRequest.Headers.Add("Authorization: Basic " + encode_clientid_clientsecret);

            var request = ("grant_type=client_credentials");
            byte[] req_bytes = Encoding.ASCII.GetBytes(request);
            webRequest.ContentLength = req_bytes.Length;

            Stream strm = webRequest.GetRequestStream();
            strm.Write(req_bytes, 0, req_bytes.Length);
            strm.Close();

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            String json = "";
            using (Stream respStr = resp.GetResponseStream())
            {
                using (StreamReader rdr = new StreamReader(respStr, Encoding.UTF8))
                {
                    //should get back a string i can then turn to json and parse for accesstoken
                    json = rdr.ReadToEnd();
                    rdr.Close();
                }
            }
            token = JsonConvert.DeserializeObject<SpotifyTokenModel>(json);
            return token.Access_token;
        }


        public string GetTrackInfo(string url)
        {
            string webResponse = string.Empty;
            var myToken = GetAccessToken();
            try
            {
                // Get token for request
                HttpClient hc = new HttpClient();
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Authorization", "Bearer " + myToken);
                // TODO: Had a task cancelled here, too many errors. Find a better way to process this maybe.
                // Same code is in GetTrackFeatures function.
                var task = hc.SendAsync(request)
                    .ContinueWith((taskwithmsg) =>
                    {
                        var response = taskwithmsg.Result;
                        var jsonTask = response.Content.ReadAsStringAsync();
                        webResponse = jsonTask.Result;
                    });
                task.Wait();


            }
            catch (WebException ex)
            {
                Console.WriteLine("Track Request Error: " + ex.Status);
            }
            catch (TaskCanceledException tex)
            {
                Console.WriteLine("Track Request Error: " + tex.Message);
            }
            return webResponse;
        }

    }
}