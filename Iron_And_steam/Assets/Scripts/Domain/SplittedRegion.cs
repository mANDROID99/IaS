using System;
using UnityEngine;

namespace IaS.Domain
{
    public class SplittedRegion
    {
        public readonly BlockBounds Bounds;

        public SplittedRegion(BlockBounds bounds)
        {
            Bounds = bounds;
        }

        public bool Contains(BlockBounds bounds)
        {
            return Bounds.Contains(bounds);
        }

       public bool Contains(Vector3 pos)
        {
            return Bounds.Contains(pos);
        }

        internal UnityEngine.Bounds ToAxisAlignedBounds()
        {
            return Bounds.ToAxisAlignedBounds();
        }
    }
}
