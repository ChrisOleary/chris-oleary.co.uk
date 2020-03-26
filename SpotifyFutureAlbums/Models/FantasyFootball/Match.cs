using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpotifyFutureAlbums.Models
{
    public class Match
    {
        public int id { get; set; }
        public int entry_1_entry { get; set; }
        public string entry_1_name { get; set; }
        public string entry_1_player_name { get; set; }
        public int entry_1_points { get; set; }
        public int entry_1_win { get; set; }
        public int entry_1_draw { get; set; }
        public int entry_1_loss { get; set; }
        public int entry_1_total { get; set; }
        public int entry_2_entry { get; set; }
        public string entry_2_name { get; set; }
        public string entry_2_player_name { get; set; }
        public int entry_2_points { get; set; }
        public int entry_2_win { get; set; }
        public int entry_2_draw { get; set; }
        public int entry_2_loss { get; set; }
        public int entry_2_total { get; set; }
        public bool is_knockout { get; set; }
        public int winner { get; set; }
        public object seed_value { get; set; }
        public int @event { get; set; }
        public object tiebreak { get; set; }
    }
}