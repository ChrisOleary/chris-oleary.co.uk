using System;
using System.Net;

namespace SpotifyFutureAlbums
{
    public class Webscaper
    {
        public string GetUrlSource(string url)
        {
            url = url.Substring(0, 4) != "http" ? "http://" + url : url;
            string htmlCode = "";
            using (WebClient client = new WebClient())
            {
                try
                {
                    htmlCode = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            return htmlCode;
        }
    }
}