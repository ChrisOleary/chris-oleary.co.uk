using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpotifyFutureAlbums.Models
{
    public class IndexModel
    {
        public Paging<SavedTrack> SavedTracks;
    }
}