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
        public void LoadDataValidFiles()

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
    }
}