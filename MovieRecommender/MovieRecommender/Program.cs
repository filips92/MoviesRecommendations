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
            reader.ReadLine();//skipping the column names
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                Evaluation evaluation;
                evaluation = new Evaluation(){
                    Id = Convert.ToInt32(values[0]),
                    PersonId = Convert.ToInt32(values[1]),
                    MovieId = Convert.ToInt32(values[2]),
                    Grade = (values[values.Count() - 1] == "NULL") ? 0 : Convert.ToInt32(values[3])
                };
                evaluations.Add(evaluation);                 
            }

            reader = new StreamReader(File.OpenRead("../../AppData/people.csv"));

            List<int> people = new List<int>();
            reader.ReadLine();//skipping the column names
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');
                people.Add( Convert.ToInt32(values[0]) );
            }

            foreach (int personId in people) {
                List<Evaluation> userEvaluations = evaluations.Where(e => e.PersonId == personId).ToList();

                List<SimpleMovie> movies = new List<SimpleMovie>();
                List<double[]> trainingSet = new List<double[]>();
                double[] parameters = new double[4] { 1, 1, 1, 1};
                foreach (Evaluation evaluation in userEvaluations)
                {
                    SimpleMovie movie = new SimpleMovie(client.GetMovie(evaluation.MovieId, MovieMethods.Credits | MovieMethods.Keywords | MovieMethods.Reviews));
                    if (movie.MovieId != 0 && evaluation.Grade > 0)
                    {
                        movies.Add(movie);
                        double[] vector = movie.ToVector();
                        double[] singleSet = new double[3];
                        for (int i = 0; i < vector.Length; i++)
                        {
                            singleSet[i] = vector[i];
                        }
                        singleSet[singleSet.Length - 1] = (double)evaluation.Grade;
                        trainingSet.Add(singleSet);
                    }
                }
                
                Trainer trainer = new Trainer();
                trainer.trainSetElements = trainingSet.ToArray();
                trainer.parameters = parameters.ToArray();

                trainer.train();

                List<Evaluation> emptyEvaluations = evaluations.Where(e => e.Grade == 0).ToList();

                //TODO
            }
           
        }
    }
}
