using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender
{
    public class DecisionTreeEvaluator : EvaluatorBase
    {
        public DecisionTree Tree { get; set; }

        public DecisionTreeEvaluator(DecisionTree tree, List<Evaluation> userEvaluations)
            : base(userEvaluations)
        {
            // additional initializing if needed
            this.Tree = tree;
        }

        public override int PredictGrade(SimpleMovie movie)
        {
            double[] movieVector = movie.ToVector();

            Node root = Tree.Root;

            while (true) {
                if (root.Value != null)
                {
                    return (int)root.Value;
                }
                else
                {
                    //navigate through the (binary) tree
                    if (movieVector[(int)root.Attribute] == root.AttributeValue)
                    {
                        root = root.Children[0];
                    }
                    else
                    {
                        root = root.Children[1];
                    }
                    //root = root.Children.Where(c => c.PreviousNodeAttributeValue == movieVector[(int)root.Attribute]).Single();
                }
            }
        }
    }
}
