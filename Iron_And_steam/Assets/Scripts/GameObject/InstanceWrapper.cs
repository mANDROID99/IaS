using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.WorldBuilder;

namespace IaS.GameObjects
{
    public class InstanceWrapper
    {
        public GameObject gameObject;
        public BlockBounds bounds;

        public InstanceWrapper(GameObject instance, BlockBounds bounds)
        {
            this.gameObject = instance;
            this.bounds = bounds;
        }
    }
}
