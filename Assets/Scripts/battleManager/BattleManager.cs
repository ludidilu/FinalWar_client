using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using FinalWar;
using superRaycast;
using screenScale;
using superFunction;
using superGraphicRaycast;
using superEnumerator;
using superSequenceControl;
using System.Collections;
using gameObjectFactory;

public class BattleManager : MonoBehaviour
{
    public const string BATTLE_QUIT = "battleQuit";

    public const string BATTLE_SEND_DATA = "battleSendData";

    public const string BATTLE_RECEIVE_DATA = "battleReceiveData";

    [SerializeField]
    private BattleControl battleControl;

    [SerializeField]
    private float moveThreshold = 0.1f;

    [SerializeField]
    private float mapUnitWidth = 30;

    [SerializeField]
    private float mapUnitScale = 55;

    private static readonly float sqrt3 = Mathf.Sqrt(3);

    [SerializeField]
    private float minScale = 0.7f;

    [SerializeField]
    private float maxScale = 1.4f;

    [SerializeField]
    private float fixStep = 1.05f;

    [SerializeField]
    private float viewportXFix;

    [SerializeField]
    private float viewportYFix;

    [SerializeField]
    private float boundFix;

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
    private Transform battleContainer;

    [SerializeField]
    private Transform battleContentContainer;

    [SerializeField]
    private RectTransform cardContainer;

    [SerializeField]
    private Transform mapContainer;

    [SerializeField]
    private Transform heroContainer;

    [SerializeField]
    public Transform arrowContainer;

    [SerializeField]
    private Text mMoneyTf;

    [SerializeField]
    private Text mScoreTf;

    [SerializeField]
    private Text oMoneyTf;

    [SerializeField]
    private Text oScoreTf;

    [SerializeField]
    private Text oCardNumTf;

    [SerializeField]
    private Text roundNumLeftTf;

    [SerializeField]
    private GameObject actionBt;

    [SerializeField]
    private HeroDetail heroDetail;

    [SerializeField]
    private GameObject backGround;

    [SerializeField]
    private DescPanel descPanel;

    [SerializeField]
    private AlertPanel alertPanel;

    [SerializeField]
    private SpriteRenderer bg;

    private GameObject eventGo;

    private Battle_client battle = new Battle_client();

    public Dictionary<int, MapUnit> mapUnitDic = new Dictionary<int, MapUnit>();

    public Dictionary<int, HeroBattle> heroDic = new Dictionary<int, HeroBattle>();

    private Dictionary<int, HeroCard> cardDic = new Dictionary<int, HeroCard>();

    private Dictionary<int, HeroBattle> summonHeroDic = new Dictionary<int, HeroBattle>();

    private List<GameObject> arrowList = new List<GameObject>();

    private Vector2 downPos;

    private Vector2 lastPos;

    private Bounds bounds;

    private Bounds viewport;

    private float defaultScale;

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

    private Vector2 stepV;

    private int heroUid;

    public int GetHeroUid()
    {
        heroUid++;

        return heroUid;
    }

    void Awake()
    {
        stepV = new Vector2(mainCamera.aspect * mainCamera.orthographicSize, mainCamera.orthographicSize);

        viewport = new Bounds(Vector3.zero, stepV * 2);

        viewport.center = new Vector3(viewportXFix, viewportYFix, 0);

        viewport.extents = new Vector3(viewport.extents.x - viewportXFix, viewport.extents.y - viewportYFix, viewport.extents.z);

        SuperFunction.Instance.AddEventListener<int>(ClickText.eventGo, ClickText.EVENT_NAME, ClickDesc);

        SuperFunction.Instance.AddEventListener<int>(ClickImage.eventGo, ClickImage.EVENT_NAME, ClickDesc);

        gameObject.SetActive(false);
    }

    private void WriteLog(string _str)
    {
        Debug.Log(_str);
    }

    public void Init(GameObject _eventGo)
    {
        eventGo = _eventGo;

        Log.Init(WriteLog);

        SuperRaycast.SetCamera(mainCamera);

        SuperRaycast.SetIsOpen(true, "a");

        Dictionary<int, MapSDS> mapDic = StaticData.GetDic<MapSDS>();

        Dictionary<int, HeroSDS> heroDic = StaticData.GetDic<HeroSDS>();

        Dictionary<int, AuraSDS> auraDic = StaticData.GetDic<AuraSDS>();

        Dictionary<int, EffectSDS> effectDic = StaticData.GetDic<EffectSDS>();

        Battle.Init(mapDic, heroDic, auraDic, effectDic);

        battle.ClientSetCallBack(SendData, RefreshData, DoAction, BattleOver);

        SuperFunction.Instance.AddEventListener<float, Vector2>(ScreenScale.Instance.go, ScreenScale.SCALE_CHANGE, ScaleChange);

        SuperFunction.Instance.AddEventListener<bool, RaycastHit, int>(backGround, SuperRaycast.GetMouseButtonDown, GetMouseDown);

        SuperFunction.Instance.AddEventListener<bool, RaycastHit, int>(backGround, SuperRaycast.GetMouseButton, GetMouseMove);

        SuperFunction.Instance.AddEventListener<BinaryReader>(eventGo, BATTLE_RECEIVE_DATA, ReceiveData);
    }

    private void SendData(MemoryStream _ms, Action<BinaryReader> _callBack)
    {
        SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_SEND_DATA, _ms, _callBack);
    }

    private void ReceiveData(int _index, BinaryReader _br)
    {
        battle.ClientGetPackage(_br);
    }

    private void RefreshData()
    {
        if (!isInit)
        {
            isInit = true;

            gameObject.SetActive(true);
        }

        heroDetail.Hide();

        descPanel.Close();

        ClearMapUnits();

        ClearCards();

        ClearSummonHeros();

        ClearHeros();

        ClearMoves();

        CreateMapUnits();

        CreateCards();

        CreateSummonHeros();

        CreateHeros();

        CreateMoves();

        CreateMoneyTf();

        CreateScoreTf();

        CreateRoundNumLeftTf();

        RefreshTouchable(battle.GetClientCanAction());
    }

    private void RefreshDataBeforeBattle()
    {
        heroDetail.Hide();

        descPanel.Close();

        ClearMapUnits();

        ClearCards();

        ClearSummonHeros();

        ClearHeros();

        ClearMoves();

        CreateMapUnits();

        CreateCards();

        CreateHeros();

        CreateMoneyTf();
    }

    public void QuitBattle()
    {
        battle.ClientRequestQuitBattle();
    }

    private void BattleOver(Battle.BattleResult _result)
    {
        switch (_result)
        {
            case Battle.BattleResult.DRAW:

                Alert("Draw!", BattleQuit);

                break;

            case Battle.BattleResult.M_WIN:

                if (battle.clientIsMine)
                {
                    Alert("You win!", BattleQuit);
                }
                else
                {
                    Alert("You lose!", BattleQuit);
                }

                break;

            default:

                if (battle.clientIsMine)
                {
                    Alert("You lose!", BattleQuit);
                }
                else
                {
                    Alert("You win!", BattleQuit);
                }

                break;
        }
    }

    private void BattleQuit()
    {
        ClearMapUnits();

        ClearCards();

        ClearSummonHeros();

        ClearHeros();

        ClearMoves();

        isInit = false;

        RefreshTouchable(true);

        gameObject.SetActive(false);

        SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_QUIT);
    }

    private void ClearMapUnits()
    {
        Dictionary<int, MapUnit>.ValueCollection.Enumerator enumerator = mapUnitDic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            Destroy(enumerator.Current.gameObject);
        }

        mapUnitDic.Clear();

        battleContentContainer.localPosition = Vector3.zero;

        battleContentContainer.localScale = Vector3.one;

        battleContainer.localPosition = Vector3.zero;

        battleContainer.localScale = Vector3.one;
    }

    private void ClearCards()
    {
        Dictionary<int, HeroCard>.ValueCollection.Enumerator enumerator2 = cardDic.Values.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            Destroy(enumerator2.Current.gameObject);
        }

        cardDic.Clear();
    }

    private void ClearSummonHeros()
    {
        Dictionary<int, HeroBattle>.ValueCollection.Enumerator enumerator2 = summonHeroDic.Values.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            Destroy(enumerator2.Current.gameObject);
        }

        summonHeroDic.Clear();
    }

    private void ClearHeros()
    {
        heroUid = 0;

        Dictionary<int, HeroBattle>.ValueCollection.Enumerator enumerator2 = heroDic.Values.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            Destroy(enumerator2.Current.gameObject);
        }

        heroDic.Clear();
    }

    private void ClearMoves()
    {
        for (int i = 0; i < arrowList.Count; i++)
        {
            Destroy(arrowList[i]);
        }

        arrowList.Clear();
    }

    private void CreateMapUnits()
    {
        int fix = battle.clientIsMine ? 1 : -1;

        int index = 0;

        bool initBounds = false;

        for (int i = 0; i < battle.mapData.mapWidth; i++)
        {
            for (int m = 0; m < battle.mapData.mapHeight; m++)
            {
                if (i % 2 == 1 && m == battle.mapData.mapHeight - 1)
                {
                    continue;
                }

                if (!battle.mapData.dic.ContainsKey(index))
                {
                    index++;

                    continue;
                }

                GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/MapUnit.prefab", null);

                go.transform.SetParent(mapContainer, false);

                go.transform.localPosition = new Vector3(i * mapUnitWidth * 3 * fix, (-m * mapUnitWidth * sqrt3 * 2 - ((i % 2 == 1) ? mapUnitWidth * sqrt3 : 0)) * fix, 0);

                go.transform.localScale = new Vector3(mapUnitScale, mapUnitScale, mapUnitScale);

                MapUnit unit = go.GetComponent<MapUnit>();

                SuperFunction.SuperFunctionCallBack3<bool, RaycastHit, int> tmpDele = delegate (int _index, bool _blockByUI, RaycastHit _hit, int _hitIndex)
                {
                    if (!_blockByUI)
                    {
                        MapUnitDown(unit);
                    }
                };

                SuperFunction.Instance.AddEventListener(go, SuperRaycast.GetMouseButtonDown, tmpDele);

                tmpDele = delegate (int _index, bool _blockByUI, RaycastHit _hit, int _hitIndex)
                {
                    if (!_blockByUI)
                    {
                        MapUnitEnter(unit);
                    }
                };

                SuperFunction.Instance.AddEventListener(go, SuperRaycast.GetMouseEnter, tmpDele);

                SuperFunction.SuperFunctionCallBack1<bool> tmpDele2 = delegate (int _index0, bool _blockByUI)
                {
                    if (!_blockByUI)
                    {
                        MapUnitExit(unit);
                    }
                };

                SuperFunction.Instance.AddEventListener(go, SuperRaycast.GetMouseExit, tmpDele2);

                mapUnitDic.Add(index, unit);

                unit.Init(index);

                SetMapUnitColor(unit);

                MapData.MapUnitType mapUnitType = battle.mapData.dic[index];

                if (mapUnitType == MapData.MapUnitType.RIVER || mapUnitType == MapData.MapUnitType.HILL)
                {
                    Destroy(unit.GetComponent<Collider>());
                }

                if (!initBounds)
                {
                    initBounds = true;

                    bounds = go.GetComponent<Renderer>().bounds;
                }
                else
                {
                    bounds.Encapsulate(go.GetComponent<Renderer>().bounds);
                }

                index++;
            }
        }

        battleContentContainer.localPosition = new Vector3(-bounds.center.x, -bounds.center.y, 0);

        defaultScale = Mathf.Min(viewport.extents.x / (bounds.extents.x + boundFix * 0.5f), viewport.extents.y / (bounds.extents.y + boundFix * 0.5f));

        battleContainer.localScale = new Vector3(defaultScale, defaultScale, defaultScale);

        bg.transform.localPosition = new Vector3(-viewportXFix / defaultScale, -viewport.center.y / defaultScale, 0);

        float scale;

        if (bg.sprite.rect.width / bg.sprite.rect.height > stepV.x / stepV.y)
        {
            scale = stepV.y / bg.sprite.rect.height / defaultScale * 2 * bg.sprite.pixelsPerUnit;
        }
        else
        {
            scale = stepV.x / bg.sprite.rect.width / defaultScale * 2 * bg.sprite.pixelsPerUnit;
        }

        bg.transform.localScale = new Vector3(scale, scale, 1);

        //bg.transform.localScale = new Vector3(stepV.x / bg.sprite.rect.width / defaultScale * 2 * bg.sprite.pixelsPerUnit, stepV.y / bg.sprite.rect.height / defaultScale * 2 * bg.sprite.pixelsPerUnit, 1);

        FixBounds();
    }

    private void CreateCards()
    {
        List<int> mHandCards;

        List<int> oHandCards;

        if (battle.clientIsMine)
        {
            mHandCards = battle.mHandCards;

            oHandCards = battle.oHandCards;
        }
        else
        {
            mHandCards = battle.oHandCards;

            oHandCards = battle.mHandCards;
        }

        int index = 0;

        for (int i = 0; i < mHandCards.Count; i++)
        {
            int uid = mHandCards[i];

            if (battle.GetSummonContainsKey(uid))
            {
                continue;
            }

            int id = battle.GetCard(uid);

            GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/HeroCard.prefab", null);

            HeroCard hero = go.GetComponent<HeroCard>();

            hero.Init(this, battleControl, uid, id);

            hero.SetFrameVisible(false);

            cardDic.Add(uid, hero);

            go.transform.SetParent(cardContainer, false);

            float cardWidth = (go.transform as RectTransform).sizeDelta.x;
            float cardHeight = (go.transform as RectTransform).sizeDelta.y;

            float fixX = (cardContainer.rect.width - cardWidth * (mHandCards.Count - battle.GetSummonNum())) * 0.5f;

            (go.transform as RectTransform).anchoredPosition = new Vector2(fixX - 0.5f * cardContainer.rect.width + cardWidth * 0.5f + index * cardWidth, -0.5f * cardContainer.rect.height + cardHeight * 0.5f);

            index++;
        }

        oCardNumTf.text = oHandCards.Count.ToString();
    }

    private void CreateSummonHeros()
    {
        IEnumerator<KeyValuePair<int, int>> enumerator = battle.GetSummonEnumerator();

        while (enumerator.MoveNext())
        {
            AddCardToMap(enumerator.Current.Key, enumerator.Current.Value);
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
        mMoneyTf.text = battle.GetNowMoney(battle.clientIsMine).ToString();

        oMoneyTf.text = battle.GetNowMoney(!battle.clientIsMine).ToString();
    }

    private void CreateScoreTf()
    {
        if (battle.clientIsMine)
        {
            mScoreTf.text = battle.mScore.ToString();
            oScoreTf.text = battle.oScore.ToString();
        }
        else
        {
            mScoreTf.text = battle.oScore.ToString();
            oScoreTf.text = battle.mScore.ToString();
        }
    }

    private void CreateRoundNumLeftTf()
    {
        roundNumLeftTf.text = (battle.maxRoundNum - battle.roundNum).ToString();
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
                GameObject go = CreateArrow(cellData.attackers[i].pos, pos, new Color(1, 0, 0, 0.7f), i);

                arrowList.Add(go);
            }

            for (int i = 0; i < cellData.supporters.Count; i++)
            {
                GameObject go = CreateArrow(cellData.supporters[i].pos, pos, new Color(0, 1, 0, 0.7f), i);

                arrowList.Add(go);
            }

            for (int i = 0; i < cellData.shooters.Count; i++)
            {
                GameObject go = CreateShootArrow(cellData.shooters[i].pos, pos, new Color(1, 1, 0, 0.7f), i);

                arrowList.Add(go);
            }
        }
    }

    private GameObject CreateArrow(int _start, int _end, Color _color, int _index)
    {
        GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/Arrow.prefab", null);

        Arrow arrow = go.GetComponent<Arrow>();

        go.transform.SetParent(arrowContainer, false);

        MapUnit start = mapUnitDic[_start];

        MapUnit end = mapUnitDic[_end];

        go.transform.localPosition = (start.transform.localPosition + end.transform.localPosition) * 0.5f;

        float angle = Mathf.Atan2(end.transform.localPosition.y - start.transform.localPosition.y, end.transform.localPosition.x - start.transform.localPosition.x);

        Quaternion q = new Quaternion();

        q.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);

        go.transform.localRotation = q;

        arrow.SetColor(_color);

        arrow.SetIndex(_index);

        return go;
    }

    private GameObject CreateShootArrow(int _start, int _end, Color _color, int _index)
    {
        GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/ShootArrow.prefab", null);

        Arrow arrow = go.GetComponent<Arrow>();

        go.transform.SetParent(arrowContainer, false);

        MapUnit start = mapUnitDic[_start];

        MapUnit end = mapUnitDic[_end];

        go.transform.localPosition = (start.transform.localPosition + end.transform.localPosition) * 0.5f;

        float angle = Mathf.Atan2(end.transform.localPosition.y - start.transform.localPosition.y, end.transform.localPosition.x - start.transform.localPosition.x);

        Quaternion q = new Quaternion();

        q.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);

        go.transform.localRotation = q;

        float scale = Vector3.Distance(start.transform.localPosition, end.transform.localPosition) / (mapUnitWidth * sqrt3 * 4);

        go.transform.localScale = new Vector3(scale, scale, scale);

        arrow.SetColor(_color);

        //arrow.SetIndex(_index);

        return go;
    }

    private void MapUnitDown(MapUnit _mapUnit)
    {
        MapUnitDownReal(_mapUnit);

        if (nowChooseHeroCanAction)
        {
            int targetPos = battle.GetActionContainsKey(GetNowChooseHero().pos);

            if (targetPos != -1)
            {
                if (targetPos == _mapUnit.index)
                {
                    isDoingHeroAction = true;
                }

                return;
            }

            if (_mapUnit.index == GetNowChooseHero().pos)
            {
                isDoingHeroAction = true;
            }
        }
    }

    private void MapUnitEnter(MapUnit _mapUnit)
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

    private void MapUnitExit(MapUnit _mapUnit)
    {
        if (isDoingHeroAction)
        {
            mouseHasExited = true;

            int targetPos = battle.GetActionContainsKey(GetNowChooseHero().pos);

            if (targetPos != -1)
            {
                if (_mapUnit.index == targetPos)
                {
                    battle.ClientRequestUnaction(GetNowChooseHero().pos);

                    ClearMoves();

                    CreateMoves();
                }
            }
        }
    }

    private void MapUnitUpAsButton(MapUnit _mapUnit)
    {
        if (mouseHasExited)
        {
            return;
        }

        if (battle.GetSummonContainsValue(_mapUnit.index))
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

                CreateCards();

                ClearSummonHeros();

                CreateSummonHeros();
            }
            else
            {
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
        return battle.ClientRequestSummon(_cardUid, _pos);
    }

    private void UnsummonHero(int _cardUid)
    {
        battle.ClientRequestUnsummon(_cardUid);

        CreateMoneyTf();

        ClearCards();

        CreateCards();

        ClearSummonHeros();

        CreateSummonHeros();
    }

    private HeroBattle AddHeroToMap(Hero _hero)
    {
        GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/HeroBattle.prefab", null);

        HeroBattle hero = go.GetComponent<HeroBattle>();

        heroDic.Add(_hero.pos, hero);

        AddHeroToMapReal(hero, _hero.pos);

        hero.Init(this, battleControl, _hero, GetHeroUid());

        return hero;
    }

    private HeroBattle AddCardToMap(int _cardUid, int _pos)
    {
        int cardID = battle.GetCard(_cardUid);

        GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/HeroBattle.prefab", null);

        HeroBattle hero = go.GetComponent<HeroBattle>();

        summonHeroDic.Add(_pos, hero);

        AddHeroToMapReal(hero, _pos);

        hero.Init(this, battleControl, _cardUid, cardID);

        return hero;
    }

    private void AddHeroToMapReal(HeroBattle _heroCard, int _pos)
    {
        MapUnit mapUnit = mapUnitDic[_pos];

        _heroCard.SetFrameVisible(false);

        _heroCard.transform.SetParent(heroContainer, false);

        _heroCard.transform.localPosition = mapUnit.transform.localPosition;
    }


    public void ActionBtClick()
    {
        RefreshTouchable(false);

        ClearNowChooseCard();

        ClearNowChooseHero();

        battle.ClientRequestDoAction();
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
            RequestRefreshData();

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

    public void RequestRefreshData()
    {
        battle.ClientRequestRefreshData();
    }

    private System.Random random = new System.Random();

    private int GetRandomValue(int _max)
    {
        return random.Next(_max);
    }

    private void CreateAiAction()
    {
        BattleAi.Start(battle, battle.clientIsMine, GetRandomValue);

        ClearMoves();

        CreateMoves();

        ClearSummonHeros();

        CreateSummonHeros();

        ClearCards();

        CreateCards();
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
        RefreshTouchable(false);

        SuperSequenceControl.Start(DoActionReal, _step);
    }

    private IEnumerator DoActionReal(int _index, SuperEnumerator<ValueType> _step)
    {
        while (_step.MoveNext())
        {
            ValueType vo = _step.Current;

            if (vo is BattleShootVO)
            {
                SuperSequenceControl.Start(battleControl.Shoot, _index, (BattleShootVO)vo);

                yield return null;
            }
            else if (vo is BattleMoveVO)
            {
                SuperSequenceControl.Start(battleControl.Move, _index, (BattleMoveVO)vo);

                yield return null;
            }
            else if (vo is BattleRushVO)
            {
                SuperSequenceControl.Start(battleControl.Rush, _index, (BattleRushVO)vo);

                yield return null;
            }
            else if (vo is BattlePrepareAttackVO)
            {
                SuperSequenceControl.Start(battleControl.PrepareAttack, _index, (BattlePrepareAttackVO)vo);

                yield return null;
            }
            else if (vo is BattleAttackAndCounterVO)
            {
                SuperSequenceControl.Start(battleControl.AttackAndCounter, _index, (BattleAttackAndCounterVO)vo);

                yield return null;
            }
            else if (vo is BattleAttackBothVO)
            {
                SuperSequenceControl.Start(battleControl.AttackBoth, _index, (BattleAttackBothVO)vo);

                yield return null;
            }
            else if (vo is BattleDeathVO)
            {
                SuperSequenceControl.Start(battleControl.Die, _index, (BattleDeathVO)vo);

                yield return null;
            }
            else if (vo is BattleSummonVO)
            {
                SuperSequenceControl.Start(DoSummon, _index, (BattleSummonVO)vo);

                yield return null;
            }
            else if (vo is BattleAttackOverVO)
            {
                SuperSequenceControl.Start(battleControl.AttackOver, _index, (BattleAttackOverVO)vo);

                yield return null;
            }
            else if (vo is BattleAddCardsVO)
            {
                DoAddCards((BattleAddCardsVO)vo);
            }
            else if (vo is BattleMoneyChangeVO)
            {
                DoMoneyChange((BattleMoneyChangeVO)vo);
            }
            else if (vo is BattlePrepareRushVO)
            {
                DoPrepareRush();
            }
            else if (vo is BattleRushOverVO)
            {
                DoRushOver();
            }
            else if (vo is BattleRecoverVO)
            {
                DoRecover();
            }
            else if (vo is BattleRoundStartVO)
            {
                DoRecover();
            }
            else if (vo is BattleRoundOverVO)
            {
                DoRecover();
            }
            else if (vo is BattleTriggerAuraVO)
            {
                SuperSequenceControl.Start(battleControl.TriggerAura, _index, (BattleTriggerAuraVO)vo);

                yield return null;
            }
            else if (vo is BattleRefreshVO)
            {
                RefreshDataBeforeBattle();
            }
            else if (vo is BattleSupportVO)
            {
                SuperSequenceControl.Start(battleControl.Support, _index, (BattleSupportVO)vo);

                yield return null;
            }
            else if (vo is BattleScoreChangeVO)
            {
                DoScoreChange((BattleScoreChangeVO)vo);
            }
        }

        RefreshData();

        Battle.BattleResult battleResult = (Battle.BattleResult)_step.Current;

        if (battleResult != Battle.BattleResult.NOT_OVER)
        {
            BattleOver(battleResult);
        }
    }

    private IEnumerator DoSummon(int _index, int _lastIndex, BattleSummonVO _vo)
    {
        Hero hero = battle.heroMapDic[_vo.pos];

        HeroBattle heroBattle = AddHeroToMap(hero);

        heroBattle.transform.localScale = Vector3.zero;

        Action<float> toDel = delegate (float obj)
        {
            float scale = obj;

            heroBattle.transform.localScale = new Vector3(scale, scale, scale);

            heroBattle.transform.localPosition = new Vector3(heroBattle.transform.localPosition.x, heroBattle.transform.localPosition.y, 100 * (1 - obj));
        };

        SuperSequenceControl.To(10f, 1f, 0.3f, toDel, _index);

        yield return null;

        SuperSequenceControl.DelayCall(0.8f, _lastIndex);
    }

    public void SetMapUnitColor(MapUnit _unit)
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

    private void DoPrepareRush()
    {
        Dictionary<int, HeroBattle>.ValueCollection.Enumerator enumerator = heroDic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            enumerator.Current.RefreshAttack();
        }
    }

    private void DoRushOver()
    {
        Dictionary<int, HeroBattle>.ValueCollection.Enumerator enumerator = heroDic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            enumerator.Current.RefreshAttackWithoutShield();
        }
    }

    private void DoAddCards(BattleAddCardsVO _vo)
    {
        ClearCards();

        CreateCards();
    }

    private void DoMoneyChange(BattleMoneyChangeVO _vo)
    {
        CreateMoneyTf();
    }

    private void DoScoreChange(BattleScoreChangeVO _vo)
    {
        CreateScoreTf();
    }

    public void DoRecover()
    {
        Dictionary<int, HeroBattle>.ValueCollection.Enumerator enumerator = heroDic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            enumerator.Current.RefreshHpAndShield();

            enumerator.Current.RefreshAttackWithoutShield();
        }
    }

    private void ScaleChange(int _index, float _scrollValue, Vector2 _pos)
    {
        if (!canAction)
        {
            return;
        }

        Vector3 v = mainCamera.ScreenToWorldPoint(_pos);

        Vector3 v2 = battleContainer.InverseTransformPoint(v);

        battleContainer.localPosition = new Vector3(battleContainer.localPosition.x + v2.x, battleContainer.localPosition.y + v2.y, battleContainer.localPosition.z);

        float scale;

        if (_scrollValue > 1)
        {
            scale = battleContainer.localScale.x * fixStep;
        }
        else
        {
            scale = battleContainer.localScale.x * (1 / fixStep);
        }

        scale = Mathf.Clamp(scale, defaultScale * minScale, defaultScale * maxScale);

        battleContainer.localScale = new Vector3(scale, scale, scale);

        Vector3 v3 = battleContainer.TransformPoint(v2);

        battleContainer.localPosition = new Vector3(battleContainer.localPosition.x - v3.x + v.x, battleContainer.localPosition.y - v3.y + v.y, battleContainer.localPosition.z);

        FixBounds();
    }

    private void FixBounds()
    {
        Bounds tmpBounds = bounds;

        tmpBounds.Expand(boundFix);

        tmpBounds.extents = new Vector3(tmpBounds.extents.x * battleContainer.transform.localScale.x, tmpBounds.extents.y * battleContainer.transform.localScale.y, tmpBounds.extents.z * battleContainer.transform.localScale.z);

        if (tmpBounds.extents.x < viewport.extents.x)
        {
            battleContainer.localPosition = new Vector3(viewport.center.x, battleContainer.localPosition.y, battleContainer.localPosition.z);
        }
        else if (battleContainer.localPosition.x - tmpBounds.extents.x > viewport.min.x)
        {
            battleContainer.localPosition = new Vector3(viewport.min.x + tmpBounds.extents.x, battleContainer.localPosition.y, battleContainer.localPosition.z);
        }
        else if (battleContainer.localPosition.x + tmpBounds.extents.x < viewport.max.x)
        {
            battleContainer.localPosition = new Vector3(viewport.max.x - tmpBounds.extents.x, battleContainer.localPosition.y, battleContainer.localPosition.z);
        }

        if (tmpBounds.extents.y < viewport.extents.y)
        {
            battleContainer.localPosition = new Vector3(battleContainer.localPosition.x, viewport.center.y, battleContainer.localPosition.z);
        }
        else if (battleContainer.localPosition.y - tmpBounds.extents.y > viewport.min.y)
        {
            battleContainer.localPosition = new Vector3(battleContainer.localPosition.x, viewport.min.y + tmpBounds.extents.y, battleContainer.localPosition.z);
        }
        else if (battleContainer.localPosition.y + tmpBounds.extents.y < viewport.max.y)
        {
            battleContainer.localPosition = new Vector3(battleContainer.localPosition.x, viewport.max.y - tmpBounds.extents.y, battleContainer.localPosition.z);
        }
    }

    private void GetMouseDown(int _index, bool _blockByUI, RaycastHit _hit, int _hitIndex)
    {
        if (!_blockByUI && _hitIndex == 0)
        {
            downPos = lastPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            isDown = DownType.BACKGROUND;
        }
    }

    private void MapUnitDownReal(MapUnit _mapUnit)
    {
        downPos = lastPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        isDown = DownType.MAPUNIT;

        downMapUnit = _mapUnit;
    }

    private void GetMouseMove(int _index, bool _blockByUI, RaycastHit _hit, int _hitIndex)
    {
        if (isDown != DownType.NULL)
        {
            if (!_blockByUI || hasMove)
            {
                BackgroundMove();
            }
        }
    }

    private void BackgroundMove()
    {
        Vector3 nowPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (!hasMove && Vector2.Distance(nowPos, downPos) > moveThreshold)
        {
            hasMove = true;
        }

        if (!isDoingHeroAction && hasMove)
        {
            Vector3 v2 = new Vector2(nowPos.x - lastPos.x, nowPos.y - lastPos.y);

            battleContainer.localPosition = new Vector3(battleContainer.localPosition.x + v2.x, battleContainer.localPosition.y + v2.y, battleContainer.localPosition.z);

            FixBounds();
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

    private void ShowDesc(string _str, Action _callBack)
    {
        descPanel.Show(_str, _callBack);
    }

    private void Alert(string _str, Action _callBack)
    {
        alertPanel.Alert(_str, _callBack);
    }

    private void ClickDesc(int _index, int _id)
    {
        if (_id > 0)
        {
            DescSDS sds = StaticData.GetData<DescSDS>(_id);

            ShowDesc(sds.desc, null);
        }
    }

    public void ClearMapUnitIcon()
    {
        IEnumerator<MapUnit> enumerator = mapUnitDic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            enumerator.Current.SetIconVisible(false);
        }
    }

    public void ClickHeroBattleShowMapUnitIcon(HeroBattle _hero)
    {
        ClearMapUnitIcon();

        if (_hero.isHero && _hero.canAction)
        {
            List<int> list = BattlePublicTools.GetNeighbourPos(battle.mapData, _hero.pos);

            for (int i = 0; i < list.Count; i++)
            {
                int pos = list[i];

                if (battle.GetPosIsMine(pos) == _hero.isMine && !battle.GetSummonContainsValue(pos))
                {
                    MapUnit unit = mapUnitDic[pos];

                    unit.SetIconVisible(true);

                    unit.SetIconColor(new Color(0, 1, 0, 0.8f));
                }
            }

            list = _hero.GetCanAttackPos();

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    int pos = list[i];

                    MapUnit unit = mapUnitDic[pos];

                    unit.SetIconVisible(true);

                    unit.SetIconColor(new Color(1, 0, 0, 0.8f));
                }
            }
        }
    }

    public void ClickHeroCardShowMapUnitIcon(HeroCard _hero)
    {
        ClearMapUnitIcon();

        if (_hero.sds.cost <= battle.GetNowMoney(battle.clientIsMine))
        {
            IEnumerator<KeyValuePair<int, MapUnit>> enumerator = mapUnitDic.GetEnumerator();

            while (enumerator.MoveNext())
            {
                KeyValuePair<int, MapUnit> pair = enumerator.Current;

                int pos = pair.Key;

                if (battle.CheckPosCanSummon(battle.clientIsMine, pos))
                {
                    MapUnit unit = pair.Value;

                    unit.SetIconVisible(true);

                    unit.SetIconColor(new Color(1, 1, 1, 0.8f));
                }
            }
        }
    }
}
