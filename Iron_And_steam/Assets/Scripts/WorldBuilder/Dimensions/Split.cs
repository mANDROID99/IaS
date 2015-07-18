using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace IaS.WorldBuilder
{

    public class SubSplit
    {
        public Split split { get; private set; }
        public bool clipParentLeft { get; private set; }

        public SubSplit(Split split, bool clipParentLeft)
        {
            this.split = split;
            this.clipParentLeft = clipParentLeft;
        }
    }

    public class Split
    {
        public const float CONSTRAIN_UNCONSTRAINED = -1;
        public const float CONSTRAIN_BLOCKED = -2;

        public Vector3 axis { get; private set; }
        public Vector3 pivot { get; private set; }
        public float value { get; private set; }

        public SubSplit[] subSplits { get; private set; }
        public String id { get; private set; }

        public Split(String id, Vector3 splitAxis, Vector3 pivot, float splitValue, SubSplit[] subSplits)
        {
            this.axis = splitAxis;
            this.pivot = pivot;
            this.value = splitValue;
            this.id = id;
            this.subSplits = subSplits;
        }

        public Split Copy()
        {
            return new Split(id, axis, pivot, value, subSplits);
        }

        public float DistanceFromSplit(Vector3 min)
        {
            if (axis.x > 0)
            {
                return min.x - value;
            }
            else if(axis.y > 0)
            {
                return min.y - value;
            }
            else
            {
                return min.z - value;
            }
        }

        public float Constrains(bool lhs, BlockBounds bounds)
        {
            Vector3 splitAxisVal = axis * value;
            float splitVal, min, max;
            if (splitAxisVal.x > 0)
            {
                splitVal = splitAxisVal.x;
                min = bounds.minX;
                max = bounds.maxX;
            }
            else if (splitAxisVal.y > 0)
            {
                splitVal = splitAxisVal.y;
                min = bounds.minY;
                max = bounds.maxY;
            }
            else
            {
                splitVal = splitAxisVal.z;
                min = bounds.minZ;
                max = bounds.maxZ;
            }

            if (BlockedByBounds(splitVal, min, max))
            {
                return CONSTRAIN_BLOCKED;
            }

            float splitDistance = GetDistFromSplit(lhs, splitVal, min, max);
            if (splitDistance >= 0)
            {
                return splitDistance;
            }

            return CONSTRAIN_UNCONSTRAINED;
        }

        private bool BlockedByBounds(float split, float min, float max)
        {
            return split > min && split < max;
        }

        private float GetDistFromSplit(bool lhs, float split, float min, float max)
        {
            return lhs ? split - max : min - split;
        }
    }
}

