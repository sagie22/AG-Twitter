using System;
using AGTwit.Business;

namespace AGTwitFeed
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var tokens = Console.ReadLine().Split(' ');
            if (tokens.Length != 2)
            {
                Console.WriteLine("Invalid input. Please enter text file names for a user and a tweet file.");
            }
            else
            {
                var users = tokens[0];
                var tweets = tokens[1];
                var twitterer = new Twitterer();
                twitterer.LoadFromFile(users, tweets);
                twitterer.Feed().ForEach(i => Console.WriteLine("{0}\t", i));
            }
        }
    }
}