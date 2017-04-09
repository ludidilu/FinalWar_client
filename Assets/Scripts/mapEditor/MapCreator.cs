using UnityEngine;
using UnityEngine.UI;
using System.IO;
using superRaycast;
using superFunction;
using System;
using System.Collections.Generic;

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

        //		for (int i = 0; i < mapData.size; i++) {
        //			
        //			mapData.dic.Add(i,MapData.MapUnitType.HILL);
        //
        //			mapData.moveMap.Add(i,new KeyValuePair<int, int>(-1,-1));
        //		}
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

				SuperFunction.SuperFunctionCallBack click = delegate (int _index, object[] _objs)
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

        index = 0;

        for (int i = 0; i < mapData.mapHeight; i++)
        {
            for (int m = 0; m < mapData.mapWidth; m++)
            {
                if (i % 2 == 1 && m == mapData.mapWidth - 1)
                {
                    continue;
                }

                GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Arrow"));

                go.transform.SetParent(arrowContainer, false);

                arrows[index] = go;

                if (mapData.moveMap.ContainsKey(index))
                {
                    if (mapData.moveMap[index].Key != -1)
                    {
                        ShowArrow(index, mapData.moveMap[index].Key);
                    }
                    else
                    {
                        HideArrow(index);
                    }
                }
                else
                {
                    HideArrow(index);
                }

                index++;
            }
        }
    }

    private void ShowArrow(int _start, int _end)
    {
        GameObject go = arrows[_start];

        MapUnit start = units[_start];

        MapUnit end = units[_end];

        go.transform.position = (start.transform.position + end.transform.position) * 0.5f;

        float angle = Mathf.Atan2(end.transform.position.y - start.transform.position.y, end.transform.position.x - start.transform.position.x);

        Quaternion q = new Quaternion();

        q.eulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI);

        go.transform.localRotation = q;

        go.SetActive(true);
    }

    private void HideArrow(int _pos)
    {
        arrows[_pos].SetActive(false);
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

        Dictionary<int, MapData.MapUnitType>.Enumerator enumerator = mapData.dic.GetEnumerator();

        while (enumerator.MoveNext())
        {
            if (enumerator.Current.Value != MapData.MapUnitType.RIVER && enumerator.Current.Value != MapData.MapUnitType.HILL)
            {
                KeyValuePair<int, int> pair = mapData.moveMap[enumerator.Current.Key];

                if (enumerator.Current.Key == mapData.mBase)
                {
                    if (pair.Key == -1)
                    {
                        Debug.Log("moveMap error pos=" + enumerator.Current.Key);

                        return false;
                    }
                }
                else if (enumerator.Current.Key == mapData.oBase)
                {
                    if (pair.Value == -1)
                    {
                        Debug.Log("moveMap error pos=" + enumerator.Current.Key);

                        return false;
                    }
                }
                else if (enumerator.Current.Value == MapData.MapUnitType.M_AREA || enumerator.Current.Value == MapData.MapUnitType.O_AREA)
                {
                    if (pair.Key == -1 || pair.Value == -1)
                    {
                        Debug.Log("moveMap error pos=" + enumerator.Current.Key);

                        return false;
                    }
                }
            }
        }

        return true;
    }

    public void BtClick(int _index)
    {
        nowMapType = (MapType)_index;
    }

    private int downPos = -1;

    private bool hasExit = false;

    public void MapUnitDown(MapUnit _unit)
    {
        hasExit = false;

        if (mapData.dic.ContainsKey(_unit.index))
        {
            MapData.MapUnitType mapUnitType = mapData.dic[_unit.index];

            if (mapUnitType == MapData.MapUnitType.M_AREA || mapUnitType == MapData.MapUnitType.O_AREA)
            {
                downPos = _unit.index;
            }
        }
    }

    public void MapUnitEnter(MapUnit _unit)
    {
        if (downPos != -1)
        {
            if (mapData.dic.ContainsKey(_unit.index))
            {
                MapData.MapUnitType mapUnitType = mapData.dic[_unit.index];

                if (mapUnitType == MapData.MapUnitType.M_AREA || mapUnitType == MapData.MapUnitType.O_AREA)
                {
                    if (BattlePublicTools.GetDistance(mapData.mapWidth, downPos, _unit.index) == 1)
                    {
                        if (showMyTarget)
                        {
                            mapData.moveMap[downPos] = new KeyValuePair<int, int>(_unit.index, mapData.moveMap[downPos].Value);
                        }
                        else
                        {
                            mapData.moveMap[downPos] = new KeyValuePair<int, int>(mapData.moveMap[downPos].Key, _unit.index);
                        }

                        ShowArrow(downPos, _unit.index);
                    }
                }
            }
        }
    }

    public void MapUnitExit(MapUnit _unit)
    {
        hasExit = true;

        if (downPos != -1)
        {
            if (showMyTarget)
            {
                if (mapData.moveMap[downPos].Key == _unit.index)
                {
                    mapData.moveMap[downPos] = new KeyValuePair<int, int>(-1, mapData.moveMap[downPos].Value);

                    HideArrow(downPos);
                }
            }
            else
            {
                if (mapData.moveMap[downPos].Value == _unit.index)
                {
                    mapData.moveMap[downPos] = new KeyValuePair<int, int>(mapData.moveMap[downPos].Key, -1);

                    HideArrow(downPos);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            downPos = -1;
        }
    }

    public void Switch()
    {
        showMyTarget = !showMyTarget;

        switchBtText.text = showMyTarget ? "M" : "O";

        Dictionary<int, KeyValuePair<int, int>>.Enumerator enumerator = mapData.moveMap.GetEnumerator();

        while (enumerator.MoveNext())
        {
            if (showMyTarget)
            {
                if (enumerator.Current.Value.Key != -1)
                {
                    ShowArrow(enumerator.Current.Key, enumerator.Current.Value.Key);
                }
                else
                {
                    HideArrow(enumerator.Current.Key);
                }
            }
            else
            {
                if (enumerator.Current.Value.Value != -1)
                {
                    ShowArrow(enumerator.Current.Key, enumerator.Current.Value.Value);
                }
                else
                {
                    HideArrow(enumerator.Current.Key);
                }
            }
        }
    }

    public void MapUnitUpAsButton(MapUnit _unit)
    {
        if (hasExit)
        {
            return;
        }

        _unit.SetMainColor(bts[(int)nowMapType].color);

        switch (nowMapType)
        {
            case MapType.NULL:

                if (mapData.dic.ContainsKey(_unit.index))
                {
                    mapData.dic.Remove(_unit.index);
                }

                HideArrow(_unit.index);

                if (mapData.moveMap.ContainsKey(_unit.index))
                {
                    mapData.moveMap.Remove(_unit.index);
                }

                List<int> mDel = new List<int>();

                List<int> oDel = new List<int>();

                foreach (KeyValuePair<int, KeyValuePair<int, int>> pair in mapData.moveMap)
                {
                    if (pair.Value.Key == _unit.index)
                    {
                        mDel.Add(pair.Key);
                    }

                    if (pair.Value.Value == _unit.index)
                    {
                        oDel.Add(pair.Key);
                    }
                }

                for (int i = 0; i < mDel.Count; i++)
                {
                    mapData.moveMap[mDel[i]] = new KeyValuePair<int, int>(-1, mapData.moveMap[mDel[i]].Value);

                    if (showMyTarget)
                    {
                        HideArrow(mDel[i]);
                    }
                }

                for (int i = 0; i < oDel.Count; i++)
                {
                    mapData.moveMap[oDel[i]] = new KeyValuePair<int, int>(mapData.moveMap[oDel[i]].Key, -1);

                    if (!showMyTarget)
                    {
                        HideArrow(oDel[i]);
                    }
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

                if (!mapData.moveMap.ContainsKey(_unit.index))
                {
                    mapData.moveMap.Add(_unit.index, new KeyValuePair<int, int>(-1, -1));
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

                if (!mapData.moveMap.ContainsKey(_unit.index))
                {
                    mapData.moveMap.Add(_unit.index, new KeyValuePair<int, int>(-1, -1));
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

                if (!mapData.moveMap.ContainsKey(_unit.index))
                {
                    mapData.moveMap.Add(_unit.index, new KeyValuePair<int, int>(-1, -1));
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

                if (!mapData.moveMap.ContainsKey(_unit.index))
                {
                    mapData.moveMap.Add(_unit.index, new KeyValuePair<int, int>(-1, -1));
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

                HideArrow(_unit.index);

                if (mapData.moveMap.ContainsKey(_unit.index))
                {
                    mapData.moveMap.Remove(_unit.index);
                }

                mDel = new List<int>();

                oDel = new List<int>();

                foreach (KeyValuePair<int, KeyValuePair<int, int>> pair in mapData.moveMap)
                {
                    if (pair.Value.Key == _unit.index)
                    {
                        mDel.Add(pair.Key);
                    }

                    if (pair.Value.Value == _unit.index)
                    {
                        oDel.Add(pair.Key);
                    }
                }

                for (int i = 0; i < mDel.Count; i++)
                {
                    mapData.moveMap[mDel[i]] = new KeyValuePair<int, int>(-1, mapData.moveMap[mDel[i]].Value);

                    if (showMyTarget)
                    {
                        HideArrow(mDel[i]);
                    }
                }

                for (int i = 0; i < oDel.Count; i++)
                {
                    mapData.moveMap[oDel[i]] = new KeyValuePair<int, int>(mapData.moveMap[oDel[i]].Key, -1);

                    if (!showMyTarget)
                    {
                        HideArrow(oDel[i]);
                    }
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

                HideArrow(_unit.index);

                if (mapData.moveMap.ContainsKey(_unit.index))
                {
                    mapData.moveMap.Remove(_unit.index);
                }

                mDel = new List<int>();

                oDel = new List<int>();

                foreach (KeyValuePair<int, KeyValuePair<int, int>> pair in mapData.moveMap)
                {
                    if (pair.Value.Key == _unit.index)
                    {
                        mDel.Add(pair.Key);
                    }

                    if (pair.Value.Value == _unit.index)
                    {
                        oDel.Add(pair.Key);
                    }
                }

                for (int i = 0; i < mDel.Count; i++)
                {
                    mapData.moveMap[mDel[i]] = new KeyValuePair<int, int>(-1, mapData.moveMap[mDel[i]].Value);

                    if (showMyTarget)
                    {
                        HideArrow(mDel[i]);
                    }
                }

                for (int i = 0; i < oDel.Count; i++)
                {
                    mapData.moveMap[oDel[i]] = new KeyValuePair<int, int>(mapData.moveMap[oDel[i]].Key, -1);

                    if (!showMyTarget)
                    {
                        HideArrow(oDel[i]);
                    }
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
