using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using xy3d.tstd.lib.superRaycast;

#if UNITY_EDITOR

using UnityEditor;

#endif

public class MapCreator : MonoBehaviour {

	private enum MapType
	{
		MYPOS,
		MYBASE,
		OPPPOSE,
		OPPBASE,
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

	private static readonly float sqrt3 = Mathf.Sqrt (3);

	private MapUnit[] units;

	private MapData mapData;

	private MapType m_nowMapType;

	private MapType nowMapType{

		get{

			return m_nowMapType;
		}

		set{

			m_nowMapType = value;

			(btShow.transform as RectTransform).anchoredPosition = (bts [(int)m_nowMapType].transform as RectTransform).anchoredPosition;
		}
	}

	void Awake(){

		SuperRaycast.SetCamera (Camera.main);

		SuperRaycast.SetIsOpen (true, "a");
	}

	public void CreateMap(){

		choosePanel.SetActive (false);

		inputPanel.SetActive (true);
	}

	public void LoadMap(){

#if UNITY_EDITOR

		string path = EditorUtility.OpenFilePanel ("a", "a", "map");

		if (!string.IsNullOrEmpty (path)) {

			using(FileStream fs = new FileStream(path,FileMode.Open)){

				using(BinaryReader br = new BinaryReader(fs)){

					mapData = new MapData();

					mapData.GetData(br);

					choosePanel.SetActive (false);

					mapPanel.SetActive (true);

					CreateMapPanel ();
				}
			}
		}

#endif
	}

	public void ConfirmMapSize(){

		if (string.IsNullOrEmpty (widthField.text) || string.IsNullOrEmpty (heightField.text)) {

			return;
		}

		inputPanel.SetActive (false);

		mapPanel.SetActive (true);

		InitMapData (int.Parse (widthField.text),int.Parse (heightField.text));

		CreateMapPanel ();
	}

	private void InitMapData(int _mapWidth,int _mapHeight){
		
		mapData = new MapData (_mapWidth,_mapHeight);
		
		for (int i = 0; i < mapData.size; i++) {
			
			mapData.dic.Add(i,true);
		}
	}

	public void CreateMapPanel(){

		nowMapType = MapType.MYPOS;

		int size = mapData.mapWidth * mapData.mapHeight - mapData.mapHeight / 2;

		units = new MapUnit[size];

		int index = 0;

		for (int i = 0; i < mapData.mapHeight; i++) {

			for(int m = 0 ; m < mapData.mapWidth ; m++){

				if(i % 2 == 1 && m == mapData.mapWidth - 1){

					continue;
				}

				GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("MapUnit"));

				go.transform.SetParent(mapContainer,false);

				go.transform.localPosition = new Vector3(m * unitWidth * sqrt3 * 2 + ((i % 2 == 1) ? unitWidth * Mathf.Sqrt(3) : 0),-i * unitWidth * 3,0);

				go.transform.localScale = new Vector3(unitScale,unitScale,unitScale);

				MapUnit unit = go.GetComponent<MapUnit>();

				units[index] = unit;

				unit.index = index;
				
				if(mapData.dic.ContainsKey(index)){

					if(index == mapData.base1){

						unit.SetMainColor(bts[(int)MapType.MYBASE].color);

					}else if(index == mapData.base2){

						unit.SetMainColor(bts[(int)MapType.OPPBASE].color);

					}else if(mapData.dic[index]){

						unit.SetMainColor(bts[(int)MapType.MYPOS].color);

					}else{

						unit.SetMainColor(bts[(int)MapType.OPPPOSE].color);
					}

				}else{

					unit.SetMainColor(bts[(int)MapType.NULL].color);
				}

				index++;
			}
		}

		mapContainer.transform.localPosition = new Vector3 (-0.5f * (mapData.mapWidth * unitWidth * sqrt3 * 2) + unitWidth * sqrt3, 0.5f * (mapData.mapHeight * unitWidth * 3 + unitWidth) - unitWidth * 2, 0);
	}

	public void SaveMap(){

#if UNITY_EDITOR

		string path = EditorUtility.SaveFilePanel("a","a","","map");

		if (!string.IsNullOrEmpty (path)) {

			FileInfo fi = new FileInfo(path);

			if(fi.Exists){

				fi.Delete();
			}

			using(FileStream fs = fi.Create()){

				using(BinaryWriter bw = new BinaryWriter (fs)){

					mapData.SetData (bw);
				}
			}
		}
#endif
	}

	public void BtClick(int _index){

		nowMapType = (MapType)_index;
	}

	public void MapUnitUpAsButton(MapUnit _unit){

		_unit.SetMainColor (bts[(int)nowMapType].color);

		switch (nowMapType) {

		case MapType.NULL:

			if(mapData.dic.ContainsKey(_unit.index)){

				mapData.dic.Remove(_unit.index);
			}

			if(mapData.base1 == _unit.index){

				mapData.base1 = -1;
			}

			if(mapData.base2 == _unit.index){
				
				mapData.base2 = -1;
			}

			break;

		case MapType.MYPOS:

			if(mapData.dic.ContainsKey(_unit.index)){
				
				mapData.dic[_unit.index] = true;

			}else{

				mapData.dic.Add(_unit.index,true);
			}
			
			if(mapData.base1 == _unit.index){
				
				mapData.base1 = -1;
			}
			
			if(mapData.base2 == _unit.index){
				
				mapData.base2 = -1;
			}

			break;

		case MapType.OPPPOSE:
			
			if(mapData.dic.ContainsKey(_unit.index)){
				
				mapData.dic[_unit.index] = false;
				
			}else{
				
				mapData.dic.Add(_unit.index,false);
			}
			
			if(mapData.base1 == _unit.index){
				
				mapData.base1 = -1;
			}
			
			if(mapData.base2 == _unit.index){
				
				mapData.base2 = -1;
			}
			
			break;

		case MapType.MYBASE:
			
			if(mapData.dic.ContainsKey(_unit.index)){
				
				mapData.dic[_unit.index] = true;
				
			}else{
				
				mapData.dic.Add(_unit.index,true);
			}
			
			if(mapData.base1 != _unit.index){

				if(mapData.base1 != -1){

					units[mapData.base1].SetMainColor(bts[(int)MapType.MYPOS].color);
				}

				mapData.base1 = _unit.index;
			}
			
			if(mapData.base2 == _unit.index){
				
				mapData.base2 = -1;
			}
			
			break;

		case MapType.OPPBASE:
			
			if(mapData.dic.ContainsKey(_unit.index)){
				
				mapData.dic[_unit.index] = false;
				
			}else{
				
				mapData.dic.Add(_unit.index,false);
			}
			
			if(mapData.base1 == _unit.index){
				
				mapData.base1 = -1;
			}
			
			if(mapData.base2 != _unit.index){

				if(mapData.base2 != -1){
					
					units[mapData.base2].SetMainColor(bts[(int)MapType.OPPPOSE].color);
				}
				
				mapData.base2 = _unit.index;
			}
			
			break;
		}
	}
}
