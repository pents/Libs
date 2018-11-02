using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XMLParser
{
    public class XMLFile
    {
        private XmlDocument _xmlFile = new XmlDocument();

        private string _FilePath = "";
        private List<Node> _Nodes;

        public XMLFile(string FilePath)
        {
            _FilePath = FilePath;
            _Nodes = new List<Node>();
            readFile();
        }

        public Node getNode(string NodeName)
        {
            
            foreach (Node x in _Nodes)
            {
                if (x.getName() == NodeName)
                {
                    return x;
                }
                if (x.hasChildren) return x.getChildNode(NodeName);
            }
            return null;
        }

        private Node genNode(XmlNode node)
        {
            Node xmlNode = new Node(node.Name, node.FirstChild.Value);
            if (node.ChildNodes.Count > 1)
            {
                foreach(XmlNode childNode in node)
                {
                    xmlNode.addChild(genNode(childNode));
                }      
            }
            return xmlNode;
        }

        private void readFile()
        {
            _xmlFile.Load(_FilePath);
            foreach (XmlNode node in _xmlFile.DocumentElement.ChildNodes)
            {
                _Nodes.Add(genNode(node));
            }
        }
    }
}
