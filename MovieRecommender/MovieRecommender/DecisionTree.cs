using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender
{
    class DecisionTree
    {
        /// <summary>Returns a decision tree that correctly classifies the given Examples</summary>
        /// <param name="Examples">Set of data</param>
        /// <param name="Target_attribute">Attribute whose value is to be predicted by the tree</param>
        /// <param name="Attributes">List of other attributes that may be tested by the learned decision tree</param>
        public void BuildDecisionTree(Object Examples, Object Target_attribute, Object Attributes)
        {
            //TODO: determine the arguments and the return type

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
                double p_i = Set.Where(x => x[x.Length - 1] == possibleGrades[i]).Count() / Set.Count();
                entropy -= p_i * Math.Log(p_i, 2);
            }

            return entropy;

            //TODO: change data type of Set to an appriopriate one (depending on the chosen data structure)

            //throw new NotImplementedException();
        }

        /// <summary>
        /// Measures how appriopriate the candidate attribute is for the root node of the decision (sub)tree
        /// </summary>
        /// <param name="Set"></param>
        /// <param name="Attribute"></param>
        public double InformationGain(List<double[]> Set, int Attribute)
        {
            //PSEUDOCODE
            /*
             * • for all v belonging to a set of all possible values of Attribute:
             *      sum( |Set_v| / |Set| * Entropy(Set_v) ) ,
             *          where Set_v is the subset of Set for which Attribute has value v
             *      
             * • return Entropy(Set) - aboveSum
             */

            List<double> attributePossibleValues = Set.Select(x => x[Attribute]).Distinct().ToList();
            double sum = 0;

            for (int i = 0; i < attributePossibleValues.Count; i++)
            {
                List<double[]> Set_v = Set.Where(x => x[Attribute] == attributePossibleValues[i]).ToList();
                sum += (Set_v.Count() / Set.Count()) * Entropy(Set_v);
            }

            return Entropy(Set) - sum;

            //TODO: change data types of Set and Attribute to an appriopriate ones (depending on the chosen data structure)
            //throw new NotImplementedException();
        }        
    }
}
