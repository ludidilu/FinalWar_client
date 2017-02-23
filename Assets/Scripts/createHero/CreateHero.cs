using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR

using UnityEditor;

#endif
using System.IO;

public class CreateHero : MonoBehaviour {

	[SerializeField]
	private int[] aScore;

	[SerializeField]
	private int[] cScore;

	[SerializeField]
	private int aNum;

	[SerializeField]
	private int cNum;

	private int startID = 1000000;

	private string heroStr = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}";

	private Dictionary<int,AttSDS> attDic;

	// Use this for initialization
	void Start () {

		ConfigDictionary.Instance.LoadLocalConfig(Application.streamingAssetsPath + "/local.xml");
		
		StaticData.path = ConfigDictionary.Instance.table_path;
		
		StaticData.Dispose();
		
		StaticData.Load<AttSDS>("att");

		attDic = StaticData.GetDic<AttSDS>();

		int tmpID = startID;

		string str = string.Empty;

		int maxNum = (aNum + cNum) * 5;

		for (int i = 0; i < maxNum; i++) {

			str += tmpID;

			tmpID++;

			if(i != maxNum - 1){

				str += "$";
			}
		}

		Debug.Log (str);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Click(){

		int tmpID = startID;

		string result = string.Empty;

		for(int m = 0 ; m < 5 ; m++){

			for (int i = 0; i < aNum; i++) {

				int id = tmpID;

				tmpID++;

				string name = "A" + (m + 1) + "_" + (i + 1);

				string str = GetOneHero(id,false,m,name);

				result = result + str + "\r\n";
			}

			for (int i = 0; i < cNum; i++) {
				
				int id = tmpID;
				
				tmpID++;

				string name = "C" + (m + 1) + "_" + (i + 1);
				
				string str = GetOneHero(id,true,m,name);
				
				result = result + str + "\r\n";
			}
		}

#if UNITY_EDITOR
		
		string path = EditorUtility.SaveFilePanel("a", "a", "", "csv");
		
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
					bw.Write(Encoding.UTF8.GetBytes(result));
				}
			}
		}
		
#endif
	}

	private string GetOneHero(int _id,bool _canControl,int _level,string _name){

		int id = _id;
		
		int score = _canControl ? cScore[_level] : aScore[_level];
		
		int ability = (int)(UnityEngine.Random.value * attDic.Count);

		AttSDS attSDS = attDic[ability];
		
		score -= attSDS.hp;
		
		int hp = 1;
		
		int shield = 0;
		
		int attack = 0;
		
		while(score > 0){
			
			List<int> tmpList = new List<int>();
			
			if(score >= attSDS.shield && shield < 9){
				
				tmpList.Add(0);
			}
			
			if(score >= attSDS.hp && hp < 9){
				
				tmpList.Add(1);
			}
			
			if(score >= attSDS.attack && attack < 9){
				
				tmpList.Add(2);
			}
			
			if(tmpList.Count == 0){
				
				break;
			}
			
			int index = (int)(UnityEngine.Random.value * tmpList.Count);
			
			int att = tmpList[index];
			
			if(att == 0){
				
				shield++;
				
				score -= attSDS.shield;
				
			}else if(att == 1){
				
				hp++;
				
				score -= attSDS.hp;
				
			}else{
				
				attack++;
				
				score -= attSDS.attack;
			}
		}

		string str = string.Format(heroStr,id,_name,_canControl ? "1" : "0",hp,shield,attack,ability,_level + 1,string.Empty,string.Empty,string.Empty);

		return str;
	}
}
