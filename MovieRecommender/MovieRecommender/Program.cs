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
        private const string CACHED_MOVIES_DATA_FILEPATH = "../../AppData/cachedMovies.csv";
        static void Main(string[] args)
        {



            List<double[]> examples = new List<double[]>();

            examples.Add(new double[] { 1, 0, 0, 0, 1 });
            examples.Add(new double[] { 0, 1, 0, 0, 2 });
            examples.Add(new double[] { 0, 0, 1, 0, 3 });
            examples.Add(new double[] { 0, 0, 0, 1, 4 });
            examples.Add(new double[] { 0, 0, 1, 1, 5 });

            List<int> attributes = new List<int>();
            attributes.Add(0);
            attributes.Add(1);
            attributes.Add(2);
            attributes.Add(3);
            attributes.Add(4);

            DecisionTree tree = new DecisionTree();
            tree.InitiateTree(examples, attributes);


            int a = 0;
            int b = 1;
            a = b;








            var evaluations = LoadEvaluations();
			var emptyEvaluations = LoadEmptyEvaluations();

            if (args.Contains("--cacheMovies"))
            {
                Console.WriteLine("Will cache movies data.");
                CacheAndSaveMovies(evaluations);
                Console.WriteLine("Cached movies data stored in " + CACHED_MOVIES_DATA_FILEPATH + ". Program will now exit...");
                return;
            }

            var cachedMovies = LoadCachedMovies();
            var personIds = LoadPersonIds();

            foreach (var personId in personIds)
            {
                var userEvaluations = evaluations.Where(e => e.PersonId == personId).ToList();
                var evaluator = new KNearestNeighboursEvaluator(userEvaluations, cachedMovies);
                var emptyUserEvaluations = emptyEvaluations.Where(e => e.PersonId == personId).ToList();//userEvaluations.Where(e => e.IsEmpty()).ToList();

                foreach (var singleEmptyEvaluation in emptyUserEvaluations)
                {
                    var notEvaluatedMovie = cachedMovies.SingleOrDefault(cm => cm.MovieId == singleEmptyEvaluation.MovieId);
                    /*singleEmptyEvaluation.Grade*/int grade = evaluator.PredictGrade(cachedMovies.Where(m => m.MovieId == singleEmptyEvaluation.MovieId).FirstOrDefault());

                    emptyEvaluations.Single(e => e.Id == singleEmptyEvaluation.Id).Grade = grade;
                }
            }
            generateCsv(emptyEvaluations);
        }

        public static void generateCsv(List<Evaluation> evaluations) {
            StringBuilder strBuilder = new StringBuilder();
            foreach (var evaluation in evaluations)
            {
                strBuilder.AppendLine(evaluation.Id + ";" + evaluation.PersonId + ";" + evaluation.MovieId + ";" + evaluation.Grade);
            }
            File.WriteAllText("submissionTEST.csv", strBuilder.ToString());
        }
        
        private static void CacheAndSaveMovies(List<Evaluation> evaluations)
        {
            var cachedMoviesIds = new List<int>();
            var client = new TMDbClient(ConfigurationManager.AppSettings["TMDbAPIKey"]);
            var csvBuilder = new StringBuilder("MovieId;Budget;DirectorId;MainLanguageId;MainActorId;Popularity;VoteAverage;Year\r\n");

            foreach (var singleEvaluation in evaluations)
            {
                if (cachedMoviesIds.All(m => m != singleEvaluation.MovieId))
                {
                    var movieToCache = client.GetMovie(singleEvaluation.MovieId, MovieMethods.Credits | MovieMethods.Keywords | MovieMethods.Reviews | MovieMethods.Translations);
                    if (movieToCache.Id != 0) {
                    var cachedMovie = new SimpleMovie(movieToCache);
                        Console.WriteLine(cachedMovie.Title);
                        cachedMoviesIds.Add(cachedMovie.MovieId);
                        csvBuilder.AppendLine(cachedMovie.ToCsvLine());
                    }
                }
            }

            Console.WriteLine("Saving CSV");
            File.WriteAllText(CACHED_MOVIES_DATA_FILEPATH, csvBuilder.ToString());

        }      

        private static List<SimpleMovie> LoadCachedMovies()
        {
            var reader = new StreamReader(File.OpenRead(CACHED_MOVIES_DATA_FILEPATH));
            var simpleMovies = new List<SimpleMovie>();

            reader.ReadLine();//skipping the column names
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                simpleMovies.Add(new SimpleMovie(line));
            }

            return simpleMovies;
        }

        private static List<int> LoadPersonIds()
        {
            var reader = new StreamReader(File.OpenRead("../../AppData/train.csv"));
            var personIds = new List<int>();

            //reader.ReadLine();//skipping the column names
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                personIds.Add(Convert.ToInt32(values[1]));
            }

            personIds = personIds.Distinct().OrderBy(x => x).ToList();

            return personIds;
        }

        private static List<Evaluation> LoadEvaluations()
        {
            var reader = new StreamReader(File.OpenRead("../../AppData/train.csv"));
            var evaluations = new List<Evaluation>();

            //reader.ReadLine();//skipping the column names
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

        private static List<Evaluation> LoadEmptyEvaluations() {
            var reader = new StreamReader(File.OpenRead("../../AppData/task.csv"));
            var emptyEvaluations = new List<Evaluation>();

            //reader.ReadLine();//skipping the column names
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                var evaluation = new Evaluation()
                {
                    Id = Convert.ToInt32(values[0]),
                    PersonId = Convert.ToInt32(values[1]),
                    MovieId = Convert.ToInt32(values[2]),
                    Grade = 0
                };
                emptyEvaluations.Add(evaluation);
            }

            return emptyEvaluations;
        }
    }
}
