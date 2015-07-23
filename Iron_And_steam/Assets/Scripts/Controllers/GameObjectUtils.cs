using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

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

        public static GameObject EmptyGameObject(String name, Transform parent, Vector3 localPosition)
        {
            GameObject instance = new GameObject(name);
            instance.transform.parent = parent;
            instance.transform.localPosition = localPosition;
            return instance;
        }

        public static GameObject GameObjectScript<T>(String name, Transform parent, Vector3 localPosition, out T component) where T : MonoBehaviour
        {
            GameObject instance = GameObjectUtils.EmptyGameObject(name, parent, localPosition);
            component = instance.AddComponent<T>();
            return instance;
        }

    }
}
