using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AGTwit.Entities;

namespace AGTwit.Business
{
    public class Twitterer
    {
        public List<Entities.Twitterer> Users = new List<Entities.Twitterer>();

        public void LoadFromFile(string userDataFile, string tweetDataFile)
        {
            //AGTwit.data.tw
            if (!File.Exists(userDataFile) || !File.Exists(tweetDataFile))
                throw new FileNotFoundException("File(s) not found: " + userDataFile + " or " + tweetDataFile);
            try

            {
                using (var srUser = new StreamReader(userDataFile))
                {
                    LoadUsers(srUser);
                }
                using (var srTweets = new StreamReader(tweetDataFile))
                {
                    LoadTweets(srTweets);
                }
            }

            catch (Exception ex)

            {
                throw new Exception($"Error processing data from files. Error:{ex.Message}");
            }
        }

        private void LoadUsers(StreamReader srUser)
        {
            var userDataRaw = srUser.ReadToEnd();
            var lines = userDataRaw.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.RemoveEmptyEntries
            ).ToList();
            lines = lines.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            foreach (var line in lines)
            {
                var fileLineData = line.Split(
                    new[] {" follows "},
                    StringSplitOptions.None
                ).ToList();
                if (fileLineData.Count != 2)
                    throw new Exception($"Invalid data in line [{line}]");

                var user = Users.FirstOrDefault(c => c.Name == fileLineData[0]);
                var follows = fileLineData[1].Split(
                    new[] {","},
                    StringSplitOptions.None
                ).ToList();

                follows = follows.Select(s => s.Trim()).ToList();

                if (user != null)
                {
                    user.Follows.AddRange(follows);
                }
                else
                {
                    user = new Entities.Twitterer
                    {
                        Name = fileLineData[0],
                        Follows = follows,
                        Tweets = new List<Tweet>()
                    };
                    Users.Add(user);
                }
                user.Follows = user.Follows.Distinct().ToList();

                foreach (var follower in follows)
                {
                    var followUser = Users.FirstOrDefault(c => c.Name == follower);
                    if (followUser != null) continue;
                    followUser = new Entities.Twitterer
                    {
                        Name = follower,
                        Follows = new List<string>(),
                        Tweets = new List<Tweet>()
                    };
                    Users.Add(followUser);
                }
            }
        }

        private void LoadTweets(StreamReader srTweets)
        {
            var tweetDataRaw = srTweets.ReadToEnd();
            var lines = tweetDataRaw.Split(
                new[] {"\r\n", "\r", "\n"},
                StringSplitOptions.RemoveEmptyEntries
            ).ToList();


            lines = lines.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
            var sequence = 0;
            foreach (var line in lines)
            {
                sequence++;
                var tweetLine = line.Split(
                    new[] {">"},
                    StringSplitOptions.RemoveEmptyEntries
                ).ToList();
                var user = Users.FirstOrDefault(c => c.Name == tweetLine[0]);
                var tweet = tweetLine[1];

                if (user != null)
                    user.Tweets.Add(new Tweet
                    {
                        Message = "\t" + $"@{tweetLine[0]}:" + tweet,
                        Sequence = sequence
                    });
                else
                    throw new Exception($"Unknown user {tweetLine[0]}");
            }
        }


        public List<string> Feed()
        {
            var feedData = new List<string>();
            foreach (var user in Users.OrderBy(x => x.Name))
            {
                feedData.Add(user.Name);

                feedData.AddRange(GetUserFeed(user));
            }
            return feedData;
        }

        private IEnumerable<string> GetUserFeed(Entities.Twitterer user)
        {
            var feedData = new List<Tweet>();
            feedData.AddRange(user.Tweets);
            foreach (var follows in user.Follows)
            {
                var followUser = Users.FirstOrDefault(c => c.Name == follows);

                if (followUser != null) feedData.AddRange(followUser.Tweets);
            }
            var sorted = feedData.OrderBy(o => o.Sequence).ToList();
            return sorted.Select(x => x.Message).ToList();
        }
    }
}