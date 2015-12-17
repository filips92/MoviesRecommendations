using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender
{
    public abstract class EvaluatorBase
    {
        public List<Evaluation> UserEvaluations { get; set; } 
        public EvaluatorBase(List<Evaluation> userEvaluations)
        {
            this.UserEvaluations = userEvaluations;
        }
        public abstract int PredictGrade(SimpleMovie movie);
    }
}
