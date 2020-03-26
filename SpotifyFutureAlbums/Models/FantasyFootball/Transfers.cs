using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpotifyFutureAlbums.Models
{
    public class Transfers
    {
        public int Cost { get; set; }
        public string Status { get; set; }
        public int Limit { get; set; }
        public int Made { get; set; }
        public int Bank { get; set; }
        public int Value { get; set; }
    }
}