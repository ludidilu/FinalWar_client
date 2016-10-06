using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using xy3d.tstd.lib.superTween;
using System;
using FinalWar;
using xy3d.tstd.lib.superRaycast;
using xy3d.tstd.lib.screenScale;
using xy3d.tstd.lib.superFunction;
using xy3d.tstd.lib.publicTools;

public class BattleManager : MonoBehaviour {

	public static readonly Color threatColor = new Color (0.2f, 0.8f, 0.8f);

	private const float arrowZFix = -5;

	private const float mapUnitWidth = 30;
	private const float mapUnitScale = 55;
	private const float heroScale = 0.8f;
	private const float mapContainerYFix = 60;
	private static readonly float sqrt3 = Mathf.Sqrt (3);
	private const float scaleStep = 0.95f;
	private const float minScale = 0.7f;
	private const float maxScale = 1.3f;

	[SerializeField]
	private Camera mainCamera;

	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private Color myMapUnitColor;

	[SerializeField]
	private Color myBaseColor;

	[SerializeField]
	private Color oppMapUnitColor;

	[SerializeField]
	private Color oppBaseColor;

	[SerializeField]
	private GraphicRaycaster graphicRayCaster;

	[SerializeField]
	private RectTransform battleContainer;

	[SerializeField]
	private RectTransform battleContentContainer;

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
	private GameObject backGround;

	private Battle battle;

	private Dictionary<int, MapUnit> mapUnitDic = new Dictionary<int, MapUnit> ();

	private Dictionary<int, HeroCard> cardDic = new Dictionary<int, HeroCard>();

	private Dictionary<int, HeroBattle> heroDic = new Dictionary<int, HeroBattle>();

	private Dictionary<int, HeroBattle> summonHeroDic = new Dictionary<int, HeroBattle>();

	private List<GameObject> arrowList = new List<GameObject> ();

	private Vector2 downPos;

	private Vector2 lastPos;

	private enum DownType
	{
		NULL,
		BACKGROUND,
		MAPUNIT
	}

	private DownType isDown = DownType.NULL;

	private bool hasMove = false;

	private MapUnit downMapUnit;

	private HeroCard m_nowChooseCard;

	private HeroCard GetNowChooseCard(){

		return m_nowChooseCard;
	}

	private void SetNowChooseCard(HeroCard _value){

		if(_value == null){
			
			heroDetail.Hide(m_nowChooseCard);
			
		}else{
			
			heroDetail.Show(_value);
		}
		
		m_nowChooseCard = _value;
	}

	private HeroBattle m_nowChooseHero;

	private HeroBattle GetNowChooseHero(){

		return  m_nowChooseHero;
	}

	private void SetNowChooseHero(HeroBattle _value,bool _canAction){

		if(_value == null){
			
			heroDetail.Hide(m_nowChooseHero);
			
		}else{
			
			heroDetail.Show(_value);
		}
		
		m_nowChooseHero = _value;

		nowChooseHeroCanAction = _canAction;
	}

	private bool nowChooseHeroCanAction = false;

	private int movingHeroPos = -1;

	private bool mouseHasExited = false;

	private void WriteLog(string _str){

		Debug.Log (_str);
	}

	// Use this for initialization
	void Start () {

		Log.Init (WriteLog);

		SuperRaycast.SetCamera (mainCamera);

		SuperRaycast.SetIsOpen (true, "a");

		SuperRaycast.checkBlockByUi = true;

		ConfigDictionary.Instance.LoadLocalConfig (Application.streamingAssetsPath + "/local.xml");
		
		StaticData.path = ConfigDictionary.Instance.table_path;

		StaticData.Dispose ();

		StaticData.Load<MapSDS> ("map");
		
		Map.Init ();
		
		StaticData.Load<HeroSDS> ("hero");
		
		Dictionary<int, HeroSDS> heroDic = StaticData.GetDic<HeroSDS> ();
		
		Dictionary<int, IHeroSDS> newHeroDic = new Dictionary<int, IHeroSDS> ();
		
		foreach (KeyValuePair<int,HeroSDS> pair in heroDic) {
			newHeroDic.Add (pair.Key, pair.Value);
		}

		StaticData.Load<SkillSDS> ("skill");

		Dictionary<int, SkillSDS> skillDic = StaticData.GetDic<SkillSDS> ();

		Dictionary<int, ISkillSDS> newSkillDic = new Dictionary<int, ISkillSDS> ();

		foreach (KeyValuePair<int,SkillSDS> pair in skillDic) {

			newSkillDic.Add (pair.Key, pair.Value);
		}

		StaticData.Load<AuraSDS> ("aura");
		
		Dictionary<int, AuraSDS> auraDic = StaticData.GetDic<AuraSDS> ();
		
		Dictionary<int, IAuraSDS> newAuraDic = new Dictionary<int, IAuraSDS> ();
		
		foreach (KeyValuePair<int,AuraSDS> pair in auraDic) {
			
			newAuraDic.Add (pair.Key, pair.Value);
		}
		
		Battle.Init (Map.mapDataDic, newHeroDic, newSkillDic, newAuraDic);
		
		battle = new Battle ();

		battle.ClientSetCallBack (SendData, RefreshData, DoAction);
		
		Connection.Instance.Init ("127.0.0.1", 1983, ReceiveData, ConfigDictionary.Instance.uid);

		InitUi ();
	}

	private void InitUi(){

		SuperFunction.Instance.AddEventListener (ScreenScale.Instance.go, ScreenScale.SCALE_CHANGE, ScaleChange);

		SuperFunction.Instance.AddEventListener (backGround, SuperRaycast.GetMouseButtonDown, GetMouseDown);
		
		SuperFunction.Instance.AddEventListener (backGround, SuperRaycast.GetMouseButton, GetMouseMove);

		SuperFunction.Instance.AddEventListener (backGround, SuperRaycast.GetMouseButtonUp, GetMouseUp);
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

		Dictionary<int,HeroBattle>.ValueCollection.Enumerator enumerator2 = summonHeroDic.Values.GetEnumerator ();
		
		while (enumerator2.MoveNext()) {
			
			GameObject.Destroy(enumerator2.Current.gameObject);
		}
		
		summonHeroDic.Clear ();
	}

	private void ClearHeros(){

		Dictionary<int,HeroBattle>.ValueCollection.Enumerator enumerator2 = heroDic.Values.GetEnumerator ();
		
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

		GameObject newGo = new GameObject ();
		
		newGo.transform.SetParent (mapContainer, false);

		MeshRenderer mr = newGo.AddComponent<MeshRenderer> ();

		mr.material = new Material (Shader.Find("Unlit/MapUnit"));

		GameObject[] gos = new GameObject[battle.mapData.dic.Count];
		
		int index = 0;

		int index2 = 0;
		
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

				unit.Init(index,index2,mr);

				if((battle.mapData.dic[index] == battle.clientIsMine) != battle.mapBelongDic.ContainsKey(index)){

					if((!battle.clientIsMine && index == battle.mapData.base2) || (battle.clientIsMine && index == battle.mapData.base1)){

						unit.SetMainColor(myBaseColor);

					}else{

						unit.SetMainColor(myMapUnitColor);
					}

				}else{

					if((!battle.clientIsMine && index == battle.mapData.base1) || (battle.clientIsMine && index == battle.mapData.base2)){
						
						unit.SetMainColor(oppBaseColor);
						
					}else{
						
						unit.SetMainColor(oppMapUnitColor);
					}
				}
					
				index++;

				gos[index2] = go;

				index2++;
			}
		}

		Mesh mesh = PublicTools.CombineMeshs (gos);

		MeshFilter mf = newGo.AddComponent<MeshFilter> ();

		mf.mesh = mesh;

		battleContentContainer.localPosition = new Vector3 (-0.5f * (battle.mapData.mapWidth * mapUnitWidth * sqrt3 * 2) + mapUnitWidth * sqrt3,mapContainerYFix + 0.5f * (battle.mapData.mapHeight * mapUnitWidth * 3 + mapUnitWidth) - mapUnitWidth * 2, 0);
	}

	private void CreateCards(){

		Dictionary<int,int> tmpCardDic = battle.clientIsMine ? battle.mHandCards : battle.oHandCards;

		Dictionary<int,int>.Enumerator enumerator = tmpCardDic.GetEnumerator ();

		int index = 0;

		while (enumerator.MoveNext()) {

			if(battle.summon.ContainsKey(enumerator.Current.Key)){

				continue;
			}

			GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("HeroCard"));

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

		if (!moneyTf.gameObject.activeSelf) {

			moneyTf.gameObject.SetActive(true);
		}

		moneyTf.text = GetMoney().ToString ();
	}

	private void CreateMoves(){

		BattleData battleData = battle.GetBattleData ();

		Dictionary<int,BattleCellData>.Enumerator enumerator2 = battleData.actionDic.GetEnumerator ();

		while (enumerator2.MoveNext()) {

			int pos = enumerator2.Current.Key;

			BattleCellData cellData = enumerator2.Current.Value;

			for(int i = 0 ; i < cellData.attackers.Count ; i++){

				CreateArrow(cellData.attackers[i].pos,pos,Color.red,i);
			}

			for(int i = 0 ; i < cellData.supporters.Count ; i++){

				CreateArrow(cellData.supporters[i].pos,pos,Color.green,i);
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

		if(mouseHasExited){
			
			mouseHasExited = false;
		}

		if(nowChooseHeroCanAction){

			for(int i = 0 ; i < battle.action.Count ; i++){

				KeyValuePair<int,int> pair = battle.action[i];

				if(pair.Key == GetNowChooseHero().pos){

					if(pair.Value == _mapUnit.index){

						movingHeroPos = GetNowChooseHero().pos;

					}else{

						MapUnitDownReal(_mapUnit);
					}

					return;
				}
			}

			if(_mapUnit.index == GetNowChooseHero().pos){

				movingHeroPos = GetNowChooseHero().pos;
			}
		}

		if (movingHeroPos == -1) {

			MapUnitDownReal(_mapUnit);
		}
	}

	public void MapUnitEnter(MapUnit _mapUnit){

		if (movingHeroPos != -1) {

			if(battle.ClientRequestAction(movingHeroPos,_mapUnit.index)){

				ClearMoves();
				
				CreateMoves();
			}
		}
	}

	public void MapUnitExit(MapUnit _mapUnit){

		if (movingHeroPos != -1) {

			mouseHasExited = true;

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

		if (mouseHasExited) {

			return;
		}

		if (battle.summon.ContainsValue (_mapUnit.index)) {

			HeroBattle summonHero = summonHeroDic [_mapUnit.index];

			if (GetNowChooseHero() == null) {

				SetNowChooseHero(summonHero,false);

				GetNowChooseHero().SetFrameVisible (true);

			} else {

				if (GetNowChooseHero() == summonHero) {

					ClearNowChooseHero();

					UnsummonHero (summonHero.cardUid);

				} else {

					ClearNowChooseHero();

					SetNowChooseHero(summonHero,false);

					GetNowChooseHero().SetFrameVisible (true);
				}
			}
			
		} else if (battle.heroMapDic.ContainsKey (_mapUnit.index)) {

			HeroBattle nowHero = heroDic [_mapUnit.index];

			if (GetNowChooseHero() == null) {

				SetNowChooseHero(nowHero,nowHero.isMine == battle.clientIsMine);

				GetNowChooseHero().SetFrameVisible (true);
				
			} else {
				
				if (GetNowChooseHero() != nowHero) {

					ClearNowChooseHero();

					SetNowChooseHero(nowHero,nowHero.isMine == battle.clientIsMine);

					GetNowChooseHero().SetFrameVisible (true);
				}
			}

		} else if(GetNowChooseCard() != null) {

			if ((battle.mapData.dic [_mapUnit.index] == battle.clientIsMine) != battle.mapBelongDic.ContainsKey(_mapUnit.index) && GetNowChooseCard().sds.cost <= GetMoney ()) {
				
				SummonHero (GetNowChooseCard().cardUid, _mapUnit.index);
			}

		}else {

			ClearNowChooseHero();
		}

		ClearNowChooseCard ();
	}

	public void BackgroundClick(){

		ClearNowChooseHero ();

		ClearNowChooseCard ();
	}

	public void HeroClick(HeroCard _hero){

		ClearNowChooseHero();

		if (GetNowChooseCard() != _hero) {

			ClearNowChooseCard();

			SetNowChooseCard(_hero);

			GetNowChooseCard().SetFrameVisible(true);
		}
	}

	private void ClearNowChooseCard(){

		if (GetNowChooseCard() != null) {

			GetNowChooseCard().SetFrameVisible(false);

			SetNowChooseCard(null);
		}
	}

	private void ClearNowChooseHero(){

		if (GetNowChooseHero() != null) {

			GetNowChooseHero().SetFrameVisible(false);

			SetNowChooseHero(null,false);
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

	private HeroBattle AddHeroToMap(Hero _hero){

		GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("HeroBattle"));
		
		HeroBattle hero = go.GetComponent<HeroBattle>();

		heroDic.Add (_hero.pos, hero);
		
		hero.Init (_hero);
		
		AddHeroToMapReal (hero, _hero.pos);

		return hero;
	}

	private HeroBattle AddCardToMap(int _cardUid,int _pos){

		int cardID = (battle.clientIsMine ? battle.mHandCards : battle.oHandCards) [_cardUid];

		GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("HeroBattle"));
		
		HeroBattle hero = go.GetComponent<HeroBattle>();

		summonHeroDic.Add (_pos, hero);

		HeroSDS sds = StaticData.GetData<HeroSDS> (cardID);
		
		hero.Init(cardID);

		hero.cardUid = _cardUid;

		AddHeroToMapReal (hero, _pos);

		return hero;
	}

	private void AddHeroToMapReal(HeroBattle _heroCard,int _pos){

		MapUnit mapUnit = mapUnitDic [_pos];
		
		_heroCard.SetFrameVisible(false);

		_heroCard.transform.SetParent (heroContainer, false);

		_heroCard.transform.localPosition = mapUnit.transform.localPosition;
		
		_heroCard.transform.localScale = new Vector3 (heroScale, heroScale, heroScale);
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
	
		if (Input.GetKeyUp (KeyCode.F5)) {

			battle.ClientRequestRefreshData ();

		} else if (Input.GetKeyUp (KeyCode.A)) {

			HeroAi.Start(battle,battle.clientIsMine,0);

			ClearMoves();

			CreateMoves();

			ClearSummonHeros();

			CreateSummonHeros();

			ClearCards();

			CreateCards();
		}
	}

	private void RefreshTouchable(){

		bool touchable = !(battle.clientIsMine ? battle.mOver : battle.oOver);

		graphicRayCaster.enabled = touchable;

		if (SuperRaycast.GetIsOpen () != touchable) {

			SuperRaycast.SetIsOpen (touchable, "a");
		}

		actionBt.SetActive (touchable);
	}

	private void DoAction(IEnumerator<ValueType> _enumerator){

		RefreshData ();

		DoActionReal (_enumerator);
	}

	private void DoActionReal(IEnumerator<ValueType> _enumerator){

		if (_enumerator.MoveNext ()) {
			
			Action del = delegate() {
				
				DoActionReal(_enumerator);
			};

			ValueType vo = _enumerator.Current;

			if(vo is BattleShootVO){

				DoShoot((BattleShootVO)(vo),del);

			}else if(vo is BattleMoveVO){

				DoMove((BattleMoveVO)vo,del);

			}else if(vo is BattleRushVO){

				DoRush((BattleRushVO)vo,del);

			}else if(vo is BattleAttackVO){

				DoAttack((BattleAttackVO)vo,del);

			}else if(vo is BattleDeathVO){

				DoDie((BattleDeathVO)vo,del);

			}else if (vo is BattleSummonVO){

				DoSummon((BattleSummonVO)vo,del);

			}else if(vo is BattlePowerChangeVO){

				DoPowerChange((BattlePowerChangeVO)vo,del);

			}else if(vo is BattleHpChangeVO){

				DoHpChange((BattleHpChangeVO)vo,del);
			}
		}
	}

	private void DoSummon(BattleSummonVO _vo,Action _del){

		CreateMoneyTf ();

		Hero hero = battle.heroMapDic [_vo.pos];

		HeroBattle heroBattle = AddHeroToMap (hero);

		heroBattle.transform.localScale = Vector3.zero;

		Action<float> toDel = delegate(float obj) {

			float scale = heroScale * obj;

			heroBattle.transform.localScale = new Vector3(scale,scale,scale);
		};

		Action endDel = delegate() {

			SuperTween.Instance.DelayCall(0.5f,_del);
		};

		SuperTween.Instance.To (10, 1, 0.5f, toDel, endDel);

		ClearCards ();

		CreateCards ();
	}

	private void DoShoot(BattleShootVO _vo,Action _del){

		List<HeroBattle> shooters = new List<HeroBattle>();
		
		for(int i = 0 ; i < _vo.shooters.Count ; i++){
			
			shooters.Add(heroDic[_vo.shooters[i]]);
		}
		
		BattleControl.Instance.Shoot(shooters,heroDic[_vo.stander],_vo.damage,_del);
	}

	private void DoMove(BattleMoveVO _vo,Action _del){

		if (_vo.moves.Count > 0) {

			List<KeyValuePair<int,int>> tmpList = new List<KeyValuePair<int,int>>();

			List<KeyValuePair<int,int>> tmpList2 = new List<KeyValuePair<int,int>>();

			Dictionary<int,int>.Enumerator enumerator = _vo.moves.GetEnumerator();

			enumerator.MoveNext();

			tmpList.Add(enumerator.Current);

			while(tmpList.Count > 0){

				KeyValuePair<int,int> pair = tmpList[0];

				_vo.moves.Remove(pair.Key);

				tmpList.RemoveAt(0);

				tmpList2.Add(pair);

				if(_vo.moves.ContainsKey(pair.Value)){

					tmpList.Add(new KeyValuePair<int, int>(pair.Value,_vo.moves[pair.Value]));
				}

				if(_vo.moves.ContainsValue(pair.Key)){

					enumerator = _vo.moves.GetEnumerator();

					while(enumerator.MoveNext()){

						if(enumerator.Current.Value == pair.Key){

							tmpList.Add(enumerator.Current);

							break;
						}
					}
				}
			}

			Dictionary<int,HeroBattle> tmpDic = new Dictionary<int, HeroBattle>();

			for(int i = 0 ; i < tmpList2.Count ; i++){

				KeyValuePair<int,int> pair = tmpList2[i];

				HeroBattle hero = heroDic[pair.Key];

				tmpDic.Add(pair.Key,hero);

				heroDic.Remove(pair.Key);

				Vector3 startPos = mapUnitDic[pair.Key].transform.position;

				Vector3 endPos = mapUnitDic[pair.Value].transform.position;

				Action<float> toDel = delegate(float obj) {

					hero.transform.position = Vector3.Lerp(startPos,endPos,obj);
				};

				if(i == 0){
					
					Action del = delegate() {

						for(int l = 0 ; l < tmpList2.Count ; l++){

							pair = tmpList2[l];

							heroDic.Add(pair.Value,tmpDic[pair.Key]);

							int index = pair.Value;

							MapUnit unit = mapUnitDic[index];

							if((battle.mapData.dic[index] == battle.clientIsMine) != battle.mapBelongDic.ContainsKey(index)){
								
								unit.SetMainColor(myMapUnitColor);
								
							}else{
								
								unit.SetMainColor(oppMapUnitColor);
							}
						}
						
						DoMove(_vo,_del);
					};

					SuperTween.Instance.To(0,1,1,toDel,del);

				}else{

					SuperTween.Instance.To(0,1,1,toDel,null);
				}
			}

		} else {

			_del();
		}
	}

	private void DoRush(BattleRushVO _vo,Action _del){

		List<HeroBattle> attackers = new List<HeroBattle> ();

		HeroBattle stander = heroDic [_vo.stander];

		for(int i = 0 ; i < _vo.attackers.Count ; i++){

			attackers.Add(heroDic[_vo.attackers[i]]);
		}

		BattleControl.Instance.Rush (attackers, stander, _vo.damage, _del);
	}

	private void DoAttack(BattleAttackVO _vo,Action _del){

		Vector3 pos = mapUnitDic [_vo.defender].transform.position;

		List<HeroBattle> attackers = new List<HeroBattle> ();

		List<HeroBattle> supporters = new List<HeroBattle> ();

		List<int> attackersDamage = new List<int> ();

		List<int> supportersDamage = new List<int> ();

		for (int i = 0; i < _vo.attackers.Count; i++) {

			attackers.Add(heroDic[_vo.attackers[i]]);

			attackersDamage.Add(_vo.attackersDamage[i]);
		}

		for (int i = 0; i < _vo.supporters.Count; i++) {

			supporters.Add(heroDic[_vo.supporters[i]]);
			
			supportersDamage.Add(_vo.supportersDamage[i]);
		}

		HeroBattle defender;

		if (heroDic.ContainsKey (_vo.defender)) {

			defender = heroDic [_vo.defender];

		} else {

			defender = null;
		}

		BattleControl.Instance.Attack (attackers, pos, defender, supporters, _vo.defenderDamage, supportersDamage, attackersDamage, _del);
	}

	private void DoDie(BattleDeathVO _vo,Action _del){

		for(int i = 0 ; i < _vo.deads.Count ; i++){

			int pos = _vo.deads[i];

			HeroBattle hero = heroDic[pos];

			heroDic.Remove(pos);

			if(i == 0){

				hero.Die(_del);

			}else{

				hero.Die(null);
			}
		}
	}

	private void DoPowerChange(BattlePowerChangeVO _vo,Action _del){

		for(int i = 0 ; i < _vo.pos.Count ; i++){

			int pos = _vo.pos[i];

			int powerChange = _vo.powerChange[i];

			bool isDizz = _vo.isDizz[i];

			HeroBattle hero = heroDic[pos];

			hero.RefreshPower();

			string str;

			Color color;

			if(powerChange > 0){

				str = "+" + ((int)(powerChange / 100)).ToString();

				color = Color.blue;

			}else{

				if(isDizz){

					str = ((int)(powerChange / 100)).ToString() + "  混乱";

				}else{

					str = ((int)(powerChange / 100)).ToString();
				}

				color = Color.yellow;
			}

			if(i == 0){

				hero.ShowHud(str,color,_del);

			}else{

				hero.ShowHud(str,color,null);
			}
		}
	}

	private void DoHpChange(BattleHpChangeVO _vo,Action _del){

		for(int i = 0 ; i < _vo.pos.Count ; i++){
			
			int pos = _vo.pos[i];
			
			int hpChange = _vo.hpChange[i];
			
			HeroBattle hero = heroDic[pos];
			
			hero.RefreshHp();
			
			string str;
			
			Color color;
			
			if(hpChange > 0){
				
				str = "+" + hpChange.ToString();
				
				color = Color.green;
				
			}else{
				
				str = hpChange.ToString();
				
				color = Color.red;
			}
			
			if(i == 0){
				
				hero.ShowHud(str,color,_del);
				
			}else{
				
				hero.ShowHud(str,color,null);
			}
		}
	}

	private void FixBattleContainerRect(){
		
		if(battleContainer.localScale.x < 1){
			
			battleContainer.anchoredPosition = Vector2.zero;
			
		}else{
			
			if(battleContainer.anchoredPosition.x - (canvas.transform as RectTransform).rect.width / 2 * battleContainer.localScale.x > -(canvas.transform as RectTransform).rect.width / 2){
				
				battleContainer.anchoredPosition = new Vector2(-(canvas.transform as RectTransform).rect.width / 2 + (canvas.transform as RectTransform).rect.width / 2 * battleContainer.localScale.x,battleContainer.anchoredPosition.y);
				
			}else if(battleContainer.anchoredPosition.x + (canvas.transform as RectTransform).rect.width / 2 * battleContainer.localScale.x < (canvas.transform as RectTransform).rect.width / 2){
				
				battleContainer.anchoredPosition = new Vector2((canvas.transform as RectTransform).rect.width / 2 - (canvas.transform as RectTransform).rect.width / 2 * battleContainer.localScale.x,battleContainer.anchoredPosition.y);
			}
			
			if(battleContainer.anchoredPosition.y - (canvas.transform as RectTransform).rect.height / 2 * battleContainer.localScale.x > -(canvas.transform as RectTransform).rect.height / 2){
				
				battleContainer.anchoredPosition = new Vector2(battleContainer.anchoredPosition.x,-(canvas.transform as RectTransform).rect.height / 2 + (canvas.transform as RectTransform).rect.height / 2 * battleContainer.localScale.x);
				
			}else if(battleContainer.anchoredPosition.y + (canvas.transform as RectTransform).rect.height / 2 * battleContainer.localScale.x < (canvas.transform as RectTransform).rect.height / 2){
				
				battleContainer.anchoredPosition = new Vector2(battleContainer.anchoredPosition.x,(canvas.transform as RectTransform).rect.height / 2 - (canvas.transform as RectTransform).rect.height / 2 * battleContainer.localScale.x);
			}
		}
	}

	private void ScaleChange(SuperEvent e){

		float scrollValue = (float)e.data [0];

		Vector2 mousePosition = (Vector2)e.data [1];

		if(scrollValue < 1){
			
			Vector2 v = PublicTools.MousePositionToCanvasPosition(canvas,mousePosition);
			
			Vector2 v2 = (v - battleContainer.anchoredPosition) / battleContainer.localScale.x;
			
			battleContainer.localScale = battleContainer.localScale * scaleStep;

			if(battleContainer.localScale.x < minScale){

				battleContainer.localScale = new Vector3(minScale,minScale,minScale);
			}
			
			battleContainer.anchoredPosition = v - v2 * battleContainer.localScale.x;
			
			FixBattleContainerRect();
			
		}else if(scrollValue > 1){
			
			Vector2 v = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);
			
			Vector2 v2 = (v - battleContainer.anchoredPosition) / battleContainer.localScale.x;
			
			battleContainer.localScale = battleContainer.localScale / scaleStep;

			if(battleContainer.localScale.x > maxScale){
				
				battleContainer.localScale = new Vector3(maxScale,maxScale,maxScale);
			}
			
			battleContainer.anchoredPosition = v - v2 * battleContainer.localScale.x;
		}
	}

	private void GetMouseDown(SuperEvent e){
		
		if((int)e.data[1] == 0){
			
			BackgroundDown();
		}
	}

	private void BackgroundDown(){

		downPos = lastPos = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);

		isDown = DownType.BACKGROUND;
	}

	private void MapUnitDownReal(MapUnit _mapUnit){

		downPos = lastPos = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);
		
		isDown = DownType.MAPUNIT;

		downMapUnit = _mapUnit;
	}
	
	private void GetMouseMove(SuperEvent e){

		if (isDown != DownType.NULL) {
		
			BackgroundMove ();
		}
	}

	private void BackgroundMove(){
			
		Vector3 nowPos = PublicTools.MousePositionToCanvasPosition(canvas,Input.mousePosition);

		if(Vector2.Distance(nowPos,downPos) > 10){

			hasMove = true;

			if(battleContainer.localScale.x > 1){
			
				battleContainer.anchoredPosition = new Vector2(battleContainer.anchoredPosition.x + nowPos.x - lastPos.x,battleContainer.anchoredPosition.y + nowPos.y - lastPos.y);
				
				FixBattleContainerRect();
			}
		}

		lastPos = nowPos;
	}

	private void GetMouseUp(SuperEvent e){

		if (isDown != DownType.NULL) {

			if(!hasMove){

				if(isDown == DownType.BACKGROUND){

					BackgroundClick();

				}else{

					MapUnitUpAsButton(downMapUnit);
				}

			}else{

				hasMove = false;
			}

			isDown = DownType.NULL;
		}
	}
}
