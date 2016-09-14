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

	private Battle battle;

	private Dictionary<int, MapUnit> mapUnitDic = new Dictionary<int, MapUnit> ();

	private Dictionary<int, HeroCard> cardDic = new Dictionary<int, HeroCard>();

	private Dictionary<int, HeroBattle> heroDic = new Dictionary<int, HeroBattle>();

	private Dictionary<int, HeroBattle> summonHeroDic = new Dictionary<int, HeroBattle>();

	private List<GameObject> arrowList = new List<GameObject> ();

	private HeroCard m_nowChooseCard;

	private HeroCard nowChooseCard{

		get{

			return m_nowChooseCard;
		}

		set{

			if(value == null){

				heroDetail.Hide(m_nowChooseCard);

			}else{

				heroDetail.Show(value);
			}

			m_nowChooseCard = value;
		}
	}

	private HeroBattle m_nowChooseHero;

	private HeroBattle nowChooseHero{

		get{

			return m_nowChooseHero;
		}

		set{

			if(value == null){
				
				heroDetail.Hide(m_nowChooseHero);
				
			}else{
				
				heroDetail.Show(value);
			}

			m_nowChooseHero = value;
		}
	}

	private int movingHeroPos = -1;

	private bool mouseHasExited = false;

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
		
		Connection.Instance.Init ("127.0.0.1", 1983, ReceiveData, ConfigDictionary.Instance.uid);
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

		if(nowChooseHero != null && nowChooseHero.isMine == battle.clientIsMine){

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

			if(mouseHasExited){

				mouseHasExited = false;
			}
		}
	}

	public void MapUnitUpAsButton(MapUnit _mapUnit){

		if (mouseHasExited) {

			return;
		}

		if (battle.summon.ContainsValue (_mapUnit.index)) {

			HeroBattle summonHero = summonHeroDic [_mapUnit.index];

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

			HeroBattle nowHero = heroDic [_mapUnit.index];
			
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

	public void BackgroundClick(){

		ClearNowChooseHero ();

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

		hero.body.color = summonColor;

		HeroSDS sds = StaticData.GetData<HeroSDS> (cardID);
		
		hero.Init(cardID, _pos, battle.clientIsMine);

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
			}
		}
	}

	private void DoSummon(BattleSummonVO _vo,Action _del){

		CreateMoneyTf ();

		Hero hero = battle.heroMapDic [_vo.pos];

		HeroBattle heroBattle = AddHeroToMap (hero);

		Action<float> toDel = delegate(float obj) {

			float scale = heroScale * obj;

			heroBattle.transform.localScale = new Vector3(scale,scale,scale);
		};

		Action endDel = delegate() {

			SuperTween.Instance.DelayCall(0.5f,_del);
		};

		SuperTween.Instance.To (10, 1, 0.5f, toDel, endDel);
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

//		_del ();
	}
}
