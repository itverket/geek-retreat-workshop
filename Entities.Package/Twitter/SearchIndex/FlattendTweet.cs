using System;

namespace Entities.Twitter.SearchIndex
{
    public class FlattendTweet
    {
        public string TweetId { get; set; }
        public string Username { get; set; }
        public string TweetMessage { get; set; }
        public DateTime? CreatedAt { get; set; }

        public override string ToString()
        {
            return $"ID: {TweetId}\tUser: {Username}\tDate: {CreatedAt}\nTweet:{TweetMessage}";
        }
    }
}