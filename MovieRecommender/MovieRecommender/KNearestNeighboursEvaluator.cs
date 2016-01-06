using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender
{
    public class KNearestNeighboursEvaluator : EvaluatorBase
    {
        public List<SimpleMovie> movies { get; set; } 

        public KNearestNeighboursEvaluator(List<Evaluation> userEvaluations, List<SimpleMovie> movies)
            : base(userEvaluations)
        {
            // additional initializing if needed
            this.movies = movies;
        }

        public override int PredictGrade(SimpleMovie movie)
        {
            if (movie == null) {
                return 0;
            }

            List<double[]> distances = new List<double[]>();
            List<Evaluation> ratedValuations = UserEvaluations.Where(e => e.Grade != 0).ToList();


            foreach (Evaluation evaluation in ratedValuations)
            {
                SimpleMovie ratedMovie = movies.Where(m => m.MovieId == evaluation.MovieId).SingleOrDefault();
                if (ratedMovie != null)
                {
                    distances.Add(new double[] { evaluation.MovieId, EuclidianDistance(movie.ToVector(), ratedMovie.ToVector()) });
                }
            }

            distances = distances.OrderByDescending(d => d[1]).ToList();

            List<double[]> top5distances = distances.Take(5).ToList();
            List<Evaluation> top5Evaluations = new List<Evaluation>();

            foreach (double[] distance in top5distances)
            {
                top5Evaluations.Add(UserEvaluations.SingleOrDefault(e => e.MovieId == distance[0]));
            }
            //grade ; occurrence
            List<double[]> gradesOccurrences = new List<double[]>();
            gradesOccurrences.Add(new double[] { 1, 0 });
            gradesOccurrences.Add(new double[] { 2, 0 });
            gradesOccurrences.Add(new double[] { 3, 0 });
            gradesOccurrences.Add(new double[] { 4, 0 });
            gradesOccurrences.Add(new double[] { 5, 0 });

            foreach (Evaluation evaluation in top5Evaluations)
            {
                gradesOccurrences.ElementAt((int)evaluation.Grade - 1)[1] += 1;
            }

            gradesOccurrences = gradesOccurrences.OrderByDescending(g => g[1]).ToList();            

            return (int)gradesOccurrences.First()[0];
        }

        public double EuclidianDistance(double[] X, double[] Y) {
            double result = 0.0;
            
            for (int i = 0; i < X.Length; i++)
            {
                result += Math.Pow((X[i] - Y[i]),2);
            }

            return Math.Sqrt(result);
        }
    }
}

