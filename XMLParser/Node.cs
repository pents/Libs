using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLParser
{
    public class Node
    {
        private string _NodeName;
        private string _NodeValue;
        private List<Node> ChildNodes;
        public bool hasChildren = false;
        public string getName(){ return _NodeName;}
        public string getValue(){return _NodeValue;}

        public Node(string Name, string Value)
        {
            _NodeName = Name;
            _NodeValue = Value;
            ChildNodes = new List<Node>();
        }

        public void addChild(Node child)
        {
            ChildNodes.Add(child);
            hasChildren = true;
        }

        public Node getChildNode(string NodeName)
        {
            foreach(Node x in ChildNodes)
            {
                if (x.getName() == NodeName)
                {
                    return x;
                }
            }
            return null;
        }

    }
}
