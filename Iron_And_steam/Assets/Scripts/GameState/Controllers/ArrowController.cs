using Assets.Scripts.GameState.TrackConnections;
using IaS.Controllers.GO;
using IaS.World.WorldTree;
using UnityEngine;

namespace IaS.Controllers
{
    class ArrowController
    {
        private const float ArrowStayaliveTime = 1.5f;
        private readonly float _startTime;
        private readonly Material _material;
        private readonly TrackFollowingGameObject _trackFollowingGO;

        public ArrowController(float startTime, GameObject arrowPrefab, TrackRunner trackRunner)
        {
            _startTime = startTime;
            GameObject go = Object.Instantiate(arrowPrefab, new Vector3(0, 7, 0), Quaternion.identity) as GameObject;
            _material = go.transform.Find("Arrow").GetComponent<Renderer>().material;

            _trackFollowingGO = new TrackFollowingGameObject(go, trackRunner, Vector3.forward);
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
            _trackFollowingGO.MoveUpdate(1 * Time.deltaTime);
            return true;
        }

        private float Fade(float delta, float maxTime)
        {
            return delta > 0.5f ? 4f / maxTime * (maxTime - delta) : 1;
        }

        public void Destroy()
        {
            Object.Destroy(_trackFollowingGO.GameObject.gameObject);
        }
    }
}
