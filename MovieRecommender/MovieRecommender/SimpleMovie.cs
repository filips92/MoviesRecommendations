using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Objects.Movies;

namespace MovieRecommender
{
    class SimpleMovie
    {
        public SimpleMovie(Movie tmdbMovie)
        {
            this.Budget = tmdbMovie.Budget;
            this.MovieId = tmdbMovie.Id;
            this.Popularity = tmdbMovie.Popularity;
            this.Title = tmdbMovie.Title;
            this.VoteAverage = tmdbMovie.VoteAverage;
            if (tmdbMovie.Id != 0)
            {
                var director = tmdbMovie.Credits.Crew.FirstOrDefault(c => c.Job == "Director");
                if (director != null)
                {
                    this.Director = director.Name;
                    this.DirectorId = director.Id;
                }
                else
                {
                    this.Director = "N/A";
                    this.DirectorId = -1;
                }

                var mainLanguage = tmdbMovie.SpokenLanguages.FirstOrDefault();
                if (mainLanguage != null)
                {
                    this.MainLanguage = mainLanguage.Name;
                    var culture =
                        CultureInfo.GetCultures(CultureTypes.AllCultures)
                            .FirstOrDefault(c => c.TwoLetterISOLanguageName == mainLanguage.Iso_639_1);
                    if (culture != null)
                    {
                        this.MainLanguageId = culture.LCID;
                    }
                    else
                    {
                        this.MainLanguageId = -1;
                    }
                }

                var mainActor = tmdbMovie.Credits.Cast.OrderBy(c => c.Order).FirstOrDefault();
                if (mainActor != null)
                {
                    this.MainActor = mainActor.Name;
                    this.MainActorId = mainActor.Id;
                }
                else
                {
                    this.MainActor = "N/A";
                    this.MainActorId = -1;
                }

                var releaseDate = tmdbMovie.ReleaseDate;
                if (releaseDate.HasValue)
                {
                    this.Year = releaseDate.Value.Year;
                }
                else
                {
                    this.Year = -1;
                }
            }
        }

        public long Budget { get; set; }
        public string Director { get; set; }
        public int DirectorId { get; set; }
        public string MainLanguage { get; set; }
        public int MainLanguageId { get; set; }
        public int MovieId { get; set; }
        public string MainActor { get; set; }
        public int MainActorId { get; set; }
        public double Popularity { get; set; }
        public double VoteAverage { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }

        public override string ToString()
        {
            return String.Format("{0} ({1}) by {2}", this.Title, this.Year, this.Director);
        }

        public double[] ToVector()
        {
            return new double[]
            {
                //this.Budget,
                //this.DirectorId,
                //this.MainLanguageId,
                //this.MainActorId,
                this.Popularity,
                this.VoteAverage,
                //this.Year
            };
        }

        public string ToVectorString()
        {
            return String.Join(" ", this.ToVector());
        }
    }
}
