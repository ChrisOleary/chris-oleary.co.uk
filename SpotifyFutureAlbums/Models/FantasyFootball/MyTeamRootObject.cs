using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpotifyFutureAlbums.Models
{
    public class MyTeamRootObject
    {
        public List<Pick> picks { get; set; }
        public List<Chip> chips { get; set; }
        public Transfers transfers { get; set; }
    }
}