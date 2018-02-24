using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using superFunction;
using superRaycast;
using FinalWar;
using System.Collections.Generic;

#endif

public class TestAStar : MonoBehaviour
{
#if UNITY_EDITOR

    private const float mapUnitWidth = 0.12f;

    private const float mapUnitScale = 0.22f;

    private static readonly float sqrt3 = Mathf.Sqrt(3);

    [SerializeField]
    private Color unitColor;

    [SerializeField]
    private Color hillColor;

    [SerializeField]
    private Camera renderCamera;

    [SerializeField]
    private Color startColor;

    [SerializeField]
    private Color endColor;

    [SerializeField]
    private Color pathColor;

    private MapData mapData;

    private MapUnit start;

    private MapUnit end;

    private List<MapUnit> path = new List<MapUnit>();

    private System.Random random = new System.Random();

    private Dictionary<int, MapUnit> mapUnitDic = new Dictionary<int, MapUnit>();

    void Start()
    {
        string path = EditorUtility.OpenFilePanel("title", "", "map");

        if (!string.IsNullOrEmpty(path))
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    mapData = new MapData();

                    mapData.GetData(br);
                }
            }

            CreateMapPanel();

            SuperRaycast.SetCamera(renderCamera);

            SuperRaycast.SetIsOpen(true, "aa");
        }
    }

    private void CreateMapPanel()
    {
        int index = 0;

        for (int i = 0; i < mapData.mapWidth; i++)
        {
            for (int m = 0; m < mapData.mapHeight; m++)
            {
                if (i % 2 == 1 && m == mapData.mapHeight - 1)
                {
                    continue;
                }

                if (!mapData.dic.ContainsKey(index))
                {
                    index++;

                    continue;
                }

                GameObject go = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resource/prefab/battle/MapUnit.prefab"));

                go.transform.SetParent(transform, false);

                go.transform.localPosition = new Vector3(i * mapUnitWidth * 3, (-m * mapUnitWidth * sqrt3 * 2 - ((i % 2 == 1) ? mapUnitWidth * sqrt3 : 0)), 0);

                go.transform.localScale = new Vector3(mapUnitScale, mapUnitScale, mapUnitScale);

                MapUnit unit = go.GetComponent<MapUnit>();

                SuperFunction.SuperFunctionCallBack3<bool, RaycastHit, int> tmpDele = delegate (int _index, bool _blockByUI, RaycastHit _hit, int _hitIndex)
                {
                    if (!_blockByUI)
                    {
                        Click(unit);
                    }
                };

                SuperFunction.Instance.AddEventListener(go, SuperRaycast.GetMouseClick, tmpDele);

                mapUnitDic.Add(index, unit);

                unit.Init(index);

                SetMapUnitColor(unit);

                MapData.MapUnitType mapUnitType = mapData.dic[index];

                if (mapUnitType == MapData.MapUnitType.RIVER || mapUnitType == MapData.MapUnitType.HILL)
                {
                    Destroy(unit.GetComponent<Collider>());
                }

                index++;
            }
        }
    }

    private void SetMapUnitColor(MapUnit _unit)
    {
        int index = _unit.index;

        MapData.MapUnitType mapUnitType = mapData.dic[index];

        if (mapUnitType == MapData.MapUnitType.RIVER || mapUnitType == MapData.MapUnitType.HILL)
        {
            _unit.SetMainColor(hillColor);
        }
        else
        {
            _unit.SetMainColor(unitColor);
        }
    }

    private void Click(MapUnit _unit)
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (start != null)
            {
                start.SetMainColor(unitColor);
            }

            start = _unit;

            start.SetMainColor(startColor);
        }
        else if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (end != null)
            {
                end.SetMainColor(unitColor);
            }

            end = _unit;

            end.SetMainColor(endColor);
        }
    }

    void Update()
    { 
        if (Input.GetKeyUp(KeyCode.A))
        {
            if (start != null && end != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    path[i].SetMainColor(unitColor);
                }

                path.Clear();

                List<int> list = BattleAStar.Find(mapData, start.index, end.index, int.MaxValue, GetRandomValue);

                for (int i = 0; i < list.Count - 1; i++)
                {
                    MapUnit unit = mapUnitDic[list[i]];

                    unit.SetMainColor(pathColor);

                    path.Add(unit);
                }
            }
        }
    }

    private int GetRandomValue(int _max)
    {
        return random.Next(_max);
    }
#endif
}
