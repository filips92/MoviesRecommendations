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
    }
}
