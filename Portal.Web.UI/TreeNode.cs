using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Portal.Web.UI
{
    public class TreeNode
    {
        private List<TreeNode> childNodes;

        public TreeNode Parent { get; set; }
        public Tree Tree { get; set; }
        public string ThumbImageUrl { get; set; }
        public string Title { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public Int64 Id { get; set; }

        public List<TreeNode> ChildNodes
        {
            get
            {
                if (this.childNodes == null)
                {
                    this.childNodes = new List<TreeNode>();
                }

                return this.childNodes;
            }
            set
            {
                this.childNodes = value;
            }
        }

        public TreeNode() { }

        public void AddChild(TreeNode node)
        {
            this.ChildNodes.Add(node);
            node.Tree = this.Tree;
            node.Parent = this;
        }
    }
}
