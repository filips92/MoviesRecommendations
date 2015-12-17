using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender
{
    public class DecisionTreeEvaluator : EvaluatorBase
    {
        public DecisionTreeEvaluator(List<Evaluation> userEvaluations) : base(userEvaluations)
        {
            // additional initializing if needed
        }

        public override int PredictGrade(int movieId)
        {
            throw new NotImplementedException();
        }
    }
}
