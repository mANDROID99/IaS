using UnityEngine;

namespace IaS.GameObjects
{
    class GameObjectUtils
    {

        public static GameObject AsChildOf(Transform parent, Vector3 position, GameObject child)
        {
            child.transform.localPosition = position;
            child.transform.SetParent(parent, false);
            return child;
        }

        public static GameObject EmptyGameObject(string name, Transform parent, Vector3 localPosition)
        {
            GameObject instance = new GameObject(name);
            instance.transform.localPosition = localPosition;
            instance.transform.SetParent(parent, false);
            return instance;
        }

        public static GameObject GameObjectScript<T>(string name, Transform parent, Vector3 localPosition, out T component) where T : MonoBehaviour
        {
            GameObject instance = EmptyGameObject(name, parent, localPosition);
            component = instance.AddComponent<T>();
            return instance;
        }

    }
}
