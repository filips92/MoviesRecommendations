using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender
{
    public class Node
    {
        public int? Attribute { get; set; }// null if the node is the leaf
        public double? AttributeValue { get; set; }
        public int? Value { get; set; } //not null if the node is the leaf of the tree
        public List<Node> Children { get; set; } //no elements if the node is the leaf of the tree
        public List<Range> Ranges { get; set; }
        public Node()
        {
            Children = new List<Node>();
            Ranges = new List<Range>();
        }
    }
}
