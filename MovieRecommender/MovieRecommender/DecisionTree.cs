using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender
{
    public class DecisionTree
    {
        public Node Root { get; set; }

        public DecisionTree()
        {
            Root = new Node();
        }

        /// <summary>Returns a decision tree that correctly classifies the given Examples</summary>
        /// <param name="Examples">Set of data</param>
        /// <param name="Attributes">List of other attributes with possible values that may be tested by the learned decision tree</param>
        public void BuildDecisionTree(Node root, List<double[]> Examples, List<Attribute> Attributes)
        {
            //PSEUDOCODE:
            /*
             * • create a Root node for the tree
             * • if all Examples belong to the same class, Return single-node tree Root, with a label corresponding to this class
             * • if Attributes is empty, Return the single-node tree Root, with label = most common value of Target_attribute in Examples
             * • otherwise Begin
             *      • A <- the attribute from Atrributes that best classifies Examples
             *      • the decision attribute for Root <- A
             *      • for each possible value v_i of A,
             *          • add a new tree branch below Root, corresponding to the test A = v_i
             *          • let Examples_v_i be the subset of Examples that have value v_i for A
             *          • if Examples_v_i is empty
             *              • then below this new branch add a leaf node with label = most common value of Target_attribute in Examples
             *              • else below this branch add the subtree
             *                  BuildDecisionTree(Examples_v_i, Target_attribute, Attributes - |A|)
             */
            List<double> grades = Examples.Select(e => e[e.Length - 1]).Distinct().ToList();
            if (grades.Count == 1)
            {
                root.Value = (int)grades.First();
                return;
            }
            else
            {
                if (Attributes.Count == 0)
                {
                    //this one needs to be checked
                    List<double[]> mostCommon = Examples.GroupBy(e => e[e.Length - 1]).OrderByDescending(e => e.Count()).First().ToList();
                    double mostCommonValue = mostCommon.Select(e => e[e.Length - 1]).First();
                    root.Value = (int)mostCommonValue;
                    return;
                }

            }

            int bestAttribute = 0;
            double bestInformationGain = 0;
            bool[] trueOrFalse = new bool[] { true, false };

            for (int i = 0; i < Attributes.Count; i++)
            {
                //for (int j = 0; j < Attributes[i].PossibleValues.Count; j++)
                //{
                    double informationGain = InformationGain(Examples, Attributes[i].AttributeIndex, Attributes[i].ValuesRanges);
                    if (informationGain > bestInformationGain)
                    {
                        bestInformationGain = informationGain;
                        bestAttribute = Attributes[i].AttributeIndex;
                        //bestAttributeBestValue = Attributes[i].PossibleValues[j];
                    }
                    if (Attributes.Count == 1)
                    {
                        bestAttribute = Attributes[0].AttributeIndex;
                    }
                //}
            }
            root.Attribute = bestAttribute;            
            List<Range> bestAttributeRanges = Attributes.Where(a => a.AttributeIndex == bestAttribute).Single().ValuesRanges;//Examples.Select(x => x[bestAttribute]).Distinct().ToList();
            root.Ranges = bestAttributeRanges;

            //foreach (var value in trueOrFalse)
            foreach (var range in bestAttributeRanges)
            {
                List<double[]> Examples_v_i;

                //if (root.Attribute == 0)
                //{
                //    Examples_v_i = Examples.Where(e => (e[bestAttribute] == bestAttributeBestValue) == value).ToList();
                //}
                //else
                //{
                Examples_v_i = Examples.Where(e => e[bestAttribute] >= range.Min && e[bestAttribute] < range.Max).ToList();//????
                //}
                
                if (Examples_v_i.Count == 0)
                {                   
                    double _mostCommonValue = Examples.GroupBy(e => e[e.Length - 1]).OrderByDescending(e => e.Count()).First().Select(e => e[e.Length - 1]).First();                    
                    Node child = new Node();
                    child.Value = (int)_mostCommonValue;
                    root.Children.Add(child);
                }
                else
                {
                    Node child = new Node();
                    root.Children.Add(child);
                    Attribute attr = Attributes.Where(a => a.AttributeIndex == bestAttribute).SingleOrDefault();
                    if(attr != null){
                        //if (attr.PossibleValues.Contains(bestAttributeBestValue))
                        //{
                        //    attr.PossibleValues.Remove(bestAttributeBestValue);
                        //}

                        //if (Attributes.Where(a => a.AttributeIndex == bestAttribute).Single().PossibleValues.Count == 0)
                        //{
                            Attributes.Remove(Attributes.Where(a => a.AttributeIndex == bestAttribute).Single());
                        //}
                    }

                    BuildDecisionTree(child, Examples_v_i, Attributes);
                }
            }

        }

        /// <summary>
        /// Calculates the (im)purity of a collection of examples
        /// </summary>
        /// <param name="Set">Set of data</param>
        public double Entropy(List<double[]> Set)
        {
            //let's assume that here, the representation of a single movie will be a vector (SimpleMovie.toVector() with addition of the grade):
            // { Budget, DirectorId, MainLanguageId, MainActorId, Popularity, VoteAverage, Year, Grade}
            
            //PSEUDOCODE:
            /*             
             *  • for i from 1 to c (i.e. for all possible values of a given attribute (in our case grade I suppose)):
             *        sum( (-p_i) * logarythmWithBase2(p_i) ) ,
             *          where c is the number of possible values of given attribute,
             *          p_i is the proportion of examples from S with given value of given attribute (i.e. proportion of S belonging to class i)
             * 
             * • return the above sum
             */
            List<double> possibleGrades = Set.Select(x => x[x.Length - 1]).Distinct().ToList();
            double entropy = 0;

            for (int i = 0; i < possibleGrades.Count; i++)
            {
                double asd = Set.Where(x => x[x.Length - 1] == possibleGrades[i]).ToList().Count();
                double p_i = asd / (double)(Set.Count());
                entropy -= p_i * Math.Log(p_i, 2);
            }

            return entropy;
        }

        /// <summary>
        /// Measures how appriopriate the candidate attribute is for the root node of the decision (sub)tree
        /// </summary>
        /// <param name="Set"></param>
        /// <param name="Attribute"></param>
        public double InformationGain(List<double[]> Set, int AttributeIndex, List<double> AttributeValues)
        {
            //PSEUDOCODE
            /*
             * • for all v belonging to a set of all possible values of Attribute:
             *      sum( |Set_v| / |Set| * Entropy(Set_v) ) ,
             *          where Set_v is the subset of Set for which Attribute has value v
             *      
             * • return Entropy(Set) - aboveSum
             */

            //List<double> attributePossibleValues = Set.Select(x => x[Attribute]).Distinct().ToList();
            double sum = 0;

            for (int i = 0; i < AttributeValues.Count; i++)
            {
                List<double[]> Set_v = Set.Where(x => x[AttributeIndex] == AttributeValues[i]).ToList();//Set.Where(x => x[Attribute] == attributePossibleValues[i]).ToList();
                sum += ((double)Set_v.Count() / (double)Set.Count()) * Entropy(Set_v);
            }

            return Entropy(Set) - sum;
        }

        public double InformationGain(List<double[]> Set, int AttributeIndex, List<Range> AttributeRanges)
        {
            //PSEUDOCODE
            /*
             * • for all v belonging to a set of all possible values of Attribute:
             *      sum( |Set_v| / |Set| * Entropy(Set_v) ) ,
             *          where Set_v is the subset of Set for which Attribute has value v
             *      
             * • return Entropy(Set) - aboveSum
             */

            //List<double> attributePossibleValues = Set.Select(x => x[Attribute]).Distinct().ToList();
            double sum = 0;

            for (int i = 0; i < AttributeRanges.Count; i++)
            {
                List<double[]> Set_v = Set.Where(x => x[AttributeIndex] >= AttributeRanges[i].Min && x[AttributeIndex] < AttributeRanges[i].Max).ToList();//Set.Where(x => x[Attribute] == attributePossibleValues[i]).ToList();
                sum += ((double)Set_v.Count() / (double)Set.Count()) * Entropy(Set_v);
            }

            return Entropy(Set) - sum;
        }

        public int TreeDepth(Node pRoot)
        {
            if (pRoot.Children.Count == 0)
                return 0;
            int nLeft = TreeDepth(pRoot.Children[0]);
            int nRight = TreeDepth(pRoot.Children[1]);
            return (nLeft > nRight) ? (nLeft + 1) : (nRight + 1);
        }
    }
}
