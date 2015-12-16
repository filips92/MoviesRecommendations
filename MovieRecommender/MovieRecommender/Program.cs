using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using System.IO;

namespace MovieRecommender
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new TMDbClient(ConfigurationManager.AppSettings["TMDbAPIKey"]);
            var test = new SimpleMovie(client.GetMovie(47964, MovieMethods.Credits | MovieMethods.Keywords | MovieMethods.Reviews));
            int a = 2;

            var reader = new StreamReader(File.OpenRead("../../AppData/evaluations.csv"));

            List<Evaluation> evaluations = new List<Evaluation>();
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                double[] doubleArray = new double[values.Count()];
                Evaluation evaluation;
                if (values[values.Count() - 1] != "NULL") {
                    evaluation = new Evaluation(){
                        Id = Convert.ToInt32(values[0]),
                        PersonId = Convert.ToInt32(values[1]),
                        MovieId = Convert.ToInt32(values[2]),
                        Grade = Convert.ToInt32(values[3])
                    };
                    evaluations.Add(evaluation);
                }                   
            }
           
        }
    }
}
