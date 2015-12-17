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

            var evaluations = LoadEvaluations();
            var personIds = LoadPersonIds();

            foreach (var personId in personIds) {
                var userEvaluations = evaluations.Where(e => e.PersonId == personId).ToList();
                var emptyEvaluations = userEvaluations.Where(e => e.Grade == 0).ToList();
                var movies = new List<SimpleMovie>();
                var trainingSet = new List<double[]>();
                var parameters = new double[3] { 1, 1, 1};

                Trainer trainer;
                
                foreach (var singleUserEvaluation in userEvaluations)
                {
                    var movie = new SimpleMovie(client.GetMovie(singleUserEvaluation.MovieId, MovieMethods.Credits | MovieMethods.Keywords | MovieMethods.Reviews));
                    
                    if (movie.MovieId != 0 && singleUserEvaluation.Grade > 0)
                    {
                        double[] vector = movie.ToVector();
                        double[] singleSet = new double[vector.Length + 1];

                        for (int i = 0; i < vector.Length; i++)
                        {
                            singleSet[i] = vector[i];
                        }
                        
                        singleSet[singleSet.Length - 1] = (double)singleUserEvaluation.Grade.Value;
                        trainingSet.Add(singleSet);
                        movies.Add(movie);
                    }
                }

                trainer = new Trainer
                {
                    trainSetElements = trainingSet.ToArray(),
                    parameters = parameters.ToArray()
                };
                trainer.train();

                //TODO
            }
           
        }

        private static List<int> LoadPersonIds()
        {
            var reader = new StreamReader(File.OpenRead("../../AppData/people.csv"));
            var personIds = new List<int>();
            reader.ReadLine();//skipping the column names
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                personIds.Add(Convert.ToInt32(values[0]));
            }

            return personIds;
        }

        private static List<Evaluation> LoadEvaluations()
        {
            var reader = new StreamReader(File.OpenRead("../../AppData/evaluations.csv"));
            var evaluations = new List<Evaluation>();
            reader.ReadLine();//skipping the column names
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                var evaluation = new Evaluation()
                {
                    Id = Convert.ToInt32(values[0]),
                    PersonId = Convert.ToInt32(values[1]),
                    MovieId = Convert.ToInt32(values[2]),
                    Grade = (values[3] == "NULL") ? 0 : Convert.ToInt32(values[3])
                };
                evaluations.Add(evaluation);
            }

            return evaluations;
        }
    }
}
