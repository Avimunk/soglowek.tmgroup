using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portal.Web.UI
{
    public class Tree
    {
        private List<TreeNode> nodes;
        public List<TreeNode> Nodes
        {
            get
            {
                if (this.nodes == null)
                {
                    this.nodes = new List<TreeNode>();
                }

                return this.nodes;
            }
            set
            {
                this.nodes = value;
            }
        }

        public string RootTitle { get; set; }

        public void AddNode(TreeNode node)
        {
            this.Nodes.Add(node);
            node.Tree = this;
        }

    }
}
