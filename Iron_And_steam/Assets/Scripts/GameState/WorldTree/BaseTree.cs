using System.Collections.Generic;
using UnityEngine;

namespace IaS.GameState.WorldTree
{
    public class BaseTree
    {
        public BaseTree Parent { get; private set; }
        public GameObject Node { get; private set; }

        private readonly Vector3 _position;
        private readonly string _name;
        private readonly List<BaseTree> _children = new List<BaseTree>();

        public BaseTree(string name, Vector3 position, Transform transform) : this(name, position)
        {
            AttachToTransform(transform);   
        }

        public BaseTree(string name, Vector3 position, BaseTree parent) : this(name, position)
        {
            Parent = parent;
            parent._children.Add(this);
            AttachToTransform(parent.Node.transform);
        }

        private BaseTree(string name, Vector3 position)
        {
            _name = name;
            _position = position;
        }

        private void AttachToTransform(Transform transform)
        {
            if (Node != null)
            {
                Object.Destroy(Node);
            }

            Node = new GameObject(_name);
            Node.transform.SetParent(transform, true);
            Node.transform.localPosition = _position;
        }

        public void Attach(GameObject leaf, bool worldPositionStays=false)
        {
            leaf.transform.SetParent(Node.transform, worldPositionStays);
        }

    }
}
