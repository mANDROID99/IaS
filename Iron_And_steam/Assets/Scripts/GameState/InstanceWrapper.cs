using IaS.Domain;
using UnityEngine;

namespace IaS.GameState
{
    public class InstanceWrapper
    {
        public GameObject gameObject {get; private set;}
        public BlockBounds bounds {get; private set;}
        public BlockBounds rotatedBounds { get; private set; }

        public Quaternion startRotation { get; set; }
        public Quaternion endRotation { get; set; }

        public InstanceWrapper(GameObject instance, BlockBounds bounds)
        {
            this.gameObject = instance;
            this.bounds = bounds;
            this.rotatedBounds = bounds.Copy();
            this.endRotation = Quaternion.identity;
        }
    }
}
