using UnityEngine;
using UnityEngine.UI;
using System.IO;
using superRaycast;
using superFunction;

#if UNITY_EDITOR

using UnityEditor;

#endif

public enum MapType
{
    MYPOS,
    MYBASE,
    OPPPOS,
    OPPBASE,
    RIVER,
    HILL,
    NULL
}

public class MapCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject choosePanel;

    [SerializeField]
    private GameObject inputPanel;

    [SerializeField]
    private InputField widthField;

    [SerializeField]
    private InputField heightField;

    [SerializeField]
    private GameObject mapPanel;

    [SerializeField]
    private Transform mapContainer;

    [SerializeField]
    private float unitWidth = 0.4f;

    [SerializeField]
    private float unitScale = 0.7f;

    [SerializeField]
    private Image[] bts;

    private static readonly float sqrt3 = Mathf.Sqrt(3);

    private MapUnit[] units;

    private GameObject[] arrows;

    private MapData mapData;

    private bool showMyTarget = true;

    private MapType nowMapType;

#if UNITY_EDITOR

    void Awake()
    {
        SuperRaycast.SetCamera(Camera.main);

        SuperRaycast.SetIsOpen(true, "a");
    }

    public void CreateMap()
    {
        choosePanel.SetActive(false);

        inputPanel.SetActive(true);
    }

    public void LoadMap()
    {
        string path = EditorUtility.OpenFilePanel("a", "a", "map");

        if (!string.IsNullOrEmpty(path))
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    mapData = new MapData();

                    mapData.GetData(br);

                    choosePanel.SetActive(false);

                    mapPanel.SetActive(true);

                    CreateMapPanel();
                }
            }
        }
    }

    public void ConfirmMapSize()
    {
        if (string.IsNullOrEmpty(widthField.text) || string.IsNullOrEmpty(heightField.text))
        {
            return;
        }

        inputPanel.SetActive(false);

        mapPanel.SetActive(true);

        InitMapData(int.Parse(widthField.text), int.Parse(heightField.text));

        CreateMapPanel();
    }

    private void InitMapData(int _mapWidth, int _mapHeight)
    {
        mapData = new MapData(_mapWidth, _mapHeight);
    }

    public void CreateMapPanel()
    {
        nowMapType = MapType.NULL;

        units = new MapUnit[mapData.size];

        arrows = new GameObject[mapData.size];

        int index = 0;

        for (int i = 0; i < mapData.mapWidth; i++)
        {
            for (int m = 0; m < mapData.mapHeight; m++)
            {
                if (i % 2 == 1 && m == mapData.mapHeight - 1)
                {
                    continue;
                }

                GameObject go = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resource/prefab/MapUnit.prefab"));

                go.transform.SetParent(mapContainer, false);

                //go.transform.localPosition = new Vector3(m * unitWidth * sqrt3 * 2 + ((i % 2 == 1) ? unitWidth * Mathf.Sqrt(3) : 0), -i * unitWidth * 3, 0);

                go.transform.localPosition = new Vector3(i * unitWidth * 3, -m * unitWidth * sqrt3 * 2 - ((i % 2 == 1) ? unitWidth * Mathf.Sqrt(3) : 0), 0);

                go.transform.localScale = new Vector3(unitScale, unitScale, unitScale);

                MapUnit unit = go.GetComponent<MapUnit>();

				SuperFunction.SuperFunctionCallBack3<bool, RaycastHit, int> click = delegate (int _index, bool _blockByUI, RaycastHit _hit, int _hitIndex)
                {
                    if (!_blockByUI)
                    {
                        MapUnitUpAsButton(unit);
                    }
                };

                //SuperFunction.Instance.AddEventListener(go, SuperRaycast.GetMouseClick, click);

                SuperFunction.Instance.AddEventListener(go, SuperRaycast.GetMouseButtonDown, click);

                SuperFunction.Instance.AddEventListener(go, SuperRaycast.GetMouseEnter, click);

                units[index] = unit;

                unit.Init(index);

                if (mapData.dic.ContainsKey(index))
                {
                    if (index == mapData.mBase)
                    {
                        unit.SetMainColor(bts[(int)MapType.MYBASE].color);
                    }
                    else if (index == mapData.oBase)
                    {
                        unit.SetMainColor(bts[(int)MapType.OPPBASE].color);
                    }
                    else if (mapData.dic[index] == MapData.MapUnitType.M_AREA)
                    {
                        unit.SetMainColor(bts[(int)MapType.MYPOS].color);
                    }
                    else if (mapData.dic[index] == MapData.MapUnitType.O_AREA)
                    {
                        unit.SetMainColor(bts[(int)MapType.OPPPOS].color);
                    }
                    else if (mapData.dic[index] == MapData.MapUnitType.RIVER)
                    {
                        unit.SetMainColor(bts[(int)MapType.RIVER].color);
                    }
                    else if (mapData.dic[index] == MapData.MapUnitType.HILL)
                    {
                        unit.SetMainColor(bts[(int)MapType.HILL].color);
                    }
                }
                else
                {
                    unit.SetMainColor(bts[(int)MapType.NULL].color);
                }

                index++;
            }
        }

        mapContainer.transform.localPosition = new Vector3(-0.5f * (mapData.mapWidth * unitWidth * sqrt3 * 2) + unitWidth * sqrt3, 0.5f * (mapData.mapHeight * unitWidth * 3 + unitWidth) - unitWidth * 2, 0);
    }

    public void SaveMap()
    {
        if (!CheckMap())
        {
            return;
        }

        string path = EditorUtility.SaveFilePanel("a", "a", "", "map");

        if (!string.IsNullOrEmpty(path))
        {
            FileInfo fi = new FileInfo(path);

            if (fi.Exists)
            {
                fi.Delete();
            }

            using (FileStream fs = fi.Create())
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    mapData.SetData(bw);
                }
            }
        }
    }

    private bool CheckMap()
    {
        if (mapData.mBase == -1)
        {
            Debug.Log("mapData.mBase == -1");

            return false;
        }

        if (mapData.oBase == -1)
        {
            Debug.Log("mapData.oBase == -1");

            return false;
        }

        return true;
    }

    public void BtClick(MapCreatorBt _go)
    {
        if (_go.toggle.isOn)
        {
            nowMapType = (MapType)_go.mapType;
        }
    }

    private void MapUnitUpAsButton(MapUnit _unit)
    {
        _unit.SetMainColor(bts[(int)nowMapType].color);

        switch (nowMapType)
        {
            case MapType.NULL:

                if (mapData.dic.ContainsKey(_unit.index))
                {
                    mapData.dic.Remove(_unit.index);
                }

                if (mapData.mBase == _unit.index)
                {
                    mapData.mBase = -1;
                }

                if (mapData.oBase == _unit.index)
                {
                    mapData.oBase = -1;
                }

                break;

            case MapType.MYPOS:

                if (mapData.dic.ContainsKey(_unit.index))
                {
                    mapData.dic[_unit.index] = MapData.MapUnitType.M_AREA;
                }
                else
                {
                    mapData.dic.Add(_unit.index, MapData.MapUnitType.M_AREA);
                }

                if (mapData.mBase == _unit.index)
                {
                    mapData.mBase = -1;
                }

                if (mapData.oBase == _unit.index)
                {
                    mapData.oBase = -1;
                }

                break;

            case MapType.OPPPOS:

                if (mapData.dic.ContainsKey(_unit.index))
                {
                    mapData.dic[_unit.index] = MapData.MapUnitType.O_AREA;
                }
                else
                {
                    mapData.dic.Add(_unit.index, MapData.MapUnitType.O_AREA);
                }

                if (mapData.mBase == _unit.index)
                {
                    mapData.mBase = -1;
                }

                if (mapData.oBase == _unit.index)
                {
                    mapData.oBase = -1;
                }

                break;

            case MapType.MYBASE:

                if (mapData.dic.ContainsKey(_unit.index))
                {
                    mapData.dic[_unit.index] = MapData.MapUnitType.M_AREA;
                }
                else
                {
                    mapData.dic.Add(_unit.index, MapData.MapUnitType.M_AREA);
                }

                if (mapData.mBase != _unit.index)
                {
                    if (mapData.mBase != -1)
                    {
                        units[mapData.mBase].SetMainColor(bts[(int)MapType.MYPOS].color);
                    }

                    mapData.mBase = _unit.index;
                }

                if (mapData.oBase == _unit.index)
                {
                    mapData.oBase = -1;
                }

                break;

            case MapType.OPPBASE:

                if (mapData.dic.ContainsKey(_unit.index))
                {
                    mapData.dic[_unit.index] = MapData.MapUnitType.O_AREA;
                }
                else
                {
                    mapData.dic.Add(_unit.index, MapData.MapUnitType.O_AREA);
                }

                if (mapData.mBase == _unit.index)
                {
                    mapData.mBase = -1;
                }

                if (mapData.oBase != _unit.index)
                {
                    if (mapData.oBase != -1)
                    {
                        units[mapData.oBase].SetMainColor(bts[(int)MapType.OPPPOS].color);
                    }

                    mapData.oBase = _unit.index;
                }

                break;

            case MapType.RIVER:

                if (mapData.dic.ContainsKey(_unit.index))
                {
                    mapData.dic[_unit.index] = MapData.MapUnitType.RIVER;
                }
                else
                {
                    mapData.dic.Add(_unit.index, MapData.MapUnitType.RIVER);
                }

                if (mapData.mBase == _unit.index)
                {
                    mapData.mBase = -1;
                }

                if (mapData.oBase == _unit.index)
                {
                    mapData.oBase = -1;
                }

                break;

            case MapType.HILL:

                if (mapData.dic.ContainsKey(_unit.index))
                {
                    mapData.dic[_unit.index] = MapData.MapUnitType.HILL;
                }
                else
                {
                    mapData.dic.Add(_unit.index, MapData.MapUnitType.HILL);
                }

                if (mapData.mBase == _unit.index)
                {
                    mapData.mBase = -1;
                }

                if (mapData.oBase == _unit.index)
                {
                    mapData.oBase = -1;
                }

                break;
        }
    }
#endif
}
