using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using IaS.WorldBuilder;
using IaS.Helpers;

namespace IaS.GameState
{
    public class InstanceWrapper
    {
        public GameObject gameObject {get; private set;}
        public BlockBounds bounds {get; private set;}
        public BlockBounds rotatedBounds { get; private set; }

        public Quaternion startRotation { get; set; }
        public Quaternion endRotation { get; set; }
        public List<InstanceEventHandler> eventHandlers { get; set; }

        public InstanceWrapper(GameObject instance, BlockBounds bounds)
        {
            this.eventHandlers = new List<InstanceEventHandler>();
            this.gameObject = instance;
            this.bounds = bounds;
            this.rotatedBounds = bounds.Copy();
            this.endRotation = Quaternion.identity;
        }

        public void OnEndTransform(Transformation transformation)
        {
            foreach(InstanceEventHandler handler in eventHandlers)
                handler.OnEndTransform(this, transformation);
        }

        public void OnUpdateTransform(Transformation transformation)
        {
            foreach (InstanceEventHandler handler in eventHandlers)
                handler.OnUpdateTransform(this, transformation);
        }
    }

    public interface InstanceEventHandler
    {
        void OnEndTransform(InstanceWrapper instance, Transformation transform);
        void OnUpdateTransform(InstanceWrapper instance, Transformation transform);
    }
}
