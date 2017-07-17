using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using superTween;
using System;
using FinalWar;
using superRaycast;
using screenScale;
using superFunction;
using publicTools;
using superGraphicRaycast;
using superEnumerator;
using superSequenceControl;

public class BattleManager : MonoBehaviour
{
    public const string BATTLE_OVER = "battleOver";

    private const float mapUnitWidth = 30;
    private const float mapUnitScale = 55;
    private const float heroScale = 0.8f;
    private const float mapContainerYFix = 60;
    private static readonly float sqrt3 = Mathf.Sqrt(3);
    private const float scaleStep = 0.95f;
    private const float minScale = 0.7f;
    private const float maxScale = 1.4f;
    private const int defaultMapHeight = 5;

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
    private Color riverColor;

    [SerializeField]
    private Color hillColor;

    [SerializeField]
    private RectTransform battleContainer;

    [SerializeField]
    private RectTransform battleScaleContainer;

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

    [SerializeField]
    private AlertPanel alertPanel;

    private Battle battle;

    private Dictionary<int, MapUnit> mapUnitDic = new Dictionary<int, MapUnit>();

    private Dictionary<int, HeroCard> cardDic = new Dictionary<int, HeroCard>();

    private Dictionary<int, HeroBattle> heroDic = new Dictionary<int, HeroBattle>();

    private Dictionary<int, HeroBattle> summonHeroDic = new Dictionary<int, HeroBattle>();

    private List<GameObject> arrowList = new List<GameObject>();

    private Vector2 downPos;

    private Vector2 lastPos;

    private GameObject mapGo;

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

    private HeroCard GetNowChooseCard()
    {
        return m_nowChooseCard;
    }

    private void SetNowChooseCard(HeroCard _value)
    {
        if (_value == null)
        {
            heroDetail.Hide(m_nowChooseCard);
        }
        else
        {
            heroDetail.Show(_value);
        }

        m_nowChooseCard = _value;
    }

    private HeroBattle m_nowChooseHero;

    private HeroBattle GetNowChooseHero()
    {
        return m_nowChooseHero;
    }

    private void SetNowChooseHero(HeroBattle _value, bool _canAction)
    {
        if (_value == null)
        {
            heroDetail.Hide(m_nowChooseHero);
        }
        else
        {
            heroDetail.Show(_value);
        }

        m_nowChooseHero = _value;

        nowChooseHeroCanAction = _canAction;
    }

    private bool nowChooseHeroCanAction = false;

    private bool isDoingHeroAction = false;

    private bool mouseHasExited = false;

    private bool canAction = true;

    private bool isInit = false;

    private void WriteLog(string _str)
    {
        Debug.Log(_str);
    }

    public void Init(Action<MemoryStream> _sendDataCallBack)
    {
        Log.Init(WriteLog);

        SuperRaycast.SetCamera(mainCamera);

        SuperRaycast.SetIsOpen(true, "a");

        SuperRaycast.checkBlockByUi = true;

        Dictionary<int, HeroSDS> heroDic = StaticData.GetDic<HeroSDS>();

        Dictionary<int, SkillSDS> skillDic = StaticData.GetDic<SkillSDS>();

        Dictionary<int, AuraSDS> auraDic = StaticData.GetDic<AuraSDS>();

        Battle.Init(Map.mapDataDic, heroDic, skillDic, auraDic);

        battle = new Battle();

        battle.ClientSetCallBack(_sendDataCallBack, RefreshData, DoAction, BattleQuit);

        SuperFunction.Instance.AddEventListener<float, Vector2>(ScreenScale.Instance.go, ScreenScale.SCALE_CHANGE, ScaleChange);

        SuperFunction.Instance.AddEventListener<RaycastHit, int>(backGround, SuperRaycast.GetMouseButtonDown, GetMouseDown);

        SuperFunction.Instance.AddEventListener<RaycastHit, int>(backGround, SuperRaycast.GetMouseButton, GetMouseMove);

        //		SuperFunction.Instance.AddEventListener (backGround, SuperRaycast.GetMouseButtonUp, GetMouseUp);
    }

    public void ReceiveData(byte[] _bytes)
    {
        battle.ClientGetPackage(_bytes);
    }

    private void RefreshData()
    {
        if (!isInit)
        {
            isInit = true;
        }

        heroDetail.Hide();

        ClearMapUnits();

        ClearCards();

        ClearSummonHeros();

        ClearHeros();

        ClearMoves();

        CreateMapPanel();

        CreateCards(true);

        CreateSummonHeros();

        CreateHeros();

        CreateMoves();

        CreateMoneyTf();

        RefreshTouchable(battle.GetClientCanAction());

        if (battle.mWin && battle.oWin)
        {

            Alert("Draw!", BattleOver);

        }
        else if (battle.mWin)
        {

            if (battle.clientIsMine)
            {

                Alert("You win!", BattleOver);

            }
            else {

                Alert("You lose!", BattleOver);
            }

        }
        else if (battle.oWin)
        {

            if (battle.clientIsMine)
            {

                Alert("You lose!", BattleOver);

            }
            else {

                Alert("You win!", BattleOver);
            }
        }
    }

    private void RefreshDataBeforeBattle()
    {

        heroDetail.Hide();

        ClearMapUnits();

        ClearCards();

        ClearSummonHeros();

        ClearHeros();

        ClearMoves();

        CreateMapPanel();

        CreateCards(false);

        //		CreateSummonHeros();

        CreateHeros();

        //		CreateMoves();

        //		CreateMoneyTf();

        CreateMoneyTfOrigin();

        RefreshTouchable(battle.GetClientCanAction());
    }

    public void QuitBattle()
    {
        battle.ClientRequestQuitBattle();
    }

    private void BattleQuit()
    {
        Alert("Battle quit!", BattleOver);
    }

    private void BattleOver()
    {
        RefreshTouchable(true);

        gameObject.SetActive(false);

        SuperFunction.Instance.DispatchEvent(gameObject, BATTLE_OVER);
    }

    private void ClearMapUnits()
    {
        if (mapGo != null)
        {
            Destroy(mapGo);
        }

        Dictionary<int, MapUnit>.ValueCollection.Enumerator enumerator = mapUnitDic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            GameObject.Destroy(enumerator.Current.gameObject);
        }

        mapUnitDic.Clear();
    }

    private void ClearCards()
    {
        Dictionary<int, HeroCard>.ValueCollection.Enumerator enumerator2 = cardDic.Values.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            GameObject.Destroy(enumerator2.Current.gameObject);
        }

        cardDic.Clear();
    }

    private void ClearSummonHeros()
    {
        Dictionary<int, HeroBattle>.ValueCollection.Enumerator enumerator2 = summonHeroDic.Values.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            GameObject.Destroy(enumerator2.Current.gameObject);
        }

        summonHeroDic.Clear();
    }

    private void ClearHeros()
    {
        Dictionary<int, HeroBattle>.ValueCollection.Enumerator enumerator2 = heroDic.Values.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            GameObject.Destroy(enumerator2.Current.gameObject);
        }

        heroDic.Clear();
    }

    private void ClearMoves()
    {
        for (int i = 0; i < arrowList.Count; i++)
        {
            GameObject.Destroy(arrowList[i]);
        }

        arrowList.Clear();
    }

    private void CreateMapPanel()
    {
        mapGo = GameObject.Instantiate(Resources.Load<GameObject>("MapGo"));

        mapGo.transform.SetParent(mapContainer, false);

        MeshRenderer mr = mapGo.GetComponent<MeshRenderer>();

        Color[] colorArr = new Color[battle.mapData.dic.Count];

        Action<int, Color> dele = delegate (int arg1, Color arg2)
        {

            colorArr[arg1] = arg2;

            mr.material.SetColorArray("colors", colorArr);
        };

        mr.material = new Material(Shader.Find("Unlit/MapUnit"));

        GameObject[] gos = new GameObject[battle.mapData.dic.Count];

        int index = 0;

        int index2 = 0;

        for (int i = 0; i < battle.mapData.mapHeight; i++)
        {
            for (int m = 0; m < battle.mapData.mapWidth; m++)
            {
                if (i % 2 == 1 && m == battle.mapData.mapWidth - 1)
                {
                    continue;
                }

                if (!battle.mapData.dic.ContainsKey(index))
                {
                    index++;

                    continue;
                }

                GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("MapUnit"));

                go.transform.SetParent(mapContainer, false);

                go.transform.localPosition = new Vector3(m * mapUnitWidth * sqrt3 * 2 + ((i % 2 == 1) ? mapUnitWidth * Mathf.Sqrt(3) : 0), -i * mapUnitWidth * 3, 0);

                go.transform.localScale = new Vector3(mapUnitScale, mapUnitScale, mapUnitScale);

                MapUnit unit = go.GetComponent<MapUnit>();

                mapUnitDic.Add(index, unit);

                unit.Init(index, index2, dele);

                SetMapUnitColor(unit);

                MapData.MapUnitType mapUnitType = battle.mapData.dic[index];

                if (mapUnitType == MapData.MapUnitType.RIVER || mapUnitType == MapData.MapUnitType.HILL)
                {
                    GameObject.Destroy(unit.GetComponent<Collider>());
                }

                index++;

                gos[index2] = go;

                index2++;
            }
        }

        Mesh mesh = PublicTools.CombineMeshs(gos);

        MeshFilter mf = mapGo.GetComponent<MeshFilter>();

        mf.mesh = mesh;

        battleContentContainer.localPosition = new Vector3(-0.5f * (battle.mapData.mapWidth * mapUnitWidth * sqrt3 * 2) + mapUnitWidth * sqrt3, mapContainerYFix + 0.5f * (battle.mapData.mapHeight * mapUnitWidth * 3 + mapUnitWidth) - mapUnitWidth * 2, 0);

        float baseScale = (float)defaultMapHeight / battle.mapData.mapHeight;

        battleScaleContainer.localScale = new Vector3(baseScale, baseScale, baseScale);

        battleContainer.localScale = Vector3.one;

        battleContainer.localPosition = Vector3.zero;
    }

    private void CreateCards(bool _hideInSummon)
    {
        Dictionary<int, int> tmpCardDic = battle.clientIsMine ? battle.mHandCards : battle.oHandCards;

        int index = 0;

        Dictionary<int, int>.Enumerator enumerator = tmpCardDic.GetEnumerator();

        while (enumerator.MoveNext())
        {

            KeyValuePair<int, int> pair = enumerator.Current;

            if (_hideInSummon && battle.summon.ContainsKey(pair.Key))
            {
                continue;
            }

            GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("HeroCard"));

            HeroCard hero = go.GetComponent<HeroCard>();

            hero.SetFrameVisible(false);

            hero.Init(pair.Key, pair.Value);

            cardDic.Add(pair.Key, hero);

            go.transform.SetParent(cardContainer, false);

            float cardWidth = (go.transform as RectTransform).sizeDelta.x;
            float cardHeight = (go.transform as RectTransform).sizeDelta.y;

            (go.transform as RectTransform).anchoredPosition = new Vector2(-0.5f * cardContainer.rect.width + cardWidth * 0.5f + index * cardWidth, -0.5f * cardContainer.rect.height + cardHeight * 0.5f);

            index++;
        }
    }

    private void CreateSummonHeros()
    {
        Dictionary<int, int>.Enumerator enumerator2 = battle.summon.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            AddCardToMap(enumerator2.Current.Key, enumerator2.Current.Value);
        }
    }

    private void CreateHeros()
    {
        Dictionary<int, Hero>.ValueCollection.Enumerator enumerator = battle.heroMapDic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            AddHeroToMap(enumerator.Current);
        }
    }

    private void CreateMoneyTf()
    {
        if (!moneyTf.gameObject.activeSelf)
        {

            moneyTf.gameObject.SetActive(true);
        }

        moneyTf.text = battle.ClientGetMoney().ToString();
    }

    private void CreateMoneyTfOrigin()
    {
        if (!moneyTf.gameObject.activeSelf)
        {

            moneyTf.gameObject.SetActive(true);
        }

        moneyTf.text = battle.clientIsMine ? battle.mMoney.ToString() : battle.oMoney.ToString();
    }

    private void CreateMoves()
    {
        BattleData battleData = battle.GetBattleData();

        Dictionary<int, BattleCellData>.Enumerator enumerator2 = battleData.actionDic.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            int pos = enumerator2.Current.Key;

            BattleCellData cellData = enumerator2.Current.Value;

            for (int i = 0; i < cellData.attackers.Count; i++)
            {
                GameObject go = CreateArrow(cellData.attackers[i].pos, pos, Color.red, i);

                arrowList.Add(go);
            }

            for (int i = 0; i < cellData.supporters.Count; i++)
            {
                GameObject go = CreateArrow(cellData.supporters[i].pos, pos, Color.green, i);

                arrowList.Add(go);
            }

            for (int i = 0; i < cellData.shooters.Count; i++)
            {
                GameObject go = CreateShootArrow(cellData.shooters[i].pos, pos, Color.yellow);

                arrowList.Add(go);
            }
        }
    }

    private GameObject CreateArrow(int _start, int _end, Color _color, int _index)
    {
        GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Arrow"));

        Arrow arrow = go.GetComponent<Arrow>();

        go.transform.SetParent(arrowContainer, false);

        MapUnit start = mapUnitDic[_start];

        MapUnit end = mapUnitDic[_end];

        go.transform.localPosition = (start.transform.localPosition + end.transform.localPosition) * 0.5f;

        float angle = Mathf.Atan2(end.transform.localPosition.y - start.transform.localPosition.y, end.transform.localPosition.x - start.transform.localPosition.x);

        Quaternion q = new Quaternion();

        q.eulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI);

        go.transform.localRotation = q;

        arrow.SetColor(_color);

        arrow.SetIndex(_index);

        return go;
    }

    private GameObject CreateShootArrow(int _start, int _end, Color _color)
    {
        GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("ShootArrow"));

        ShootArrow arrow = go.GetComponent<ShootArrow>();

        go.transform.SetParent(arrowContainer, false);

        MapUnit start = mapUnitDic[_start];

        MapUnit end = mapUnitDic[_end];

        go.transform.localPosition = (start.transform.localPosition + end.transform.localPosition) * 0.5f;

        float angle = Mathf.Atan2(end.transform.localPosition.y - start.transform.localPosition.y, end.transform.localPosition.x - start.transform.localPosition.x);

        Quaternion q = new Quaternion();

        q.eulerAngles = new Vector3(0, 0, angle * 180 / Mathf.PI);

        go.transform.localRotation = q;

        float scale = Vector3.Distance(start.transform.localPosition, end.transform.localPosition) / (mapUnitWidth * sqrt3 * 4);

        go.transform.localScale = new Vector3(scale, scale, scale);

        arrow.SetColor(_color);

        return go;
    }

    public void MapUnitDown(MapUnit _mapUnit)
    {
        MapUnitDownReal(_mapUnit);

        if (nowChooseHeroCanAction)
        {
            for (int i = 0; i < battle.action.Count; i++)
            {
                KeyValuePair<int, int> pair = battle.action[i];

                if (pair.Key == GetNowChooseHero().pos)
                {
                    if (pair.Value == _mapUnit.index)
                    {
                        isDoingHeroAction = true;
                    }

                    return;
                }
            }

            if (_mapUnit.index == GetNowChooseHero().pos)
            {
                isDoingHeroAction = true;
            }
        }
    }

    public void MapUnitEnter(MapUnit _mapUnit)
    {
        if (isDoingHeroAction)
        {
            if (GetNowChooseHero().pos != _mapUnit.index)
            {

                if (battle.ClientRequestAction(GetNowChooseHero().pos, _mapUnit.index))
                {
                    ClearMoves();

                    CreateMoves();
                }
            }
        }
    }

    public void MapUnitExit(MapUnit _mapUnit)
    {
        if (isDoingHeroAction)
        {
            mouseHasExited = true;

            for (int i = 0; i < battle.action.Count; i++)
            {
                KeyValuePair<int, int> pair = battle.action[i];

                if (pair.Key == GetNowChooseHero().pos)
                {
                    battle.ClientRequestUnaction(GetNowChooseHero().pos);

                    ClearMoves();

                    CreateMoves();

                    return;
                }
            }
        }
    }

    //	public void MapUnitUp(MapUnit _mapUnit){
    //
    //		if (movingHeroPos != -1) {
    //
    //			movingHeroPos = -1;
    //		}
    //	}

    public void MapUnitUpAsButton(MapUnit _mapUnit)
    {
        if (mouseHasExited)
        {
            return;
        }

        if (battle.summon.ContainsValue(_mapUnit.index))
        {
            HeroBattle summonHero = summonHeroDic[_mapUnit.index];

            if (GetNowChooseHero() == null)
            {
                SetNowChooseHero(summonHero, false);

                GetNowChooseHero().SetFrameVisible(true);
            }
            else
            {
                if (GetNowChooseHero() == summonHero)
                {
                    ClearNowChooseHero();

                    UnsummonHero(summonHero.cardUid);
                }
                else
                {
                    ClearNowChooseHero();

                    SetNowChooseHero(summonHero, false);

                    GetNowChooseHero().SetFrameVisible(true);
                }
            }
        }
        else if (battle.heroMapDic.ContainsKey(_mapUnit.index))
        {
            HeroBattle nowHero = heroDic[_mapUnit.index];

            if (GetNowChooseHero() == null)
            {
                SetNowChooseHero(nowHero, nowHero.isMine == battle.clientIsMine && nowHero.canAction);

                GetNowChooseHero().SetFrameVisible(true);
            }
            else
            {
                if (GetNowChooseHero() != nowHero)
                {
                    ClearNowChooseHero();

                    SetNowChooseHero(nowHero, nowHero.isMine == battle.clientIsMine && nowHero.canAction);

                    GetNowChooseHero().SetFrameVisible(true);
                }
            }
        }
        else if (GetNowChooseCard() != null)
        {
            bool b = SummonHero(GetNowChooseCard().cardUid, _mapUnit.index);

            if (b)
            {

                CreateMoneyTf();

                ClearCards();

                CreateCards(true);

                ClearSummonHeros();

                CreateSummonHeros();

            }
            else {

                ClearNowChooseHero();
            }
        }
        else
        {
            ClearNowChooseHero();
        }

        ClearNowChooseCard();
    }

    public void BackgroundClick()
    {
        ClearNowChooseHero();

        ClearNowChooseCard();
    }

    public void HeroClick(HeroCard _hero)
    {
        ClearNowChooseHero();

        if (GetNowChooseCard() != _hero)
        {
            ClearNowChooseCard();

            SetNowChooseCard(_hero);

            GetNowChooseCard().SetFrameVisible(true);
        }
    }

    private void ClearNowChooseCard()
    {
        if (GetNowChooseCard() != null)
        {
            GetNowChooseCard().SetFrameVisible(false);

            SetNowChooseCard(null);
        }
    }

    private void ClearNowChooseHero()
    {
        if (GetNowChooseHero() != null)
        {
            GetNowChooseHero().SetFrameVisible(false);

            SetNowChooseHero(null, false);
        }
    }

    private bool SummonHero(int _cardUid, int _pos)
    {
        bool b = battle.ClientRequestSummon(_cardUid, _pos);

        if (b)
        {

            CreateMoneyTf();

            ClearCards();

            CreateCards(true);

            ClearSummonHeros();

            CreateSummonHeros();
        }

        return b;
    }

    private void UnsummonHero(int _cardUid)
    {
        battle.ClientRequestUnsummon(_cardUid);

        CreateMoneyTf();

        ClearCards();

        CreateCards(true);

        ClearSummonHeros();

        CreateSummonHeros();
    }

    private HeroBattle AddHeroToMap(Hero _hero)
    {
        GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("HeroBattle"));

        HeroBattle hero = go.GetComponent<HeroBattle>();

        heroDic.Add(_hero.pos, hero);

        hero.Init(_hero);

        AddHeroToMapReal(hero, _hero.pos);

        return hero;
    }

    private HeroBattle AddCardToMap(int _cardUid, int _pos)
    {
        Dictionary<int, int> list = battle.clientIsMine ? battle.mHandCards : battle.oHandCards;

        int cardID = list[_cardUid];

        GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("HeroBattle"));

        HeroBattle hero = go.GetComponent<HeroBattle>();

        summonHeroDic.Add(_pos, hero);

        hero.Init(cardID);

        hero.cardUid = _cardUid;

        AddHeroToMapReal(hero, _pos);

        return hero;
    }

    private void AddHeroToMapReal(HeroBattle _heroCard, int _pos)
    {
        MapUnit mapUnit = mapUnitDic[_pos];

        _heroCard.SetFrameVisible(false);

        _heroCard.transform.SetParent(heroContainer, false);

        _heroCard.transform.localPosition = mapUnit.transform.localPosition;

        _heroCard.transform.localScale = new Vector3(heroScale, heroScale, heroScale);
    }


    public void ActionBtClick()
    {
        ClearNowChooseCard();

        ClearNowChooseHero();

        battle.ClientRequestDoAction();

        RefreshTouchable(battle.GetClientCanAction());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            GetMouseUp();
        }

        if (Input.GetKeyUp(KeyCode.F5))
        {
            battle.ClientRequestRefreshData();

        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            CreateAiAction();
        }

        //		if (actionBt.activeSelf) {
        //
        //			CreateAiAction();
        //
        //			ActionBtClick();
        //		}
    }

    private void CreateAiAction()
    {

        //		HeroAi.Start(battle, battle.clientIsMine, 0.2);

        ClearMoves();

        CreateMoves();

        ClearSummonHeros();

        CreateSummonHeros();

        ClearCards();

        CreateCards(true);
    }

    private void RefreshTouchable(bool _canAction)
    {
        if (canAction && !_canAction)
        {
            SuperGraphicRaycast.SetIsOpen(false, "a");

            SuperRaycast.SetIsOpen(false, "a");

            actionBt.SetActive(false);

            canAction = _canAction;

        }
        else if (!canAction && _canAction)
        {
            SuperGraphicRaycast.SetIsOpen(true, "a");

            SuperRaycast.SetIsOpen(true, "a");

            actionBt.SetActive(true);

            canAction = _canAction;
        }
    }

    private void DoAction(SuperEnumerator<ValueType> _step)
    {
        RefreshDataBeforeBattle();

        DoActionReal(_step);
    }

    private void DoActionReal(SuperEnumerator<ValueType> _step)
    {
        if (_step.MoveNext())
        {
            ValueType vo = _step.Current;

            Action del = delegate ()
            {
                DoActionReal(_step);
            };

            if (vo is BattleShootVO)
            {
                DoShoot((BattleShootVO)(vo), del);
            }
            else if (vo is BattleMoveVO)
            {
                DoMove((BattleMoveVO)vo, del);
            }
            else if (vo is BattleRushVO)
            {
                DoRush((BattleRushVO)vo, del);
            }
            else if (vo is BattleAttackVO)
            {
                DoAttack((BattleAttackVO)vo, del);
            }
            else if (vo is BattlePrepareAttackVO)
            {
                DoPrepareAttack((BattlePrepareAttackVO)vo, del);
            }
            else if (vo is BattleAttackAndCounterVO)
            {
                DoAttackAndCounter((BattleAttackAndCounterVO)vo, del);
            }
            else if (vo is BattleCounterVO)
            {
                DoCounter((BattleCounterVO)vo, del);
            }
            else if (vo is BattleDeathVO)
            {
                DoDie((BattleDeathVO)vo, del);
            }
            else if (vo is BattleSummonVO)
            {
                DoSummon((BattleSummonVO)vo, del);
            }
            else if (vo is BattleAddCardsVO)
            {
                DoAddCards((BattleAddCardsVO)vo, del);
            }
            else if (vo is BattleDelCardsVO)
            {
                DoDelCards((BattleDelCardsVO)vo, del);
            }
            else if (vo is BattleMoneyChangeVO)
            {
                DoMoneyChange((BattleMoneyChangeVO)vo, del);
            }
            else if (vo is BattleLevelUpVO)
            {
                DoLevelUp((BattleLevelUpVO)vo, del);
            }
            else {

                throw new Exception("vo type error:" + vo);
            }
        }
        else {

            battle.ClientEndBattle();

            RefreshData();
        }
    }

    private void DoSummon(BattleSummonVO _vo, Action _del)
    {
        CreateMoneyTfOrigin();

        Hero hero = battle.heroMapDic[_vo.pos];

        HeroBattle heroBattle = AddHeroToMap(hero);

        heroBattle.transform.localScale = Vector3.zero;

        Action<float> toDel = delegate (float obj)
        {
            float scale = heroScale * obj;

            heroBattle.transform.localScale = new Vector3(scale, scale, scale);
        };

        Action endDel = delegate ()
        {
            SuperTween.Instance.DelayCall(0.5f, _del);
        };

        SuperTween.Instance.To(10, 1, 0.5f, toDel, endDel);

        ClearCards();

        CreateCards(false);
    }

    private void DoShoot(BattleShootVO _vo, Action _del)
    {
        SuperSequenceControl.Start(BattleControl.Instance.Shoot, heroDic[_vo.shooter], heroDic[_vo.stander], _vo.damage, _del);
    }

    private void DoPrepareAttack(BattlePrepareAttackVO _vo, Action _del)
    {
        HeroBattle attacker = heroDic[_vo.attacker];

        HeroBattle defender = null;

        HeroBattle supporter = null;

        if (_vo.pos == _vo.defender)
        {
            defender = heroDic[_vo.defender];
        }
        else
        {
            supporter = heroDic[_vo.defender];

            heroDic.TryGetValue(_vo.pos, out defender);
        }

        List<HeroBattle> attackerSupporters = null;

        if (_vo.attackerSupperters != null)
        {
            attackerSupporters = new List<HeroBattle>();

            for (int i = 0; i < _vo.attackerSupperters.Count; i++)
            {
                attackerSupporters.Add(heroDic[_vo.attackerSupperters[i]]);
            }
        }

        List<HeroBattle> defenderSupporters = null;

        if (_vo.defenderSupporters != null)
        {
            defenderSupporters = new List<HeroBattle>();

            for (int i = 0; i < _vo.defenderSupporters.Count; i++)
            {
                defenderSupporters.Add(heroDic[_vo.defenderSupporters[i]]);
            }
        }

        SuperSequenceControl.Start(BattleControl.Instance.PrepareAttack, mapUnitDic[_vo.pos].transform.localPosition, attacker, defender, supporter, attackerSupporters, defenderSupporters, _vo.attackerSpeed, _vo.defenderSpeed, _del);
    }

    private void DoMove(BattleMoveVO _vo, Action _del)
    {
        if (_vo.moves.Count > 0)
        {
            List<KeyValuePair<int, int>> tmpList = new List<KeyValuePair<int, int>>();

            List<KeyValuePair<int, int>> tmpList2 = new List<KeyValuePair<int, int>>();

            Dictionary<int, int>.Enumerator enumerator = _vo.moves.GetEnumerator();

            enumerator.MoveNext();

            tmpList.Add(enumerator.Current);

            while (tmpList.Count > 0)
            {
                KeyValuePair<int, int> pair = tmpList[0];

                _vo.moves.Remove(pair.Key);

                tmpList.RemoveAt(0);

                tmpList2.Add(pair);

                if (_vo.moves.ContainsKey(pair.Value))
                {
                    tmpList.Add(new KeyValuePair<int, int>(pair.Value, _vo.moves[pair.Value]));
                }

                if (_vo.moves.ContainsValue(pair.Key))
                {
                    enumerator = _vo.moves.GetEnumerator();

                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Value == pair.Key)
                        {
                            tmpList.Add(enumerator.Current);

                            break;
                        }
                    }
                }
            }

            Dictionary<int, HeroBattle> tmpDic = new Dictionary<int, HeroBattle>();

            for (int i = 0; i < tmpList2.Count; i++)
            {
                KeyValuePair<int, int> pair = tmpList2[i];

                HeroBattle hero = heroDic[pair.Key];

                tmpDic.Add(pair.Key, hero);

                heroDic.Remove(pair.Key);

                Vector3 startPos = mapUnitDic[pair.Key].transform.localPosition;

                Vector3 endPos = mapUnitDic[pair.Value].transform.localPosition;

                Action<float> toDel = delegate (float obj)
                {
                    hero.transform.localPosition = Vector3.Lerp(startPos, endPos, obj);
                };

                if (i == 0)
                {
                    Action del = delegate ()
                    {
                        for (int l = 0; l < tmpList2.Count; l++)
                        {
                            pair = tmpList2[l];

                            heroDic.Add(pair.Value, tmpDic[pair.Key]);

                            int index = pair.Value;

                            MapUnit unit = mapUnitDic[index];

                            SetMapUnitColor(unit);
                        }

                        DoMove(_vo, _del);
                    };

                    SuperTween.Instance.To(0, 1, 1, toDel, del);
                }
                else
                {
                    SuperTween.Instance.To(0, 1, 1, toDel, null);
                }
            }
        }
        else
        {
            _del();
        }
    }

    private void SetMapUnitColor(MapUnit _unit)
    {
        int index = _unit.index;

        MapData.MapUnitType mapUnitType = battle.mapData.dic[index];

        if (mapUnitType == MapData.MapUnitType.RIVER)
        {
            _unit.SetMainColor(riverColor);
        }
        else if (mapUnitType == MapData.MapUnitType.HILL)
        {
            _unit.SetMainColor(hillColor);
        }
        else if (battle.GetPosIsMine(index) == battle.clientIsMine)
        {
            if ((!battle.clientIsMine && index == battle.mapData.oBase) || (battle.clientIsMine && index == battle.mapData.mBase))
            {
                _unit.SetMainColor(myBaseColor);
            }
            else
            {
                _unit.SetMainColor(myMapUnitColor);
            }
        }
        else
        {
            if ((!battle.clientIsMine && index == battle.mapData.mBase) || (battle.clientIsMine && index == battle.mapData.oBase))
            {
                _unit.SetMainColor(oppBaseColor);
            }
            else
            {
                _unit.SetMainColor(oppMapUnitColor);
            }
        }
    }

    private void DoRush(BattleRushVO _vo, Action _del)
    {
        SuperSequenceControl.Start(BattleControl.Instance.Rush, heroDic[_vo.attacker], heroDic[_vo.stander], _vo.damage, _del);
    }

    private void DoAttack(BattleAttackVO _vo, Action _del)
    {
        SuperSequenceControl.Start(BattleControl.Instance.Attack, mapUnitDic[_vo.pos].transform.localPosition, heroDic[_vo.attacker], heroDic[_vo.defender], _vo.damage, _del);
    }

    private void DoCounter(BattleCounterVO _vo, Action _del)
    {
        SuperSequenceControl.Start(BattleControl.Instance.Counter, mapUnitDic[_vo.pos].transform.localPosition, heroDic[_vo.attacker], heroDic[_vo.defender], _vo.damage, _del);
    }

    private void DoAttackAndCounter(BattleAttackAndCounterVO _vo, Action _del)
    {
        SuperSequenceControl.Start(BattleControl.Instance.AttackAndCounter, mapUnitDic[_vo.pos].transform.localPosition, heroDic[_vo.attacker], heroDic[_vo.defender], _vo.attackDamage, _vo.defenseDamage, _del);
    }

    private void DoDie(BattleDeathVO _vo, Action _del)
    {
        bool getDie = false;

        for (int i = 0; i < _vo.deads.Count; i++)
        {
            int pos = _vo.deads[i];

            HeroBattle hero = heroDic[pos];

            heroDic.Remove(pos);

            if (!getDie)
            {
                getDie = true;

                hero.Die(_del);
            }
            else
            {
                hero.Die(null);
            }
        }
    }

    private void DoAddCards(BattleAddCardsVO _vo, Action _del)
    {
        if (_vo.isMine == battle.clientIsMine)
        {
            ClearCards();

            CreateCards(true);
        }

        _del();
    }

    private void DoDelCards(BattleDelCardsVO _vo, Action _del)
    {
        if (_vo.isMine == battle.clientIsMine)
        {
            ClearCards();

            CreateCards(true);
        }

        _del();
    }

    private void DoMoneyChange(BattleMoneyChangeVO _vo, Action _del)
    {
        if (_vo.isMine == battle.clientIsMine)
        {
            CreateMoneyTf();
        }

        _del();
    }

    private void DoLevelUp(BattleLevelUpVO _vo, Action _del)
    {
        HeroBattle hero = heroDic[_vo.pos];

        hero.RefreshAll();

        _del();
    }

    private void FixBattleContainerRect()
    {
        if (battleContainer.localScale.x < 1)
        {
            battleContainer.anchoredPosition = Vector2.zero;
        }
        else
        {
            if (battleContainer.anchoredPosition.x - (canvas.transform as RectTransform).rect.width / 2 * battleContainer.localScale.x > -(canvas.transform as RectTransform).rect.width / 2)
            {
                battleContainer.anchoredPosition = new Vector2(-(canvas.transform as RectTransform).rect.width / 2 + (canvas.transform as RectTransform).rect.width / 2 * battleContainer.localScale.x, battleContainer.anchoredPosition.y);
            }
            else if (battleContainer.anchoredPosition.x + (canvas.transform as RectTransform).rect.width / 2 * battleContainer.localScale.x < (canvas.transform as RectTransform).rect.width / 2)
            {
                battleContainer.anchoredPosition = new Vector2((canvas.transform as RectTransform).rect.width / 2 - (canvas.transform as RectTransform).rect.width / 2 * battleContainer.localScale.x, battleContainer.anchoredPosition.y);
            }

            if (battleContainer.anchoredPosition.y - (canvas.transform as RectTransform).rect.height / 2 * battleContainer.localScale.x > -(canvas.transform as RectTransform).rect.height / 2)
            {
                battleContainer.anchoredPosition = new Vector2(battleContainer.anchoredPosition.x, -(canvas.transform as RectTransform).rect.height / 2 + (canvas.transform as RectTransform).rect.height / 2 * battleContainer.localScale.x);
            }
            else if (battleContainer.anchoredPosition.y + (canvas.transform as RectTransform).rect.height / 2 * battleContainer.localScale.x < (canvas.transform as RectTransform).rect.height / 2)
            {
                battleContainer.anchoredPosition = new Vector2(battleContainer.anchoredPosition.x, (canvas.transform as RectTransform).rect.height / 2 - (canvas.transform as RectTransform).rect.height / 2 * battleContainer.localScale.x);
            }
        }
    }

    private void ScaleChange(int _index, float _scrollValue, Vector2 _pos)
    {
        float scrollValue = _scrollValue;

        Vector2 mousePosition = _pos;

        if (scrollValue < 1)
        {
            Vector2 v = PublicTools.MousePositionToCanvasPosition(canvas, mousePosition);

            Vector2 v2 = (v - battleContainer.anchoredPosition) / battleContainer.localScale.x;

            battleContainer.localScale = battleContainer.localScale * scaleStep;

            if (battleContainer.localScale.x < minScale)
            {
                battleContainer.localScale = new Vector3(minScale, minScale, minScale);
            }

            battleContainer.anchoredPosition = v - v2 * battleContainer.localScale.x;

            FixBattleContainerRect();
        }
        else if (scrollValue > 1)
        {
            Vector2 v = PublicTools.MousePositionToCanvasPosition(canvas, Input.mousePosition);

            Vector2 v2 = (v - battleContainer.anchoredPosition) / battleContainer.localScale.x;

            battleContainer.localScale = battleContainer.localScale / scaleStep;

            if (battleContainer.localScale.x > maxScale)
            {
                battleContainer.localScale = new Vector3(maxScale, maxScale, maxScale);
            }

            battleContainer.anchoredPosition = v - v2 * battleContainer.localScale.x;
        }
    }

    private void GetMouseDown(int _index, RaycastHit _hit, int _hitIndex)
    {
        if (_hitIndex == 0)
        {
            BackgroundDown();
        }
    }

    private void BackgroundDown()
    {
        downPos = lastPos = PublicTools.MousePositionToCanvasPosition(canvas, Input.mousePosition);

        isDown = DownType.BACKGROUND;
    }

    private void MapUnitDownReal(MapUnit _mapUnit)
    {
        downPos = lastPos = PublicTools.MousePositionToCanvasPosition(canvas, Input.mousePosition);

        isDown = DownType.MAPUNIT;

        downMapUnit = _mapUnit;
    }

    private void GetMouseMove(int _index, RaycastHit _hit, int _hitIndex)
    {
        if (isDown != DownType.NULL)
        {
            BackgroundMove();
        }
    }

    private void BackgroundMove()
    {
        Vector3 nowPos = PublicTools.MousePositionToCanvasPosition(canvas, Input.mousePosition);

        if (!hasMove && Vector2.Distance(nowPos, downPos) > 10)
        {
            hasMove = true;

            SuperRaycast.checkBlockByUi = false;
        }

        if (!isDoingHeroAction && hasMove && battleContainer.localScale.x > 1)
        {
            battleContainer.anchoredPosition = new Vector2(battleContainer.anchoredPosition.x + nowPos.x - lastPos.x, battleContainer.anchoredPosition.y + nowPos.y - lastPos.y);

            FixBattleContainerRect();
        }

        lastPos = nowPos;
    }

    private void GetMouseUp()
    {
        if (isDown != DownType.NULL)
        {
            if (!hasMove)
            {
                if (isDown == DownType.BACKGROUND)
                {
                    BackgroundClick();
                }
                else
                {
                    MapUnitUpAsButton(downMapUnit);
                }
            }
            else
            {
                SuperRaycast.checkBlockByUi = true;

                hasMove = false;
            }

            isDown = DownType.NULL;
        }

        if (mouseHasExited)
        {
            mouseHasExited = false;
        }

        if (isDoingHeroAction)
        {
            isDoingHeroAction = false;
        }
    }

    private void Alert(string _str, Action _callBack)
    {
        alertPanel.Alert(_str, _callBack);
    }

    void Start()
    {

        gameObject.SetActive(false);
    }
}
