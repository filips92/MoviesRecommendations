using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender
{
    public class Attribute
    {
        public int AttributeIndex { get; set; }
        public List<double> PossibleValues { get; set; }
        public List<Range> ValuesRanges { get; set; }
        public Attribute() {
            PossibleValues = new List<double>();
            ValuesRanges = new List<Range>();
        }
    }
}
