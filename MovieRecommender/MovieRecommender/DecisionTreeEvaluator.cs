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
            if (movie == null)
            {
                return 0;
            }

            double[] movieVector = movie.ToVector();

            Node root = Tree.Root;

            while (true) {
                if (root.Value != null)
                {
                    return (int)root.Value;
                }
                else
                {
                    //navigate through the tree
                    if (movieVector[(int)root.Attribute] <= root.Ranges[0].Min)
                    {
                        root = root.Children[0];
                    }
                    else 
                    {
                        if (movieVector[(int)root.Attribute] >= root.Ranges.Last().Max)
                        {
                            root = root.Children.Last();
                        }
                        else
                        {
                            for (int i = 0; i < root.Ranges.Count; i++)
                            {
                                if (movieVector[(int)root.Attribute] >= root.Ranges[i].Min && movieVector[(int)root.Attribute] < root.Ranges[i].Max)
                                {
                                    root = root.Children[i];
                                }
                            }
                        }
                    }
                    

                        //if (movieVector[(int)root.Attribute] > root.AttributeValue)//????
                        //{
                        //    root = root.Children[0];
                        //}
                        //else
                        //{
                        //    root = root.Children[1];
                        //}
                }
            }
        }
    }
}
