using System;
using UnityEngine;

namespace IaS.WorldBuilder.XML.mappers
{
    public class DirectionMapper : IXmlValueMapper<Vector3>
    {
        public const string DirectionForward = "forward";
        public const string DirectionBack = "back";
        public const string DirectionRight = "right";
        public const string DirectionLeft = "left";
        public const string DirectionUp = "up";
        public const string DirectionDown = "down";

        public Vector3 Map(string from)
        {
            switch (from)
            {
                case DirectionForward:
                    return Vector3.forward;
                case DirectionBack:
                    return Vector3.back;
                case DirectionLeft:
                    return Vector3.left;
                case DirectionRight:
                    return Vector3.right;
                case DirectionUp:
                    return Vector3.up;
                case DirectionDown:
                    return Vector3.down;
                default:
                    throw new Exception(string.Format("Value '{0}' cannot be converted to a Direction.", from));
            }
        }
    }
}