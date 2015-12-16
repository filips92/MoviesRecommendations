using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;

namespace MovieRecommender
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TMDbClient(ConfigurationManager.AppSettings["TMDbAPIKey"]);
            var test = new SimpleMovie(client.GetMovie(47964, MovieMethods.Credits | MovieMethods.Keywords | MovieMethods.Reviews));
            int a = 2;
        }
    }
}
