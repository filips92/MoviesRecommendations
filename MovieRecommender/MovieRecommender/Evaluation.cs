using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender
{
    class Evaluation
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int MovieId { get; set; }
        public int? Grade { get; set; }

        public bool IsEmpty()
        {
            return !Grade.HasValue || Grade.GetValueOrDefault() == 0;
        }

        public override string ToString()
        {
            return String.Format("Person {0} evaluated {1} with {2} (Id: {3})",
                this.PersonId, this.MovieId, this.Grade.GetValueOrDefault(), this.Id);
        }
    }
}
