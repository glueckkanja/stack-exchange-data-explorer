﻿namespace StackExchange.DataExplorer.ViewModel
{
    public class QueryVoting
    {
        public int RevisionId { get; set; }
        public int TotalVotes { get; set; }
        public bool HasVoted { get; set; }
        public bool ReadOnly { get; set; }
    }
}