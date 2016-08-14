using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using xy3d.tstd.lib.superTween;
using System;
using FinalWar;

public class BattleManager : MonoBehaviour {

	private static readonly Color summonColor = new Color (0.2f, 0.8f, 0.8f);

	private const float arrowZFix = -5;

	private const float mapUnitWidth = 30;
	private const float mapUnitScale = 55;
	private const float heroScale = 0.5f;
	private const float mapContainerYFix = 60;
	private static readonly float sqrt3 = Mathf.Sqrt (3);

	[SerializeField]
	private Color myMapUnitColor;

	[SerializeField]
	private Color oppMapUnitColor;

	[SerializeField]
	private GraphicRaycaster graphicRayCaster;

	[SerializeField]
	private RectTransform battleContainer;

	[SerializeField]
	private RectTransform cardContainer;

	[SerializeField]
	private RectTransform mapContainer;

	[SerializeField]
	private RectTransform heroContainer;

	[SerializeField]
	private RectTransform arrowContainer;

	[SerializeField]
	private Text moneyTf;

	[SerializeField]
	private GameObject actionBt;

	[SerializeField]
	private HeroDetail heroDetail;

	[SerializeField]
	private RoundNum roundNum;

	private Battle battle;

	private Dictionary<int,MapUnit> mapUnitDic = new Dictionary<int, MapUnit> ();

	private Dictionary<int,HeroCard> cardDic = new Dictionary<int, HeroCard>();

	private Dictionary<int,HeroCard> heroDic = new Dictionary<int, HeroCard>();

	private Dictionary<int,HeroCard> summonHeroDic = new Dictionary<int, HeroCard>();

	private List<GameObject> arrowList = new List<GameObject> ();

	private HeroCard m_nowChooseCard;

	private HeroCard nowChooseCard{

		get{

			return m_nowChooseCard;
		}

		set{

			if(value == null){

				heroDetail.Hide();

			}else{

				heroDetail.Init(value);
			}

			m_nowChooseCard = value;
		}
	}

	private HeroCard m_nowChooseHero;

	private HeroCard nowChooseHero{

		get{

			return m_nowChooseHero;
		}

		set{

			if(value == null){
				
				heroDetail.Hide();
				
			}else{
				
				heroDetail.Init(value);
			}

			m_nowChooseHero = value;
		}
	}

	private int movingHeroPos = -1;

	private void WriteLog(string _str){

		Debug.Log (_str);
	}

	// Use this for initialization
	void Start () {

		Log.Init (WriteLog);

		ConfigDictionary.Instance.LoadLocalConfig(Application.streamingAssetsPath + "/local.xml");
		
		StaticData.path = ConfigDictionary.Instance.table_path;
		
		StaticData.Load<MapSDS>("map");
		
		Map.Init();
		
		StaticData.Load<HeroSDS>("hero");
		
		Dictionary<int, HeroSDS> dic = StaticData.GetDic<HeroSDS>();
		
		Dictionary<int, IHeroSDS> newDic = new Dictionary<int, IHeroSDS>();
		
		foreach(KeyValuePair<int,HeroSDS> pair in dic)
		{
			newDic.Add(pair.Key, pair.Value);
		}
		
		Battle.Init(newDic,Map.mapDataDic);
		
		battle = new Battle ();

		battle.ClientSetCallBack (SendData, RefreshData, DoAction);
		
		Connection.Instance.Init ("127.0.0.1", 1983, ReceiveData);
	}
	
	private void ReceiveData(byte[] _bytes){

		battle.ClientGetPackage (_bytes);
	}

	private void SendData(MemoryStream _ms){

		Connection.Instance.Send (_ms);
	}

	private void RefreshData(){

		heroDetail.Hide();

		ClearMapUnits ();
		
		ClearCards ();

		ClearSummonHeros ();

		ClearHeros ();

		ClearMoves ();

		CreateMapPanel ();

		CreateCards ();

		CreateSummonHeros ();

		CreateHeros ();

		CreateMoves ();

		CreateMoneyTf ();

		RefreshTouchable ();
	}

	private void ClearMapUnits(){

		Dictionary<int,MapUnit>.ValueCollection.Enumerator enumerator = mapUnitDic.Values.GetEnumerator ();
		
		while (enumerator.MoveNext()) {
			
			GameObject.Destroy(enumerator.Current.gameObject);
		}
		
		mapUnitDic.Clear ();
	}

	private void ClearCards(){

		Dictionary<int,HeroCard>.ValueCollection.Enumerator enumerator2 = cardDic.Values.GetEnumerator ();
		
		while (enumerator2.MoveNext()) {
			
			GameObject.Destroy(enumerator2.Current.gameObject);
		}
		
		cardDic.Clear ();
	}

	private void ClearSummonHeros(){

		Dictionary<int,HeroCard>.ValueCollection.Enumerator enumerator2 = summonHeroDic.Values.GetEnumerator ();
		
		while (enumerator2.MoveNext()) {
			
			GameObject.Destroy(enumerator2.Current.gameObject);
		}
		
		summonHeroDic.Clear ();
	}

	private void ClearHeros(){

		Dictionary<int,HeroCard>.ValueCollection.Enumerator enumerator2 = heroDic.Values.GetEnumerator ();
		
		while (enumerator2.MoveNext()) {
			
			GameObject.Destroy(enumerator2.Current.gameObject);
		}
		
		heroDic.Clear ();
	}

	private void ClearMoves(){

		for(int i = 0 ; i < arrowList.Count ; i++){

			GameObject.Destroy(arrowList[i]);
		}

		arrowList.Clear ();
	}

	private void CreateMapPanel(){
		
		int index = 0;
		
		for (int i = 0; i < battle.mapData.mapHeight; i++) {
			
			for(int m = 0 ; m < battle.mapData.mapWidth ; m++){
				
				if(i % 2 == 1 && m == battle.mapData.mapWidth - 1){
					
					continue;
				}

				if(!battle.mapData.dic.ContainsKey(index)){

					index++;

					continue;
				}
				
				GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("MapUnit"));
				
				go.transform.SetParent(mapContainer,false);
				
				go.transform.localPosition = new Vector3(m * mapUnitWidth * sqrt3 * 2 + ((i % 2 == 1) ? mapUnitWidth * Mathf.Sqrt(3) : 0),-i * mapUnitWidth * 3,0);

				go.transform.localScale = new Vector3(mapUnitScale,mapUnitScale,mapUnitScale);

				MapUnit unit = go.GetComponent<MapUnit>();

				mapUnitDic.Add(index,unit);

				unit.index = index;
				
				if((battle.mapData.dic[index] == battle.clientIsMine) != battle.mapBelongDic.ContainsKey(index)){

					unit.SetMainColor(myMapUnitColor);

				}else{
					
					unit.SetMainColor(oppMapUnitColor);
				}
					
				index++;
			}
		}
		
		battleContainer.localPosition = new Vector3 (-0.5f * (battle.mapData.mapWidth * mapUnitWidth * sqrt3 * 2) + mapUnitWidth * sqrt3,mapContainerYFix + 0.5f * (battle.mapData.mapHeight * mapUnitWidth * 3 + mapUnitWidth) - mapUnitWidth * 2, 0);
	}

	private void CreateCards(){

		cardDic.Clear ();

		Dictionary<int,int> tmpCardDic = battle.clientIsMine ? battle.mHandCards : battle.oHandCards;

		Dictionary<int,int>.Enumerator enumerator = tmpCardDic.GetEnumerator ();

		int index = 0;

		while (enumerator.MoveNext()) {

			if(battle.summon.ContainsKey(enumerator.Current.Key)){

				continue;
			}

			GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Hero"));

			HeroCard hero = go.GetComponent<HeroCard>();

			hero.SetFrameVisible(false);

			hero.Init(enumerator.Current.Key,enumerator.Current.Value);

			cardDic.Add(enumerator.Current.Key,hero);

			go.transform.SetParent(cardContainer,false);

			float cardWidth = (go.transform as RectTransform).sizeDelta.x;
			float cardHeight = (go.transform as RectTransform).sizeDelta.y;

			(go.transform as RectTransform).anchoredPosition = new Vector2(-0.5f * cardContainer.rect.width + cardWidth * 0.5f + index * cardWidth,-0.5f * cardContainer.rect.height + cardHeight * 0.5f);

			index++;
		}
	}

	private void CreateSummonHeros(){

		Dictionary<int,int>.Enumerator enumerator2 = battle.summon.GetEnumerator ();
		
		while (enumerator2.MoveNext()) {
			
			AddCardToMap(enumerator2.Current.Key,enumerator2.Current.Value);
		}
	}

	private void CreateHeros(){

		Dictionary<int,Hero>.ValueCollection.Enumerator enumerator = battle.heroMapDic.Values.GetEnumerator ();

		while (enumerator.MoveNext()) {

			AddHeroToMap(enumerator.Current);
		}
	}

	private void CreateMoneyTf(){

		moneyTf.text = GetMoney().ToString ();
	}

	private void CreateMoves(){

		BattleData battleData = battle.GetBattleData ();

		Dictionary<int,int>.Enumerator enumerator = battleData.moveDic.GetEnumerator ();

		while (enumerator.MoveNext()) {

			CreateArrow(enumerator.Current.Key,enumerator.Current.Value,Color.green,0);
		}

		Dictionary<int,BattleCellData>.Enumerator enumerator2 = battleData.actionDic.GetEnumerator ();

		while (enumerator2.MoveNext()) {

			int pos = enumerator2.Current.Key;

			BattleCellData cellData = enumerator2.Current.Value;

			for(int i = 0 ; i < cellData.attackers.Count ; i++){

				CreateArrow(cellData.attackers[i].pos,pos,Color.red,i);
			}

			for(int i = 0 ; i < cellData.supporters.Count ; i++){

				CreateArrow(cellData.supporters[i].pos,pos,Color.blue,i);
			}

			for(int i = 0 ; i < cellData.shooters.Count ; i++){
				
				CreateShootArrow(cellData.shooters[i].pos,pos,Color.yellow);
			}
		}
	}

	private void CreateArrow(int _start,int _end,Color _color,int _index){

		GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Arrow"));
		
		Arrow arrow = go.GetComponent<Arrow>();
		
		go.transform.SetParent(arrowContainer,false);
		
		MapUnit start = mapUnitDic[_start];
		
		MapUnit end = mapUnitDic[_end];
		
		go.transform.position = (start.transform.position + end.transform.position) * 0.5f;
		
		float angle = Mathf.Atan2(end.transform.position.y - start.transform.position.y,end.transform.position.x - start.transform.position.x);
		
		Quaternion q = new Quaternion();
		
		q.eulerAngles = new Vector3(0,0,angle * 180 / Mathf.PI);
		
		go.transform.localRotation = q;
		
		arrow.SetColor (_color);

		arrow.SetIndex (_index);
		
		arrowList.Add(go);
	}

	private void CreateShootArrow(int _start,int _end,Color _color){

		GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("ShootArrow"));
		
		ShootArrow arrow = go.GetComponent<ShootArrow>();
		
		go.transform.SetParent(arrowContainer,false);
		
		MapUnit start = mapUnitDic[_start];
		
		MapUnit end = mapUnitDic[_end];
		
		go.transform.position = (start.transform.position + end.transform.position) * 0.5f;
		
		float angle = Mathf.Atan2(end.transform.position.y - start.transform.position.y,end.transform.position.x - start.transform.position.x);
		
		Quaternion q = new Quaternion();
		
		q.eulerAngles = new Vector3(0,0,angle * 180 / Mathf.PI);
		
		go.transform.localRotation = q;
		
		arrow.SetColor (_color);
		
		arrowList.Add(go);
	}

	public void MapUnitDown(MapUnit _mapUnit){

		if(nowChooseHero != null && nowChooseHero.pos != -1 && nowChooseHero.isMine == battle.clientIsMine){

			for(int i = 0 ; i < battle.action.Count ; i++){

				KeyValuePair<int,int> pair = battle.action[i];

				if(pair.Key == nowChooseHero.pos){

					if(pair.Value == _mapUnit.index){

						movingHeroPos = nowChooseHero.pos;
					}

					return;
				}
			}

			if(_mapUnit.index == nowChooseHero.pos){

				movingHeroPos = nowChooseHero.pos;
			}
		}
	}

	public void MapUnitEnter(MapUnit _mapUnit){

		if (movingHeroPos != -1) {

			List<int> tmpList = BattlePublicTools.GetNeighbourPos(battle.mapData.neighbourPosMap,movingHeroPos);

			if(tmpList.Contains(_mapUnit.index)){

				battle.ClientRequestAction(movingHeroPos,_mapUnit.index);
				
				ClearMoves();
				
				CreateMoves();

			}else{

				if((battle.mapData.dic[_mapUnit.index] == battle.clientIsMine) == battle.mapBelongDic.ContainsKey(_mapUnit.index)){

					tmpList = BattlePublicTools.GetNeighbourPos2(battle.mapData.neighbourPosMap,movingHeroPos);

					if(tmpList.Contains(_mapUnit.index)){

						battle.ClientRequestAction(movingHeroPos,_mapUnit.index);
						
						ClearMoves();
						
						CreateMoves();
					}
				}
			}
		}
	}

	public void MapUnitExit(MapUnit _mapUnit){

		if (movingHeroPos != -1) {

			for(int i = 0 ; i < battle.action.Count ; i++){
				
				KeyValuePair<int,int> pair = battle.action[i];
				
				if(pair.Key == movingHeroPos){

					battle.ClientRequestUnaction(movingHeroPos);

					ClearMoves();

					CreateMoves();
					
					return;
				}
			}
		}
	}

	public void MapUnitUp(MapUnit _mapUnit){

		if (movingHeroPos != -1) {

			movingHeroPos = -1;
		}
	}

	public void MapUnitUpAsButton(MapUnit _mapUnit){

		if (battle.summon.ContainsValue (_mapUnit.index)) {

			HeroCard summonHero = summonHeroDic [_mapUnit.index];

			if (nowChooseHero == null) {

				nowChooseHero = summonHero;

				nowChooseHero.SetFrameVisible (true);

			} else {

				if (nowChooseHero == summonHero) {

					nowChooseHero = null;

					UnsummonHero (summonHero.cardUid);

				} else {

					ClearNowChooseHero();

					nowChooseHero = summonHero;

					nowChooseHero.SetFrameVisible (true);
				}
			}
			
		} else if (battle.heroMapDic.ContainsKey (_mapUnit.index)) {

			HeroCard nowHero = heroDic [_mapUnit.index];
			
			if (nowChooseHero == null) {
				
				nowChooseHero = nowHero;
				
				nowChooseHero.SetFrameVisible (true);
				
			} else {
				
				if (nowChooseHero != nowHero) {

					ClearNowChooseHero();
					
					nowChooseHero = nowHero;
					
					nowChooseHero.SetFrameVisible (true);
				}
			}

		} else if(nowChooseCard != null) {

			if ((battle.mapData.dic [_mapUnit.index] == battle.clientIsMine) != battle.mapBelongDic.ContainsKey(_mapUnit.index) && nowChooseCard.sds.cost <= GetMoney ()) {
				
				SummonHero (nowChooseCard.cardUid, _mapUnit.index);
			}

		}else {

			ClearNowChooseHero();
		}

		ClearNowChooseCard ();
	}

	public void HeroClick(HeroCard _hero){

		ClearNowChooseHero();

		if (nowChooseCard != _hero) {

			ClearNowChooseCard();

			nowChooseCard = _hero;

			nowChooseCard.SetFrameVisible(true);
		}
	}

	private void ClearNowChooseCard(){

		if (nowChooseCard != null) {

			nowChooseCard.SetFrameVisible(false);

			nowChooseCard = null;
		}
	}

	private void ClearNowChooseHero(){

		if (nowChooseHero != null) {

			nowChooseHero.SetFrameVisible(false);

			nowChooseHero = null;
		}
	}

	private void SummonHero(int _cardUid,int _pos){
		
		battle.ClientRequestSummon (_cardUid, _pos);

		CreateMoneyTf ();

		ClearCards ();

		CreateCards ();

		ClearSummonHeros ();

		CreateSummonHeros ();

		ClearMoves ();

		CreateMoves ();
	}

	private void UnsummonHero(int _cardUid){

		battle.ClientRequestUnsummon (_cardUid);
		
		CreateMoneyTf ();
		
		ClearCards ();
		
		CreateCards ();
		
		ClearSummonHeros ();
		
		CreateSummonHeros ();

		ClearMoves ();
		
		CreateMoves ();
	}

	private void AddHeroToMap(Hero _hero){

		GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Hero"));
		
		HeroCard hero = go.GetComponent<HeroCard>();

		heroDic.Add (_hero.pos, hero);
		
		hero.Init (_hero.sds.GetID(), _hero.nowHp, _hero.pos, _hero.isMine);
		
		AddHeroToMapReal (hero, _hero.pos);
	}

	private void AddCardToMap(int _cardUid,int _pos){

		int cardID = (battle.clientIsMine ? battle.mHandCards : battle.oHandCards) [_cardUid];

		GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Hero"));
		
		HeroCard hero = go.GetComponent<HeroCard>();

		summonHeroDic.Add (_pos, hero);

		hero.body.color = summonColor;
		
		hero.Init(_cardUid, cardID);

		AddHeroToMapReal (hero, _pos);
	}

	private void AddHeroToMapReal(HeroCard _heroCard,int _pos){

		MapUnit mapUnit = mapUnitDic [_pos];
		
		_heroCard.SetFrameVisible(false);

		_heroCard.transform.SetParent (heroContainer, false);

		_heroCard.transform.localPosition = mapUnit.transform.localPosition;
		
//		_heroCard.transform.localPosition = Vector3.zero;
		
		_heroCard.transform.localScale = new Vector3 (heroScale, heroScale, heroScale);
		
		_heroCard.SetMouseEnable (false);
	}

	private int GetMoney(){

		int money = battle.clientIsMine ? battle.mMoney : battle.oMoney;

		Dictionary<int,int> cards = battle.clientIsMine ? battle.mHandCards : battle.oHandCards;

		Dictionary<int,int>.KeyCollection.Enumerator enumerator = battle.summon.Keys.GetEnumerator ();

		while (enumerator.MoveNext()) {

			int cardID = cards[enumerator.Current];

			HeroSDS heroSDS = StaticData.GetData<HeroSDS>(cardID);

			money -= heroSDS.cost;
		}

		return money;
	}

	public void ActionBtClick(){

		ClearNowChooseCard ();

		ClearNowChooseHero ();

		battle.ClientRequestDoAction ();

		RefreshTouchable ();
	}

	// Update is called once per frame
	void Update () {
	
		if(Input.GetKeyUp(KeyCode.F5)){

			battle.ClientRequestRefreshData();
		}
	}

	private void RefreshTouchable(){

		bool touchable = !(battle.clientIsMine ? battle.mOver : battle.oOver);

		graphicRayCaster.enabled = touchable;
		MapUnit.touchable = touchable;
		actionBt.SetActive (touchable);
	}

	private void DoAction(BinaryReader _br){

		DoSummonMyHero (_br);

//		battle.ClientDoSummonMyHero (_br);
//
//		battle.ClientDoSummonOppHero (_br);


		 
//		IEnumerator enumerator = battle.ClientDoAttack (_br);
//
//		while (enumerator.MoveNext()) {
//
//			Debug.Log(enumerator.Current);
//		}
//
//		battle.ClientDoRecover (_br);
	}

	private void DoSummonMyHero(BinaryReader _br){

		Debug.Log ("DoSummonMyHero!");

		int summonNum = battle.ClientDoSummonMyHero (_br);

		Debug.Log ("summonNum:" + summonNum);

		if (summonNum > 0) {

			RefreshData ();

			Action del = delegate() {

				DoSummonOppHero (_br);
			};

			SuperTween.Instance.DelayCall (1, del);

		} else {

			DoSummonOppHero (_br);
		}
	}

	private void DoSummonOppHero(BinaryReader _br){

		Debug.Log ("DoSummonOppHero!");

		int summonNum = battle.ClientDoSummonOppHero (_br);

		if (summonNum > 0) {

			RefreshData ();
			
			Action del = delegate() {
				
				DoMove (_br);
			};
			
			SuperTween.Instance.DelayCall (1, del);

		} else {

			DoMove (_br);
		}
	}

	private void DoMove(BinaryReader _br){

		battle.ClientDoRecover (_br);

		battle.ClientRequestRefreshData ();

//		Dictionary<int,int> moveDic = battle.ClientDoMove (_br);
//
//		if (moveDic.Count > 0) {
//
//			ClearMoves ();
//
//			Dictionary<int,int>.Enumerator enumerator = moveDic.GetEnumerator ();
//
//			bool hasCallBack = false;
//
//			while (enumerator.MoveNext()) {
//
//				int oldPos = enumerator.Current.Key;
//
//				int newPos = enumerator.Current.Value;
//
//				HeroCard hero = heroDic [oldPos];
//
//				Action<float> delX = delegate(float obj) {
//
//					hero.transform.localPosition = new Vector3 (obj, hero.transform.localPosition.y, hero.transform.localPosition.z);
//				};
//
//				Action<float> delY = delegate(float obj) {
//					
//					hero.transform.localPosition = new Vector3 (hero.transform.localPosition.x, obj, hero.transform.localPosition.z);
//				};
//
//				if(!hasCallBack){
//
//					hasCallBack = true;
//					
//					Action over = delegate() {
//
//						RefreshData();
//						
//						DoAttack(_br);
//					};
//
//					SuperTween.Instance.To (hero.transform.localPosition.x, mapUnitDic [newPos].transform.localPosition.x, 1, delX, over);
//					
//				}else{
//
//					SuperTween.Instance.To (hero.transform.localPosition.x, mapUnitDic [newPos].transform.localPosition.x, 1, delX, null);
//				}
//
//				SuperTween.Instance.To (hero.transform.localPosition.y, mapUnitDic [newPos].transform.localPosition.y, 1, delY, null);
//			}
//
//		} else {
//
//			DoAttack(_br);
//		}
	}

	private void DoAttack(BinaryReader _br){

//		IEnumerator enumerator = battle.ClientDoAttack (_br);
//
//		DoAttackReal (enumerator, _br);
	}

	private void DoAttackReal(IEnumerator _enumerator,BinaryReader _br){

		if (_enumerator.MoveNext ()) {

			if(_enumerator.Current is KeyValuePair<int,int>){

				DoRush(_enumerator,_br);

			}else if(_enumerator.Current is KeyValuePair<int, KeyValuePair<int, int>[]>){

				DoDamage(_enumerator,_br);

			}else if(_enumerator.Current is int){

				Action del = delegate() {

					DoAttackReal(_enumerator,_br);
				};

				RefreshData();

				roundNum.Move("Round " + _enumerator.Current,del);
			}

		} else {

			battle.ClientDoRecover (_br);

			_br.Close();
		}
	}

	private void DoRush(IEnumerator _enumerator,BinaryReader _br){

//		KeyValuePair<int,int> pair = (KeyValuePair<int,int>)_enumerator.Current;
//
//		GameObject go = GameObject.Instantiate<GameObject> (Resources.Load<GameObject> ("Attack"));
//
//		go.GetComponent<SpriteRenderer> ().color = Color.magenta;
//
//		go.transform.SetParent (arrowContainer, false);
//
//		go.transform.localPosition = new Vector3(mapUnitDic [pair.Key].transform.localPosition.x,mapUnitDic [pair.Key].transform.localPosition.y,arrowZFix);
//
//		go.transform.localScale = new Vector3 (mapUnitScale, mapUnitScale, mapUnitScale);
//
//		Action doNext = delegate() {
//
//			DoAttackReal(_enumerator,_br);
//		};
//
//		Action over = delegate() {
//
//			GameObject.Destroy(go);
//
//			heroDic[pair.Value].SetPower(battle.heroMapDic[pair.Value].nowPower);
//
//			SuperTween.Instance.DelayCall(1,doNext);
//		};
//
//		Action<float> delX = delegate(float obj) {
//
//			go.transform.localPosition = new Vector3(obj,go.transform.localPosition.y,go.transform.localPosition.z);
//		};
//
//		Action<float> delY = delegate(float obj) {
//			
//			go.transform.localPosition = new Vector3(go.transform.localPosition.x,obj,go.transform.localPosition.z);
//		};
//
//		Vector3 targetPos = mapUnitDic [pair.Value].transform.localPosition;
//
//		go.transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(targetPos.y - go.transform.localPosition.y,targetPos.x - go.transform.localPosition.x) * 180 / Mathf.PI);
//
//		SuperTween.Instance.To (go.transform.localPosition.x, targetPos.x, 1, delX, over);
//
//		SuperTween.Instance.To (go.transform.localPosition.y, targetPos.y, 1, delY, null);
	}

	private void DoDamage(IEnumerator _enumerator,BinaryReader _br){
		
		KeyValuePair<int, KeyValuePair<int, int>[]> pair = (KeyValuePair<int, KeyValuePair<int, int>[]>)_enumerator.Current;

		KeyValuePair<int,int>[] targets = pair.Value;

		for (int i = 0; i < targets.Length; i++) {

			GameObject go = GameObject.Instantiate<GameObject> (Resources.Load<GameObject> ("Attack"));

			go.GetComponent<SpriteRenderer> ().color = Color.black;
			
			go.transform.SetParent (arrowContainer, false);
			
			go.transform.localPosition = new Vector3(mapUnitDic [pair.Key].transform.localPosition.x,mapUnitDic [pair.Key].transform.localPosition.y,arrowZFix);
			
			go.transform.localScale = new Vector3 (mapUnitScale, mapUnitScale, mapUnitScale);

			Action<float> delX = delegate(float obj) {
				
				go.transform.localPosition = new Vector3 (obj, go.transform.localPosition.y, go.transform.localPosition.z);
			};
			
			Action<float> delY = delegate(float obj) {
				
				go.transform.localPosition = new Vector3 (go.transform.localPosition.x, obj, go.transform.localPosition.z);
			};
			
			Vector3 targetPos = mapUnitDic [targets[i].Key].transform.localPosition;

			go.transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(targetPos.y - go.transform.localPosition.y,targetPos.x - go.transform.localPosition.x) * 180 / Mathf.PI);

			int heroPos = targets[i].Key;

			int damage = targets[i].Value;

			Action over;

			if(i == 0){

				Action doNext = delegate() {

					DoAttackReal (_enumerator, _br);
				};
			
				over = delegate() {
					
					GameObject.Destroy (go);
					
					heroDic [heroPos].SetHp (battle.heroMapDic [heroPos].nowHp);

					GameObject damageGo = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("DamageNum"));

					damageGo.transform.SetParent(arrowContainer,false);

					damageGo.transform.localPosition = heroDic [heroPos].transform.localPosition;

					DamageNum damageNum = damageGo.GetComponent<DamageNum>();

					damageNum.Init(-damage,doNext);

					RefreshData();
					
//					SuperTween.Instance.DelayCall(1,doNext);
				};

			}else{

				over = delegate() {
					
					GameObject.Destroy (go);
					
					heroDic [heroPos].SetHp (battle.heroMapDic [heroPos].nowHp);

					GameObject damageGo = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("DamageNum"));

					damageGo.transform.SetParent(arrowContainer,false);
					
					damageGo.transform.localPosition = heroDic [heroPos].transform.localPosition;
					
					DamageNum damageNum = damageGo.GetComponent<DamageNum>();
					
					damageNum.Init(-damage,null);
				};
			}

			SuperTween.Instance.To (go.transform.localPosition.x, targetPos.x, 1, delX, over);

			SuperTween.Instance.To (go.transform.localPosition.y, targetPos.y, 1, delY, null);
		}
	}
}
