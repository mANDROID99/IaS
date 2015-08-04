using System.Collections.Generic;
using System.Linq;
using System.Text;
using IaS.GameObjects;
using UnityEngine;

namespace IaS.Controllers
{
    class ArrowController
    {
        private const float ArrowStayaliveTime = 2.5f;
        private readonly float _startTime;
        private readonly GameObject _instance;
        private readonly Material _material;

        public ArrowController(float startTime, GameObject arrowPrefab, Transform particleTransform)
        {
            _startTime = startTime;
            _instance = Object.Instantiate(arrowPrefab, new Vector3(0, 7, 0), Quaternion.identity) as GameObject;
            _instance.transform.SetParent(particleTransform, false);
            _material = _instance.GetComponent<Renderer>().material;
        }

        public bool Update(float time)
        {
            float delta = time - _startTime;
            if (delta >= ArrowStayaliveTime)
            {
                return false;
            }

            Color color = _material.color;
            
            color.a = Mathf.Clamp(Fade(delta, ArrowStayaliveTime), 0, 1);
            _material.color = color;
            _instance.transform.Translate(new Vector3(0, 1 * Time.deltaTime, 0));
            return true;
        }

        private float Fade(float delta, float maxTime)
        {
            if (delta < 0.5f)
            {
                return 4f / maxTime * delta;
            }
            else
            {
                return 4f / maxTime * (maxTime - delta);
            }
        }

        public void Destroy()
        {
            GameObject.Destroy(_instance);
        }
    }
}
