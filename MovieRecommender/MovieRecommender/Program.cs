using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MovieRecommender
{
    class Program
    {
        private static List<SimpleMovie> CachedMovies = new List<SimpleMovie>();

        static void Main(string[] args)
        {
            var client = new TMDbClient(ConfigurationManager.AppSettings["TMDbAPIKey"]);

            var evaluations = LoadEvaluations();
            var personIds = LoadPersonIds();

            foreach (var personId in personIds)
            {
                var userEvaluations = evaluations.Where(e => e.PersonId == personId).ToList();
                var emptyUserEvaluations = userEvaluations.Where(e => e.IsEmpty()).ToList();
                var movies = new List<SimpleMovie>();
                var parameters = new double[3] { 1, 1, 1 };
                var trainingSet = BuildTrainingSetForUser(client, userEvaluations);
                var trainer = new Trainer
                {
                    trainSetElements = trainingSet.ToArray(),
                    parameters = parameters.ToArray()
                };

                trainer.train();

                //TODO
                foreach (var singleEmptyEvaluation in emptyUserEvaluations)
                {
                    var notEvaluatedMovie = FetchMovie(client, singleEmptyEvaluation.MovieId);
                    if (notEvaluatedMovie != null)
                    {
                        singleEmptyEvaluation.Grade = FastRoundToInteger(trainer.polynomialValue(notEvaluatedMovie.ToVector(),
                            trainer.parameters));
                    }
                    else
                    {
                        singleEmptyEvaluation.Grade = -1;
                    }
                    Console.WriteLine(singleEmptyEvaluation.ToString());
                }
            }

        }

        private static int FastRoundToInteger(double input)
        {
            return (int)(input + 0.5);
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

        private static SimpleMovie FetchMovie(TMDbClient client, int movieId)
        {
            var result = CachedMovies.SingleOrDefault(cm => cm.MovieId == movieId);

            if (result == null)
            {
                var tmdbMovie = client.GetMovie(movieId,
                    MovieMethods.Credits | MovieMethods.Keywords | MovieMethods.Reviews);

                if (tmdbMovie.Id > 0)
                {
                    result = new SimpleMovie(tmdbMovie);
                    CachedMovies.Add(result);
                }
            }

            return result;
        }

        public static List<double[]> BuildTrainingSetForUser(TMDbClient client, List<Evaluation> userEvaluations)
        {
            var trainingSet = new List<double[]>();

            foreach (var singleUserEvaluation in userEvaluations)
            {
                var movie = FetchMovie(client, singleUserEvaluation.MovieId);

                if (movie != null)
                {
                    double[] vector = movie.ToVector();
                    double[] singleSet = new double[vector.Length + 1];

                    for (int i = 0; i < vector.Length; i++)
                    {
                        singleSet[i] = vector[i];
                    }

                    singleSet[singleSet.Length - 1] = (double)singleUserEvaluation.Grade.Value;
                    trainingSet.Add(singleSet);
                }
            }

            return trainingSet;
        }
    }
}
