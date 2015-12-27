﻿using System;
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
        static void Main(string[] args)
        {
            var client = new TMDbClient(ConfigurationManager.AppSettings["TMDbAPIKey"]);
            var evaluations = LoadEvaluations();
            var emptyEvaluations = LoadEmptyEvaluations();
            var personIds = LoadPersonIds();
            var cachedMovies = LoadCachedMovies();
            
            foreach (var personId in personIds)
            {
                var userEvaluations = evaluations.Where(e => e.PersonId == personId).ToList();
                var evaluator = new KNearestNeighboursEvaluator(userEvaluations, cachedMovies);
                var emptyUserEvaluations = emptyEvaluations.Where(e => e.PersonId == personId).ToList();//userEvaluations.Where(e => e.IsEmpty()).ToList();
               
                foreach (var singleEmptyEvaluation in emptyUserEvaluations)
                {
                    var notEvaluatedMovie = cachedMovies.SingleOrDefault(cm => cm.MovieId == singleEmptyEvaluation.MovieId);
                    singleEmptyEvaluation.Grade = evaluator.PredictGrade(cachedMovies.Where(m => m.MovieId == singleEmptyEvaluation.MovieId).FirstOrDefault());
                }
            }
            generateCsv(evaluations);
        }

        public static void generateCsv(List<Evaluation> evaluations) {
            StringBuilder strBuilder = new StringBuilder();
            foreach (var evaluation in evaluations)
            {
                strBuilder.AppendLine(evaluation.Id + ";" + evaluation.PersonId + ";" + evaluation.MovieId + ";" + evaluation.Grade);
            }
            File.WriteAllText("submissionTEST.csv", strBuilder.ToString());
        }

        private static List<SimpleMovie> LoadCachedMovies()
        {
            var reader = new StreamReader(File.OpenRead("../../AppData/cachedMovies.csv"));
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

            reader.ReadLine();//skipping the column names
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

        private static List<Evaluation> LoadEmptyEvaluations() {
            var reader = new StreamReader(File.OpenRead("../../AppData/task.csv"));
            var emptyEvaluations = new List<Evaluation>();

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
                    Grade = 0
                };
                emptyEvaluations.Add(evaluation);
            }

            return emptyEvaluations;
        }
    }
}
