﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender
{
    class Node
    {
        public int Attribute { get; set; }
        public int? Value { get; set; } //not null if the node is the leaf of the tree
        public List<Node> Children { get; set; } //no elements if the node is the leaf of the tree

        public Node()
        {
            Children = new List<Node>();
        }
    }
}
