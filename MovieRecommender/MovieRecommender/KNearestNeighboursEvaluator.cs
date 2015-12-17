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
            List<double[]> distances = new List<double[]>();

            foreach(Evaluation evaluation in UserEvaluations){
                SimpleMovie ratedMovie = movies.Where(m => m.MovieId == evaluation.MovieId).SingleOrDefault();
                distances.Add(new double[]{evaluation.MovieId, EuclidianDistance(movie.ToVector(), ratedMovie.ToVector())});
            }

            distances = distances.OrderByDescending(d => d[1]).ToList();
            Evaluation mostSimilarEvaluation = UserEvaluations.SingleOrDefault(e => e.MovieId == distances.First()[0]);

            return (int)mostSimilarEvaluation.Grade;
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

