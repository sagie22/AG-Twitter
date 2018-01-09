using System.Collections.Generic;

namespace AGTwit.Entities
{
    public class Twitterer
    {
        public List<string> Follows;
        public string Name;
        public List<Tweet> Tweets;
    }

    public class Tweet
    {
        public string Message;
        public int Sequence;
    }
}