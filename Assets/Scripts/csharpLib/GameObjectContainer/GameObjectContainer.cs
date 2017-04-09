using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectContainer : MonoBehaviour
{
    public List<Entity> Entitys;

    [System.Serializable]
    public class Entity
    {
        public string name;
        public GameObject value;
    }

    public void Add()
    {
        if (Entitys == null)
        {
            Entitys = new List<Entity>();
        }

        Entitys.Add(new Entity());
    }
}


