
using System.Collections.Generic;

namespace VoteMap.Core.Models
{
    public class VoteMapResp
    {
        public int retCode { get; set; } = 0;
        public string retMessage { get; set; } = "";
        public string requestID { get; set; } = "";
    }

    public class VoteMapResp<T> : VoteMapResp
    {
        public T result { get; set; }
    }

    public class ListResult<T>
    {
        public List<T> list { get; set; } = new List<T>();
    }

    public class Legislator
    {
        public string city { get; set; } = "";
        public string town { get; set; } = "";
        public string kmt { get; set; } = "";
        public int kmt_val
        {
            get
            {
                int val = 0;
                int.TryParse(kmt, out val);
                return val;
            }
        }
        public string dpp { get; set; } = "";
        public int dpp_val
        {
            get
            {
                int val = 0;
                int.TryParse(dpp, out val);
                return val;
            }
        }
        public string tpp { get; set; } = "";
        public int tpp_val
        {
            get
            {
                int val = 0;
                int.TryParse(tpp, out val);
                return val;
            }
        }
    }

    public class Mayer
    {
        public string show_city { get; set; } = "";
        public string city
        {
            get
            {
                return show_city;
            }
        }
        public string show_town { get; set; } = "";
        public string town
        {
            get
            {
                return show_town;
            }
        }
        public string candidate_name { get; set; } = "";
        public string political_party { get; set; } = "";
        public string vote_count { get; set; } = "";
        public int vote_count_val
        {
            get
            {
                int val = 0;
                int.TryParse(vote_count, out val);
                return val;
            }
        }
    }

    public class MapMetric
    {
        public string show_city { get; set; } = "";
        public string city
        {
            get
            {
                return show_city;
            }
        }
        public string show_town { get; set; } = "";
        public string town
        {
            get
            {
                return show_town;
            }
        }
        public string candidate_no { get; set; } = "";
        public string candidate_name { get; set; } = "";
        public string political_party { get; set; } = "";
        public string vote_count { get; set; } = "";
        public int vote_count_val
        {
            get
            {
                int val = 0;
                int.TryParse(vote_count, out val);
                return val;
            }
        }
    }

    public class TownView
    {
        public string year { get; set; } = "";
        public string type { get; set; } = "";
    }

    public class PoliticalParty
    {
        public string party_no { get; set; } = "";
        public string party_name { get; set; } = "";
        public string party_code { get; set; } = "";
        public string party_bg_color { get; set; } = "";
        public string party_text_color { get; set; } = "";
    }
}
