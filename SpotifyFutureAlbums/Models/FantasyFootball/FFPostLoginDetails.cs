using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpotifyFutureAlbums.Models
{
    public class FFPostLoginDetails
    {
        public string password { get; set; }
        public string login { get; set; }
        public string redirect_uri { get; set; }
        public string app { get; set; }

    }
}