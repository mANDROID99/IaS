using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IaS.WorldBuilder
{

    public class SplitTree
    {
        public static readonly BlockBounds InfiniteBounds = new BlockBounds(float.MinValue, float.MinValue, float.MinValue, float.MaxValue, float.MaxValue, float.MaxValue);


        private readonly SplitTreeNode _root;
        private IList<SplitTreeNode> _latestGeneration;

        public SplitTree(BlockBounds bounds)
        {
            _root = new SplitTreeNode(null, null);
            _root.children = new[] { new SplitTreeNode(_root, bounds) };
            _latestGeneration = _root.children;
        }

        public BlockBounds[] GatherSplitBounds()
        {
            return _latestGeneration.Select(generation => generation.bounds).ToArray();
        }

        public void Split(IList<Split> splits)
        {
            foreach (Split split in splits)
                Split(split);
        }

        public void Split(Split split)
        {

            List<SplitTreeNode> newGeneration = new List<SplitTreeNode>();
            foreach(SplitTreeNode child in _latestGeneration)
            {
                BlockBounds childBounds = child.bounds;
                BlockBounds splitL, splitR;
                this.Split(split.Axis, split.Value, childBounds, out splitL, out splitR);

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

            _latestGeneration = newGeneration.ToArray();
        }

        private void Split(Vector3 axis, float value, BlockBounds bounds, out BlockBounds splitL, out BlockBounds splitR)
        {
            float splitX = axis.x * value;
            float splitY = axis.y * value;
            float splitZ = axis.z * value;

            if(((axis.x != 0) && (splitX > bounds.MinX)) ||
                ((axis.y != 0) && (splitY > bounds.MinY)) ||
                ((axis.z != 0) && (splitZ > bounds.MinZ)))
            {
                float maxX = axis.x == 0 ? bounds.MaxX : Mathf.Min(splitX, bounds.MaxX);
                float maxY = axis.y == 0 ? bounds.MaxY : Mathf.Min(splitY, bounds.MaxY);
                float maxZ = axis.z == 0 ? bounds.MaxZ : Mathf.Min(splitZ, bounds.MaxZ);
                splitL = new BlockBounds(bounds.MinX, bounds.MinY, bounds.MinZ, maxX, maxY, maxZ);
            }
            else
            {
                splitL = null;
            }

            if (((axis.x != 0) && (splitX < bounds.MaxX)) ||
                ((axis.y != 0) && (splitY < bounds.MaxY)) ||
                ((axis.z != 0) && (splitZ < bounds.MaxZ)))
            {
                float minX = axis.x == 0 ? bounds.MinX : Mathf.Max(splitX, bounds.MinX);
                float minY = axis.y == 0 ? bounds.MinY : Mathf.Max(splitY, bounds.MinY);
                float minZ = axis.z == 0 ? bounds.MinZ : Mathf.Max(splitZ, bounds.MinZ);
                splitR = new BlockBounds(minX, minY, minZ, bounds.MaxX, bounds.MaxY, bounds.MaxZ);
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
