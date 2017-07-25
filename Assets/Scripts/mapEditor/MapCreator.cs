using UnityEngine;
using UnityEngine.UI;
using System.IO;
using superRaycast;
using superFunction;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class MapCreator : MonoBehaviour
{
    private enum MapType
    {
        MYPOS,
        MYBASE,
        OPPPOSE,
        OPPBASE,
        RIVER,
        HILL,
        NULL
    }

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

    [SerializeField]
    private GameObject btShow;

    [SerializeField]
    private Text switchBtText;

    [SerializeField]
    private RectTransform arrowContainer;

    private static readonly float sqrt3 = Mathf.Sqrt(3);

    private MapUnit[] units;

    private GameObject[] arrows;

    private MapData mapData;

    private bool showMyTarget = true;

    private MapType m_nowMapType;

    private MapType nowMapType
    {
        get
        {
            return m_nowMapType;
        }

        set
        {
            m_nowMapType = value;

            (btShow.transform as RectTransform).anchoredPosition = (bts[(int)m_nowMapType].transform as RectTransform).anchoredPosition;
        }
    }

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

#if UNITY_EDITOR

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

#endif
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
        nowMapType = MapType.MYPOS;

        int size = mapData.mapWidth * mapData.mapHeight - mapData.mapHeight / 2;

        units = new MapUnit[size];

        arrows = new GameObject[size];

        int index = 0;

        for (int i = 0; i < mapData.mapHeight; i++)
        {
            for (int m = 0; m < mapData.mapWidth; m++)
            {
                if (i % 2 == 1 && m == mapData.mapWidth - 1)
                {
                    continue;
                }

                GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("MapUnit"));

                go.transform.SetParent(mapContainer, false);

                go.transform.localPosition = new Vector3(m * unitWidth * sqrt3 * 2 + ((i % 2 == 1) ? unitWidth * Mathf.Sqrt(3) : 0), -i * unitWidth * 3, 0);

                go.transform.localScale = new Vector3(unitScale, unitScale, unitScale);

                MapUnit unit = go.GetComponent<MapUnit>();

				SuperFunction.SuperFunctionCallBack0 click = delegate (int _index)
                {
                    MapUnitUpAsButton(unit);
                };

                SuperFunction.Instance.AddEventListener(go, SuperRaycast.GetMouseClick, click);

                units[index] = unit;

                unit.Init(index, 0, null);

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
                        unit.SetMainColor(bts[(int)MapType.OPPPOSE].color);
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

#if UNITY_EDITOR

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
#endif
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

    public void BtClick(int _index)
    {
        nowMapType = (MapType)_index;
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

            case MapType.OPPPOSE:

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
                        units[mapData.oBase].SetMainColor(bts[(int)MapType.OPPPOSE].color);
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
}
