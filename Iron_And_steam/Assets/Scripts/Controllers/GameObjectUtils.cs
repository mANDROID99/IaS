using UnityEngine;

namespace IaS.GameObjects
{
    class GameObjectUtils
    {

        public static GameObject AsChildOf(Transform parent, Vector3 position, GameObject child)
        {
            child.transform.parent = parent;
            child.transform.localPosition = position;
            return child;
        }

        public static GameObject EmptyGameObject(string name, Transform parent, Vector3 localPosition)
        {
            GameObject instance = new GameObject(name);
            instance.transform.parent = parent;
            instance.transform.localPosition = localPosition;
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
