using UnityEngine;
using System;
using gameObjectFactory;

public class Main : MonoBehaviour
{
    [SerializeField]
    private Transform root;

    [SerializeField]
    private Transform mask;

    private const string PATH_FIX = "Assets/Resource/prefab/{0}.prefab";

    void Awake()
    {
        Application.targetFrameRate = 60;

        UIManager.Instance.Init(root, mask, LoadUi);

        ResourceLoader.Load(LoadOver);
    }

    private void LoadOver()
    {
        UIManager.Instance.Show<BattleEntrance>();
    }

    private void LoadUi(Type _type, Action<GameObject> _callBack)
    {
        GameObjectFactory.Instance.GetGameObject(string.Format(PATH_FIX, _type.Name), _callBack);
    }
}
