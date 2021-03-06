﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using SpotifyFutureAlbums.Models;
using SpotifyFutureAlbums.ViewModels;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace SpotifyFutureAlbums.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            //public FF stats
            var FootballObject = await FantasyFootball<FootballObject>();

           // Authenticated FF stats
            var FFstats = await GetFFStats<MyTeamRootObject>();

            //Always Sunny
            var AlwaysSunnyQuote = await GetAlwaysSunnyQuote();

            //Weather Api
            //var WeatherDetail = new WeatherDetail(city);

            //Spotify
            //var spotify = new GetAccessToken();

            // TODO XBOX API

            return View(new AllAPIDetails
            {
                 Football = FootballObject,
                 MyTeamRootObject = FFstats,
                 AlwaysSunny = AlwaysSunnyQuote
                 //Weather = WeatherDetail
                //Spotify = spotify
            }
            );
        }

       // Webscrapper
       [HttpPost]
        public string GetUrlSource(string url)
        {
            var ws = new Webscaper();
            return ws.GetUrlSource(url);
            
        }

        //Fantasy Football Authenticated API
        [HttpPost]
        static async Task<MyTeamRootObject> GetFFStats<MyTeamRootObject>()
        {
            //TODO: create POST method to retrieve tokens on initial request.
            //Hardcoded for testing
            var csrftoken = GetFFAuthTokensAsync(); // "m8KFRIjVvN17OGfwRvlXc8GRLNiFGnlQgX3uWzpxSGTQGRYO62dZXiOi3AKSR5LE";
            var pl_profile = "eyJzIjogIld6RXNNVE16TWpnek1EVmQ6MWs2QUVsOmNnSEQzazdUcjVyeElyb2w1NTNwZDJMOW1SZyIsICJ1IjogeyJpZCI6IDEzMzI4MzA1LCAiZm4iOiAiQ2hyaXMiLCAibG4iOiAiTydsZWFyeSIsICJmYyI6IG51bGx9fQ==";


            // this url requires the authentication
            var uri = "https://fantasy.premierleague.com/api/my-team/186809";
            var cookiecontainer = new CookieContainer();

            // Gets the cookie container used to store server cookies by the handler
            var handler = new HttpClientHandler();
            handler.CookieContainer = cookiecontainer;
            //cookiecontainer.Add(new Uri(uri), new Cookie("csrftoken", csrftoken));
            //cookiecontainer.Add(new Uri(uri), new Cookie("pl_profile", pl_profile));
            MyTeamRootObject DeserializeObject = default;
            try
            {
                var client = new HttpClient(handler);
                var result = await client.GetStringAsync(uri);
                var errors = new List<string>();

                // revised way as previous was falling over on null data being passed in
                // previous: var DeserializeObject = JsonConvert.DeserializeObject<MyTeamRootObject>(result);
                DeserializeObject = JsonConvert.DeserializeObject<MyTeamRootObject>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs earg)
                    {
                        errors.Add(earg.ErrorContext.Member.ToString());
                        earg.ErrorContext.Handled = true;
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return DeserializeObject;
        }

        [HttpPost]
        static async Task<Cookie> GetFFAuthTokensAsync()
        {
            //CookieContainer cookies = new CookieContainer();
            //HttpClientHandler handler = new HttpClientHandler();
            //handler.CookieContainer = cookies;

            //HttpClient client2 = new HttpClient(handler);
            //HttpResponseMessage response = client2.GetAsync("http://google.com").Result;

            //Uri uri = new Uri("http://google.com");
            //IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
            //foreach (Cookie cookie in responseCookies)
            //    Console.WriteLine(cookie.Name + ": " + cookie.Value);

            //return null;

                var client = new HttpClient();
                client.BaseAddress = new Uri("https://users.premierleague.com/accounts/login/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //setup login data
                var username = "chris_oleary@hotmail.co.uk";
                var password = "99Vs3C!ND4rpzRQ";
                var formContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type","password"),
                    new KeyValuePair<string, string>("username", username),
                    new KeyValuePair<string, string>("grant_type", password),
                });
            // send request
                HttpClientHandler handler = new HttpClientHandler();
                CookieContainer cookies = new CookieContainer();
                handler.CookieContainer = cookies;
                 HttpResponseMessage responseMessage = await client.PostAsync(client.BaseAddress, formContent);
                Uri uri = new Uri("https://users.premierleague.com/accounts/login/");
                IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
                foreach (Cookie cookie in responseCookies)
                    Console.WriteLine(cookie.Name + ": " + cookie.Value);

            return null;

        }

        //Fantasy Football Public API
        [HttpGet]
        static async Task<T> FantasyFootball<T>()
        { 
            string url = "https://fantasy.premierleague.com/api/entry/892863/";
            var client = new HttpClient();
            var result = await client.GetStringAsync(url);

            var errors = new List<string>();
            var DeserializeObject = JsonConvert.DeserializeObject<T>(result, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Include,
                    Error = delegate (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs earg)
                    {
                        errors.Add(earg.ErrorContext.Member.ToString());
                        earg.ErrorContext.Handled = true;
                    }
                }
            );

            return DeserializeObject;

        }

        //Always Sunny AJAX Reload button in View
        public async Task<PartialViewResult> GetQuote() {

            var quote = await GetAlwaysSunnyQuote();
            return PartialView("_AlwaysSunny", quote);
        }

        //Always Sunny Public API
        [HttpGet]
        private async Task<AlwaysSunnyObject> GetAlwaysSunnyQuote()
        {
            string url = "http://sunnyquotes.net/q.php?random";
            var httpclient = new HttpClient();
            var result = await httpclient.GetStringAsync(url);
            var DeserializeObject = JsonConvert.DeserializeObject<AlwaysSunnyObject>(result);

            return DeserializeObject;
        }

        //OpenWeather
        //TODO
        [HttpPost]
        public string WeatherDetail(string City)
        {

            //Assign API KEY which received from OPENWEATHERMAP.ORG  
            string appId = "d8c2be690a8086d8dd04a6cf4e6df6f5";

            //API path with CITY parameter and other parameters.  
            string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&cnt=1&APPID={1}", City, appId);

            var jsonstring = "";

            using (WebClient client = new WebClient())
            {
                try
                {
                    string json = client.DownloadString(url);

                    //Converting to OBJECT from JSON string.  
                    WeatherRootObject weatherInfo = (new JavaScriptSerializer()).Deserialize<WeatherRootObject>(json);

                    //Special VIEWMODEL design to send only required fields not all fields which received from   
                    //www.openweathermap.org api  
                    ResultViewModel rslt = new ResultViewModel();

                    rslt.Country = weatherInfo.sys.country;
                    rslt.City = weatherInfo.name;
                    rslt.Lat = Convert.ToString(weatherInfo.coord.lat);
                    rslt.Lon = Convert.ToString(weatherInfo.coord.lon);
                    rslt.Description = weatherInfo.weather[0].description;
                    rslt.Humidity = Convert.ToString(weatherInfo.main.humidity);
                    rslt.Temp = Convert.ToString(weatherInfo.main.temp);
                    rslt.TempFeelsLike = Convert.ToString(weatherInfo.main.feels_like);
                    rslt.TempMax = Convert.ToString(weatherInfo.main.temp_max);
                    rslt.TempMin = Convert.ToString(weatherInfo.main.temp_min);
                    rslt.WeatherIcon = weatherInfo.weather[0].icon;

                    //Converting OBJECT to JSON String   
                    jsonstring = new JavaScriptSerializer().Serialize(rslt);
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.InnerException);
                }
 
                //Return JSON string.  
                return jsonstring;
            }
        }

        //Spotify
        [HttpGet]
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