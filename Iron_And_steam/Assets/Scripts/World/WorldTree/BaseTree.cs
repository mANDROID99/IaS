using System.Collections.Generic;
using UnityEngine;

namespace IaS.World.WorldTree
{
    public class BaseTree
    {
        public BaseTree Parent { get; private set; }
        public GameObject Node { get; private set; }

        private readonly NodeConfig _nodeConfig;
        private readonly Vector3 _position;
        private readonly string _name;
        private readonly List<BaseTree> _children = new List<BaseTree>();

        public BaseTree(string name, Vector3 position, BaseTree parent, NodeConfig nodeConfig) : this(name, position, parent.Node.transform, nodeConfig)
        {
            Parent = parent;
            parent._children.Add(this);
        }

        public BaseTree(string name, Vector3 position, Transform transform, NodeConfig nodeConfig)
        {
            _name = name;
            _position = position;
            _nodeConfig = nodeConfig;
            AttachToTransform(transform);
        }


        private void AttachToTransform(Transform transform)
        {
            if (Node != null)
            {
                Object.Destroy(Node);
            }

            Node = new GameObject(_name);
            Node.isStatic = _nodeConfig.Static;
            Node.transform.SetParent(transform, true);
            Node.transform.localPosition = _position;
        }

        public void Attach(GameObject leaf, bool worldPositionStays=false)
        {
            leaf.transform.SetParent(Node.transform, worldPositionStays);

            if (_nodeConfig.PropagateStaticToChildren)
            {
                PropogateStaticToChildren(leaf);
            }
        }

        private void PropogateStaticToChildren(GameObject leaf)
        {
            leaf.isStatic = _nodeConfig.Static;
            for(int i = 0; i < leaf.transform.childCount; i++)
            {
                PropogateStaticToChildren(leaf.transform.GetChild(i).gameObject);
            }
        }

    }
}
