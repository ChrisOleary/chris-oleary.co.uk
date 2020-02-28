using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpotifyFutureAlbums.Models
{
    public class Status
    {
        public int qualification_event { get; set; }
        public int qualification_numbers { get; set; }
        public int qualification_rank { get; set; }
        public string qualification_state { get; set; }
    }
}