using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MovieRecommender
{
    public class SimilarityEvaluator : EvaluatorBase
    {
        private List<Evaluation> AllEvaluations { get; set; }
        private readonly int PersonId;

        public SimilarityEvaluator(List<Evaluation> allEvaluations, List<Evaluation> userEvaluations)
            : base(userEvaluations)
        {
            AllEvaluations = allEvaluations;
            PersonId = userEvaluations.Count > 0 ? userEvaluations.First().PersonId : -1;
        }

        public override int PredictGrade(SimpleMovie movie)
        {
            var result = -1;

            if (UserEvaluations.Count > 0)
            {
                var userEvaluatedMoviesIds = UserEvaluations.Select(e => e.MovieId).ToList();

                var usersIdsThatEvaluatedThisMovie = AllEvaluations
                    .Where(e => e.PersonId != this.PersonId && e.MovieId == movie.MovieId && e.Grade.HasValue)
                    .Select(e => e.PersonId)
                    .Distinct()
                    .ToList();

                var otherUsersEvaluations = AllEvaluations
                    .Where(e => usersIdsThatEvaluatedThisMovie.Contains(e.PersonId) && (userEvaluatedMoviesIds.Contains(e.MovieId) || e.MovieId == movie.MovieId))
                    .ToList();

                if (otherUsersEvaluations.Any())
                {
                    // most similar = the most similar movies evaluated
                    var groups = otherUsersEvaluations
                        .GroupBy(e => e.PersonId).OrderByDescending(g => g.Count());
                    var mostSimilarPersonEvaluations = groups.First(); 

                    result = mostSimilarPersonEvaluations.First(e => e.MovieId == movie.MovieId).Grade.Value;
                }
                else
                {
                   // result = new Random().Next(0,5);
                }
            }
            else
            {
              //  result = new Random().Next(0, 5);
            }

            return result;
        }
    }
}
