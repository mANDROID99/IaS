using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.Helpers;


namespace IaS.WorldBuilder
{

    public class SplitTree
    {
        private SplitTreeNode root;
        private IList<SplitTreeNode> latestGeneration;

        public SplitTree(BlockBounds bounds)
        {
            root = new SplitTreeNode(null, null);
            root.children = new SplitTreeNode[] { new SplitTreeNode(root, bounds) };
            this.latestGeneration = root.children;
        }

        /*
        public SplitTree(IList<MeshBlock> meshBlocks)
        {
            root = new SplitTreeNode(null, null);
            root.children = meshBlocks.Select(block => new SplitTreeNode(root, block)).ToArray();
            this.latestGeneration = root.children;
        }*/

        public BlockBounds[] GatherSplitBounds()
        {
            return latestGeneration.Select(generation => generation.bounds).ToArray();
        }

        public void Split(IList<Split> splits)
        {
            foreach (Split split in splits)
                Split(split);
        }

        public void Split(Split split)
        {

            List<SplitTreeNode> newGeneration = new List<SplitTreeNode>();
            foreach(SplitTreeNode child in latestGeneration)
            {
                BlockBounds childBounds = child.bounds;
                BlockBounds splitL, splitR;
                this.Split(split.axis, split.value, childBounds, out splitL, out splitR);

                if ((splitL != null) && (splitR != null))
                {
                    SplitTreeNode nodeL = new SplitTreeNode(child, splitL);
                    SplitTreeNode nodeR = new SplitTreeNode(child, splitR);

                    newGeneration.Add(nodeL);
                    newGeneration.Add(nodeR);

                    child.bounds = null;
                    child.children = new SplitTreeNode[] { nodeL, nodeR };

                }
                else
                {
                    newGeneration.Add(child);
                }
            }

            latestGeneration = newGeneration.ToArray();
        }

        private void Split(Vector3 axis, float value, BlockBounds bounds, out BlockBounds splitL, out BlockBounds splitR)
        {
            float splitX = axis.x * value;
            float splitY = axis.y * value;
            float splitZ = axis.z * value;

            if(((axis.x != 0) && (splitX > bounds.minX)) ||
                ((axis.y != 0) && (splitY > bounds.minY)) ||
                ((axis.z != 0) && (splitZ > bounds.minZ)))
            {
                float maxX = axis.x == 0 ? bounds.maxX : Mathf.Min(splitX, bounds.maxX);
                float maxY = axis.y == 0 ? bounds.maxY : Mathf.Min(splitY, bounds.maxY);
                float maxZ = axis.z == 0 ? bounds.maxZ : Mathf.Min(splitZ, bounds.maxZ);
                splitL = new BlockBounds(bounds.minX, bounds.minY, bounds.minZ, maxX, maxY, maxZ);
            }
            else
            {
                splitL = null;
            }

            if (((axis.x != 0) && (splitX < bounds.maxX)) ||
                ((axis.y != 0) && (splitY < bounds.maxY)) ||
                ((axis.z != 0) && (splitZ < bounds.maxZ)))
            {
                float minX = axis.x == 0 ? bounds.minX : Mathf.Max(splitX, bounds.minX);
                float minY = axis.y == 0 ? bounds.minY : Mathf.Max(splitY, bounds.minY);
                float minZ = axis.z == 0 ? bounds.minZ : Mathf.Max(splitZ, bounds.minZ);
                splitR = new BlockBounds(minX, minY, minZ, bounds.maxX, bounds.maxY, bounds.maxZ);
            }
            else
            {
                splitR = null;
            }
        }
    }

    internal class SplitTreeNode
    {
        public IList<SplitTreeNode> children { get; internal set; }
        public SplitTreeNode parent { get; internal set; }
        public BlockBounds bounds { get; internal set; }

        public bool IsLatestGeneration { get { return children == null || children.Count == 0; } }

        public SplitTreeNode(SplitTreeNode parent, BlockBounds bounds)
        {
            this.children = new SplitTreeNode[0];
            this.parent = parent;
            this.bounds = bounds;
        }

        internal IEnumerable<SplitTreeNode> GatherDescendants()
        {
            Stack<SplitTreeNode> stack = new Stack<SplitTreeNode>();
            stack.Push(this);
            while (stack.Count > 0)
            {
                SplitTreeNode child = stack.Pop();
                if (child.IsLatestGeneration)
                {
                    yield return child;
                }
                else
                {
                    foreach (SplitTreeNode descendant in child.children)
                    {
                        stack.Push(descendant);
                    }
                }
            }
        }
    }
}
