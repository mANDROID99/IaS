using Assets.Scripts.Controllers;
using IaS.Domain;
using UnityEngine;

namespace IaS.Controllers
{
    class JunctionController : Controller
    {
        private readonly Junction _junction;

        public JunctionController(Junction junction)
        {
            _junction = junction;
        }

        public void Update(MonoBehaviour mono)
        {
            if (Input.GetKeyUp(KeyCode.Z))
            {
                _junction.SwitchDirection();
            }
        }
    }
}
