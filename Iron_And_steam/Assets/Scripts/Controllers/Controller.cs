using IaS.GameState;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public interface Controller
    {
        void Update(MonoBehaviour mono, GlobalGameState globalGameState);
    }
}
