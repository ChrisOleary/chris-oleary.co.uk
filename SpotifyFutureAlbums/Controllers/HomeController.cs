﻿using System;
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
using PagedList;

namespace SpotifyFutureAlbums.Controllers
{

    public class HomeController : Controller
    {
        private static SpotifyWebAPI _spotify;

        public async Task<ActionResult> Index()
        {

            //FANTASY FOOTBALL API
            var fantasyFootball = FantasyFootball();

            var myToken = GetAccessToken();
            _spotify = new SpotifyWebAPI()
            {
                AccessToken = myToken,
                TokenType = "Bearer"
            };
            var savedTracks = await _spotify.GetSavedTracksAsync(50);
            return View(savedTracks);
        }

        public string FantasyFootball()
        {
            string url = "https://users.premierleague.com/accounts/login/";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Accept = "application/json";
           

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            string json = "";

          

            return json;
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
                    .ContinueWith((taskwithmsg) => {
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