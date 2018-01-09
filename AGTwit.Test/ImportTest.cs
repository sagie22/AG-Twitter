using System.IO;
using AGTwit.Business;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AGTwit.Test
{
    [TestClass]
    public class ImportTest
    {
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void LoadDataNotFound()

        {
            var data = new Twitterer();
            data.LoadFromFile("junk.txt", "i don't exist.txt");
        }

        [TestMethod]
        public void LoadDataValidFilesAsPerSample()

        {
            var data = new Twitterer();
            data.LoadFromFile("user.txt", "tweet.txt");

            Assert.AreEqual(3, data.Users.Count);
            var userAlan = data.Users.Find(x => x.Name == "Alan");
            Assert.AreEqual(2, userAlan.Tweets.Count);
            var userWard = data.Users.Find(x => x.Name == "Ward");
            Assert.AreEqual(1, userWard.Tweets.Count);
            var userMartin = data.Users.Find(x => x.Name == "Martin");
            Assert.AreEqual(0, userMartin.Tweets.Count);

            var feed = data.Feed();
            Assert.AreEqual(8, feed.Count);
        }

        [TestMethod]
        public void LoadDataValidFilesAsPerSample2()

        {
            var data = new Twitterer();
            data.LoadFromFile("userSample2.txt", "tweetSample2.txt");

            Assert.AreEqual(5, data.Users.Count);
            var userAlan = data.Users.Find(x => x.Name == "Alan");
            Assert.AreEqual(2, userAlan.Tweets.Count);
            var userWard = data.Users.Find(x => x.Name == "Ward");
            Assert.AreEqual(1, userWard.Tweets.Count);
            var userMartin = data.Users.Find(x => x.Name == "Martin");
            Assert.AreEqual(0, userMartin.Tweets.Count);

            var userSagie = data.Users.Find(x => x.Name == "Sagie");
            Assert.AreEqual(1, userSagie.Tweets.Count);
            Assert.AreEqual("\t@Sagie: more <brackets> are not good Sagie>", userSagie.Tweets[0].Message);

            var feed = data.Feed();
            Assert.AreEqual(13, feed.Count);
        }
    }
}