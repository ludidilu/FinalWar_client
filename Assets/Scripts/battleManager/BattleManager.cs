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
using superTween;
using publicTools;

public class BattleManager : MonoBehaviour
{
    public const string BATTLE_START = "battleStart";

    public const string BATTLE_QUIT = "battleQuit";

    public const string BATTLE_SEND_DATA = "battleSendData";

    public const string BATTLE_RECEIVE_DATA = "battleReceiveData";

    public const string BATTLE_HERO_ACTION = "battleHeroAction";

    public const string BATTLE_HERO_UNACTION = "battleHeroUnaction";

    public const string BATTLE_HERO_SUMMON = "battleHeroSummon";

    public const string BATTLE_HERO_UNSUMMON = "battleHeroUnsummon";

    public const string BATTLE_ROUND_OVER = "battleRoundOver";

    public const string BATTLE_CHOOSE_CARD = "battleChooseCard";

    public const string BATTLE_CHOOSE_HERO = "battleChooseHero";

    public const string BATTLE_ACTION = "battleAction";

    private static readonly float sqrt3 = Mathf.Sqrt(3);

    [SerializeField]
    private BattleControl battleControl;

    [SerializeField]
    private float moveThreshold = 0.1f;

    [SerializeField]
    private float mapUnitWidth = 30;

    [SerializeField]
    private float mapUnitScale = 55;

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
    public Color myMapUnitColor;

    [SerializeField]
    public Color myBaseColor;

    [SerializeField]
    public Color oppMapUnitColor;

    [SerializeField]
    public Color oppBaseColor;

    [SerializeField]
    public Color riverColor;

    [SerializeField]
    public Color hillColor;

    [SerializeField]
    public Transform battleContainer;

    [SerializeField]
    private Transform battleContentContainer;

    [SerializeField]
    private RectTransform uiContainer;

    [SerializeField]
    private Vector2 uiContainerFix;

    [SerializeField]
    private RectTransform myCardContainer;

    [SerializeField]
    private RectTransform oppCardContainer;

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
    private Text mCardsNumTf;

    [SerializeField]
    private Text oCardsNumTf;

    [SerializeField]
    private Text roundNumLeftTf;

    [SerializeField]
    private GameObject actionBt;

    [SerializeField]
    public GameObject quitBt;

    [SerializeField]
    private HeroDetail heroDetail;

    [SerializeField]
    private GameObject backGround;

    [SerializeField]
    private DescPanel descPanel;

    [SerializeField]
    private AlertPanel alertPanel;

    [SerializeField]
    public SpriteRenderer bg;

    [SerializeField]
    private float bgScaleFix;

    [SerializeField]
    private CanvasGroup alphaCg;

    [SerializeField]
    private int auraDescID;

    [SerializeField]
    private int shootDescID;

    [SerializeField]
    private int supportDescID;

    [SerializeField]
    private int effectDescID;

    [SerializeField]
    private int featureDescID;

    [SerializeField]
    private float minDefaultScale;

    [SerializeField]
    private float maxDefaultScale;

    [SerializeField]
    private float minMapScale;

    [SerializeField]
    private float maxMapScale;

    [HideInInspector]
    public GameObject eventGo;

    [SerializeField]
    private float mapDragFix;

    [SerializeField]
    private float mapDragRevertFix;

    public Battle_client battle = new Battle_client();

    public Dictionary<int, MapUnit> mapUnitDic = new Dictionary<int, MapUnit>();

    public Dictionary<int, HeroBattle> heroDic = new Dictionary<int, HeroBattle>();

    [HideInInspector]
    public List<HeroCard> cardList = new List<HeroCard>();

    [HideInInspector]
    public List<GameObject> oppCardList = new List<GameObject>();

    private Dictionary<int, HeroBattle> summonHeroDic = new Dictionary<int, HeroBattle>();

    private List<ShootArrow> arrowList = new List<ShootArrow>();

    private Vector2 downPos;

    private Vector2 lastPos;

    private Bounds bounds;

    public Bounds viewport;

    [HideInInspector]
    public float defaultScale;

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
            SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_CHOOSE_CARD);

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
            SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_CHOOSE_HERO);

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

    private bool isUiShow = true;

    private int heroUid;

    [HideInInspector]
    public string auraDescFix;

    [HideInInspector]
    public string shootDescFix;

    [HideInInspector]
    public string supportDescFix;

    [HideInInspector]
    public string effectDescFix;

    [HideInInspector]
    public string featureDescFix;

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

        viewport.extents = new Vector3(viewport.extents.x, viewport.extents.y, viewport.extents.z);

        SuperFunction.Instance.AddEventListener<int>(ClickText.eventGo, ClickText.EVENT_NAME, ClickDesc);

        SuperFunction.Instance.AddEventListener<int>(ClickImage.eventGo, ClickImage.EVENT_NAME, ClickDesc);

        gameObject.SetActive(false);
    }

    public void Init(GameObject _eventGo)
    {
        eventGo = _eventGo;

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

        auraDescFix = StaticData.GetData<DescSDS>(auraDescID).desc;

        shootDescFix = StaticData.GetData<DescSDS>(shootDescID).desc;

        supportDescFix = StaticData.GetData<DescSDS>(supportDescID).desc;

        effectDescFix = StaticData.GetData<DescSDS>(effectDescID).desc;

        featureDescFix = StaticData.GetData<DescSDS>(featureDescID).desc;
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

            SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_START);
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
        IEnumerator<MapUnit> enumerator = mapUnitDic.Values.GetEnumerator();

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
        for (int i = 0; i < cardList.Count; i++)
        {
            Destroy(cardList[i].gameObject);
        }

        cardList.Clear();

        for (int i = 0; i < oppCardList.Count; i++)
        {
            Destroy(oppCardList[i]);
        }

        oppCardList.Clear();
    }

    private void ClearSummonHeros()
    {
        IEnumerator<HeroBattle> enumerator2 = summonHeroDic.Values.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            Destroy(enumerator2.Current.gameObject);
        }

        summonHeroDic.Clear();
    }

    public void ClearHeros()
    {
        heroUid = 0;

        IEnumerator<HeroBattle> enumerator2 = heroDic.Values.GetEnumerator();

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
            Destroy(arrowList[i].gameObject);
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

                GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/battle/MapUnit.prefab", null);

                go.name = string.Format("MapUnit{0}", index);

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

        defaultScale = Mathf.Clamp(defaultScale, minDefaultScale, maxDefaultScale);

        battleContainer.localScale = new Vector3(defaultScale, defaultScale, 1);

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

        bg.transform.localScale = new Vector3(scale * bgScaleFix, scale * bgScaleFix, 1);

        //bg.transform.localScale = new Vector3(stepV.x / bg.sprite.rect.width / defaultScale * 2 * bg.sprite.pixelsPerUnit, stepV.y / bg.sprite.rect.height / defaultScale * 2 * bg.sprite.pixelsPerUnit, 1);

        FixBounds();
    }

    private void CreateCards()
    {
        List<int> mHandCards;

        List<int> oHandCards;

        Queue<int> mCards;

        Queue<int> oCards;

        if (battle.clientIsMine)
        {
            mHandCards = battle.mHandCards;

            oHandCards = battle.oHandCards;

            mCards = battle.mCards;

            oCards = battle.oCards;
        }
        else
        {
            mHandCards = battle.oHandCards;

            oHandCards = battle.mHandCards;

            mCards = battle.oCards;

            oCards = battle.mCards;
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

            GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/battle/HeroCard.prefab", null);

            go.name = string.Format("HeroCard{0}", id);

            HeroCard hero = go.GetComponent<HeroCard>();

            hero.Init(this, battleControl, uid, id);

            hero.SetFrameVisible(false);

            cardList.Add(hero);

            go.transform.SetParent(myCardContainer, false);

            float cardWidth = (go.transform as RectTransform).sizeDelta.x;
            float cardHeight = (go.transform as RectTransform).sizeDelta.y;

            float fixX = (myCardContainer.rect.width - cardWidth * (mHandCards.Count - battle.GetSummonNum())) * 0.5f;

            (go.transform as RectTransform).anchoredPosition = new Vector2(fixX - 0.5f * myCardContainer.rect.width + cardWidth * 0.5f + index * cardWidth, 0);

            index++;
        }

        for (int i = 0; i < oHandCards.Count; i++)
        {
            GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/battle/OppHeroCard.prefab", null);

            oppCardList.Add(go);

            go.transform.SetParent(oppCardContainer, false);

            float cardWidth = (go.transform as RectTransform).sizeDelta.x;
            float cardHeight = (go.transform as RectTransform).sizeDelta.y;

            float fixX = (oppCardContainer.rect.width - cardWidth * oHandCards.Count) * 0.5f;

            (go.transform as RectTransform).anchoredPosition = new Vector2(fixX - 0.5f * oppCardContainer.rect.width + cardWidth * 0.5f + i * cardWidth, 0);
        }

        mCardsNumTf.text = mCards.Count.ToString();

        if (mCards.Count == 0)
        {
            mCardsNumTf.color = Color.red;
        }
        else
        {
            mCardsNumTf.color = Color.white;
        }

        oCardsNumTf.text = oCards.Count.ToString();

        if (oCards.Count == 0)
        {
            oCardsNumTf.color = Color.red;
        }
        else
        {
            oCardsNumTf.color = Color.white;
        }
    }

    private void CreateSummonHeros()
    {
        IEnumerator<KeyValuePair<int, int>> enumerator = battle.GetSummonEnumerator();

        while (enumerator.MoveNext())
        {
            AddCardToMap(enumerator.Current.Key, enumerator.Current.Value);
        }
    }

    public void CreateHeros()
    {
        IEnumerator<Hero> enumerator = battle.GetHeroEnumerator();

        while (enumerator.MoveNext())
        {
            AddHeroToMap(enumerator.Current);
        }
    }

    public void CreateMoneyTf()
    {
        Queue<int> mCards;
        Queue<int> oCards;

        if (battle.clientIsMine)
        {
            mCards = battle.mCards;
            oCards = battle.oCards;
        }
        else
        {
            mCards = battle.oCards;
            oCards = battle.mCards;
        }

        int mMoney = battle.GetNowMoney(battle.clientIsMine);

        mMoneyTf.text = mMoney.ToString();

        if (mMoney + BattleConst.ADD_MONEY > BattleConst.MAX_MONEY && mCards.Count > 0)
        {
            mMoneyTf.color = Color.red;
        }
        else
        {
            mMoneyTf.color = Color.white;
        }

        int oMoney = battle.GetNowMoney(!battle.clientIsMine);

        oMoneyTf.text = oMoney.ToString();

        if (oMoney + BattleConst.ADD_MONEY > BattleConst.MAX_MONEY && oCards.Count > 0)
        {
            oMoneyTf.color = Color.red;
        }
        else
        {
            oMoneyTf.color = Color.white;
        }
    }

    private void CreateScoreTf()
    {
        if (battle.clientIsMine)
        {
            mScoreTf.text = battle.mScore.ToString();
            oScoreTf.text = battle.oScore.ToString();

            if (battle.mScore > battle.oScore)
            {
                mScoreTf.color = Color.green;
                oScoreTf.color = Color.red;
            }
            else if (battle.mScore < battle.oScore)
            {
                mScoreTf.color = Color.red;
                oScoreTf.color = Color.green;
            }
            else
            {
                mScoreTf.color = Color.white;
                oScoreTf.color = Color.white;
            }
        }
        else
        {
            mScoreTf.text = battle.oScore.ToString();
            oScoreTf.text = battle.mScore.ToString();

            if (battle.mScore > battle.oScore)
            {
                mScoreTf.color = Color.red;
                oScoreTf.color = Color.green;
            }
            else if (battle.mScore < battle.oScore)
            {
                mScoreTf.color = Color.green;
                oScoreTf.color = Color.red;
            }
            else
            {
                mScoreTf.color = Color.white;
                oScoreTf.color = Color.white;
            }
        }
    }

    private void CreateRoundNumLeftTf()
    {
        int leftRoundNum = battle.maxRoundNum - battle.roundNum;

        roundNumLeftTf.text = leftRoundNum.ToString();

        if (leftRoundNum == 1)
        {
            roundNumLeftTf.color = Color.red;
        }
        else
        {
            roundNumLeftTf.color = Color.white;
        }
    }

    private void CreateMoves()
    {
        BattleData battleData = battle.GetBattleData();

        IEnumerator<KeyValuePair<int, BattleCellData>> enumerator2 = battleData.actionDic.GetEnumerator();

        while (enumerator2.MoveNext())
        {
            int pos = enumerator2.Current.Key;

            BattleCellData cellData = enumerator2.Current.Value;

            for (int i = 0; i < cellData.attackers.Count; i++)
            {
                int index = cellData.attackers.Count > 1 ? i : -1;

                ShootArrow arrow = CreateArrow(cellData.attackers[i].pos, pos, new Color(1, 0, 0, 0.7f), index);

                arrowList.Add(arrow);
            }

            for (int i = 0; i < cellData.supporters.Count; i++)
            {
                int index = cellData.supporters.Count > 1 ? i : -1;

                ShootArrow arrow = CreateArrow(cellData.supporters[i].pos, pos, new Color(0, 1, 0, 0.7f), index);

                arrowList.Add(arrow);
            }

            for (int i = 0; i < cellData.shooters.Count; i++)
            {
                ShootArrow arrow = CreateShootArrow(cellData.shooters[i].pos, pos, new Color(1, 1, 0, 0.7f), i);

                arrowList.Add(arrow);
            }
        }
    }

    private ShootArrow CreateArrow(int _start, int _end, Color _color, int _index)
    {
        GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/battle/Arrow.prefab", null);

        Arrow arrow = go.GetComponent<Arrow>();

        go.transform.SetParent(arrowContainer, false);

        MapUnit start = mapUnitDic[_start];

        MapUnit end = mapUnitDic[_end];

        Vector3 tmpPos = (start.transform.position + end.transform.position) * 0.5f;

        go.transform.position = new Vector3(tmpPos.x, tmpPos.y, go.transform.position.z);

        float angle = Mathf.Atan2(end.transform.position.y - start.transform.position.y, end.transform.position.x - start.transform.position.x);

        Quaternion q = new Quaternion();

        q.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);

        go.transform.localRotation = q;

        arrow.SetColor(_color);

        arrow.SetIndex(_index);

        return arrow;
    }

    private ShootArrow CreateShootArrow(int _start, int _end, Color _color, int _index)
    {
        GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/battle/ShootArrow.prefab", null);

        ShootArrow arrow = go.GetComponent<ShootArrow>();

        go.transform.SetParent(arrowContainer, false);

        MapUnit start = mapUnitDic[_start];

        MapUnit end = mapUnitDic[_end];

        Vector3 tmpPos = (start.transform.position + end.transform.position) * 0.5f;

        go.transform.position = new Vector3(tmpPos.x, tmpPos.y, go.transform.position.z);

        float angle = Mathf.Atan2(end.transform.position.y - start.transform.position.y, end.transform.position.x - start.transform.position.x);

        Quaternion q = new Quaternion();

        q.eulerAngles = new Vector3(0, 0, angle * Mathf.Rad2Deg);

        go.transform.localRotation = q;

        float scale = Vector3.Distance(start.transform.localPosition, end.transform.localPosition) / (mapUnitWidth * sqrt3 * 4);

        go.transform.localScale = new Vector3(scale, scale, scale);

        arrow.SetColor(_color);

        return arrow;
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
                if (HeroAction(GetNowChooseHero().pos, _mapUnit.index))
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

            if (targetPos == _mapUnit.index)
            {
                bool b = HeroUnaction(GetNowChooseHero().pos);

                if (b)
                {
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
                    bool b = UnsummonHero(summonHero.cardUid);

                    if (b)
                    {
                        ClearNowChooseHero();

                        CreateMoneyTf();

                        ClearCards();

                        CreateCards();

                        ClearSummonHeros();

                        CreateSummonHeros();
                    }
                }
                else
                {
                    ClearNowChooseHero();

                    SetNowChooseHero(summonHero, false);

                    GetNowChooseHero().SetFrameVisible(true);
                }
            }
        }
        else if (battle.GetHeroMapContainsKey(_mapUnit.index))
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

    private bool HeroAction(int _pos, int _target)
    {
        if (!canAction)
        {
            return false;
        }

        bool b = battle.ClientRequestAction(_pos, _target);

        if (b)
        {
            SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_HERO_ACTION);
        }

        return b;
    }

    private bool HeroUnaction(int _pos)
    {
        if (!canAction)
        {
            return false;
        }

        SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_HERO_UNACTION);

        battle.ClientRequestUnaction(_pos);

        return true;
    }

    private bool SummonHero(int _cardUid, int _pos)
    {
        if (!canAction)
        {
            return false;
        }

        bool b = battle.ClientRequestSummon(_cardUid, _pos);

        if (b)
        {
            SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_HERO_SUMMON);
        }

        return b;
    }

    private bool UnsummonHero(int _cardUid)
    {
        if (!canAction)
        {
            return false;
        }

        SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_HERO_UNSUMMON);

        battle.ClientRequestUnsummon(_cardUid);

        return true;
    }

    public HeroBattle AddHeroToMap(Hero _hero)
    {
        GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/battle/HeroBattle.prefab", null);

        HeroBattle hero = go.GetComponent<HeroBattle>();

        heroDic.Add(_hero.pos, hero);

        AddHeroToMapReal(hero, _hero.pos);

        hero.Init(this, battleControl, _hero, GetHeroUid());

        return hero;
    }

    private HeroBattle AddCardToMap(int _cardUid, int _pos)
    {
        int cardID = battle.GetCard(_cardUid);

        GameObject go = GameObjectFactory.Instance.GetGameObject("Assets/Resource/prefab/battle/HeroBattle.prefab", null);

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

        _heroCard.transform.position = new Vector3(mapUnit.transform.position.x, mapUnit.transform.position.y, _heroCard.transform.position.z);
    }


    public void ActionBtClick()
    {
        SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_ACTION);

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

        if (isUiShow && !hasMove)
        {
            ResetBattleContainer();
        }

        if (Input.GetKeyUp(KeyCode.F5))
        {
            RequestRefreshData();
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            CreateAiAction();
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
        }
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
            //SuperRaycast.SetIsOpen(false, "a");

            //clickCg.blocksRaycasts = false;

            actionBt.SetActive(false);

            canAction = _canAction;
        }
        else if (!canAction && _canAction)
        {
            //SuperRaycast.SetIsOpen(true, "a");

            //clickCg.blocksRaycasts = true;

            actionBt.SetActive(true);

            canAction = _canAction;
        }
    }

    private void ExitBattle(Action _callBack)
    {
        ClearCards();

        CreateCards();

        CreateMoneyTf();

        CreateScoreTf();

        CreateRoundNumLeftTf();

        RefreshTouchable(true);

        Action dele = delegate ()
        {
            SuperRaycast.SetIsOpen(true, "ui");

            SuperGraphicRaycast.SetIsOpen(true, "ui");

            alphaCg.blocksRaycasts = true;

            isUiShow = true;

            _callBack();
        };

        alphaCg.alpha = 1;

        SuperTween.Instance.To(1, 0, 0.5f, SetUiContainerSize, dele);
    }

    private void EnterBattle(Action _callBack)
    {
        heroDetail.Hide();

        descPanel.Close();

        isUiShow = false;

        SuperRaycast.SetIsOpen(false, "ui");

        SuperGraphicRaycast.SetIsOpen(false, "ui");

        alphaCg.blocksRaycasts = false;

        Vector2 startPos = battleContainer.position;

        float startScale = battleContainer.localScale.x;

        Action<float> toDele = delegate (float _v)
        {
            Vector2 nowPos = Vector2.Lerp(startPos, Vector2.zero, _v);

            float nowScale = PublicTools.FloatLerp(startScale, defaultScale, _v);

            battleContainer.position = new Vector3(nowPos.x, nowPos.y, battleContainer.position.z);

            battleContainer.localScale = new Vector3(nowScale, nowScale, battleContainer.localScale.z);

            SetUiContainerSize(_v);

            AlphaOutSummonHero(1 - _v);

            AlphaOutMove(1 - _v);

            IEnumerator<HeroBattle> enumerator = heroDic.Values.GetEnumerator();

            while (enumerator.MoveNext())
            {
                HeroBattle hero = enumerator.Current;

                if (!hero.canAction)
                {
                    hero.SetColorFix(PublicTools.FloatLerp(hero.colorFix, 1, _v));
                }
            }
        };

        Action dele = delegate ()
        {
            alphaCg.alpha = 0;

            ClearSummonHeros();

            ClearMoves();

            _callBack();
        };

        SuperTween.Instance.To(0, 1, 0.5f, toDele, dele);
    }

    private void SetUiContainerSize(float _v)
    {
        uiContainer.sizeDelta = Vector2.Lerp(Vector2.zero, uiContainerFix, _v);
    }

    private void DoAction(SuperEnumerator<ValueType> _step)
    {
        Action dele = delegate ()
        {
            SuperSequenceControl.Start(DoActionReal, _step);
        };

        EnterBattle(dele);
    }

    private void AlphaOutSummonHero(float _v)
    {
        IEnumerator<HeroBattle> enumerator = summonHeroDic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            enumerator.Current.SetAlpha(_v);
        }
    }

    private void AlphaOutMove(float _v)
    {
        for (int i = 0; i < arrowList.Count; i++)
        {
            ShootArrow arrow = arrowList[i];

            arrow.SetColor(new Color(arrow.GetColor().r, arrow.GetColor().g, arrow.GetColor().b, _v));
        }
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
                SuperSequenceControl.Start(battleControl.Summon, _index, (BattleSummonVO)vo);

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
            }
            else if (vo is BattleRushOverVO)
            {
            }
            else if (vo is BattleRecoverVO)
            {
                DoRecover(_index);

                yield return null;
            }
            else if (vo is BattleRoundStartVO)
            {
            }
            else if (vo is BattleRoundOverVO)
            {
            }
            else if (vo is BattleTriggerAuraVO)
            {
                SuperSequenceControl.Start(battleControl.TriggerAura, _index, (BattleTriggerAuraVO)vo);

                yield return null;
            }
            else if (vo is BattleStartVO)
            {
            }
            else if (vo is BattleRefreshVO)
            {
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

        SuperSequenceControl.Start(battleControl.ResetCamera, _index);

        yield return null;

        //RefreshData();

        SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_ROUND_OVER);

        Action dele = delegate ()
        {
            RoundOver((Battle.BattleResult)_step.Current);
        };

        ExitBattle(dele);
    }

    private void RoundOver(Battle.BattleResult _battleResult)
    {
        if (_battleResult != Battle.BattleResult.NOT_OVER)
        {
            BattleOver(_battleResult);
        }
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

    private void DoRecover(int _index)
    {
        RefreshHeroState(false);

        SuperSequenceControl.To(0, 1, 0.5f, RefresFearHero, _index);
    }

    private void RefresFearHero(float _v)
    {
        IEnumerator<HeroBattle> enumerator = heroDic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            HeroBattle hero = enumerator.Current;

            if (!hero.canAction)
            {
                hero.SetColorFix(PublicTools.FloatLerp(1, hero.colorFix, _v));
            }
        }
    }

    public void RefreshHeroState(bool _checkCanAction)
    {
        IEnumerator<HeroBattle> enumerator = heroDic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            enumerator.Current.Refresh(_checkCanAction);
        }
    }

    private void ScaleChange(int _index, float _scrollValue, Vector2 _pos)
    {
        if (!canAction)
        {
            return;
        }

        float scale;

        if (_scrollValue > 1)
        {
            scale = battleContainer.localScale.x * fixStep;
        }
        else
        {
            scale = battleContainer.localScale.x * (1 / fixStep);
        }

        scale = Mathf.Clamp(scale, minMapScale, maxMapScale);

        Vector3 v = mainCamera.ScreenToWorldPoint(_pos);

        SetBattleContainerScale(scale, v);

        FixBounds();
    }

    public void SetBattleContainerScale(float _scale, Vector2 _worldPos)
    {
        Vector3 v2 = battleContainer.InverseTransformPoint(_worldPos);

        battleContainer.localPosition = new Vector3(battleContainer.localPosition.x + v2.x, battleContainer.localPosition.y + v2.y, battleContainer.localPosition.z);

        battleContainer.localScale = new Vector3(_scale, _scale, 1);

        Vector3 v3 = battleContainer.TransformPoint(v2);

        battleContainer.localPosition = new Vector3(battleContainer.localPosition.x - v3.x + _worldPos.x, battleContainer.localPosition.y - v3.y + _worldPos.y, battleContainer.localPosition.z);
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
            MoveBattleContainer(lastPos, nowPos);
        }

        lastPos = nowPos;
    }

    private void MoveBattleContainer(Vector2 _lastPos, Vector2 _nowPos)
    {
        Bounds tmpBounds = bounds;

        tmpBounds.Expand(boundFix);

        tmpBounds.extents = new Vector3(tmpBounds.extents.x * battleContainer.transform.localScale.x, tmpBounds.extents.y * battleContainer.transform.localScale.y, tmpBounds.extents.z * battleContainer.transform.localScale.z);

        float scale = 1;

        if (tmpBounds.extents.x < viewport.extents.x)
        {
            if (battleContainer.transform.position.x < viewport.center.x == _nowPos.x < _lastPos.x)
            {
                float dis = Mathf.Abs(battleContainer.transform.position.x - viewport.center.x);

                scale = GetBattleContainerDragScale(dis);
            }
        }
        else if (battleContainer.localPosition.x - tmpBounds.extents.x > viewport.min.x)
        {
            if (_nowPos.x > _lastPos.x)
            {
                float dis = battleContainer.localPosition.x - tmpBounds.extents.x - viewport.min.x;

                scale = GetBattleContainerDragScale(dis);
            }
        }
        else if (battleContainer.localPosition.x + tmpBounds.extents.x < viewport.max.x)
        {
            if (_nowPos.x < _lastPos.x)
            {
                float dis = viewport.max.x - battleContainer.localPosition.x - tmpBounds.extents.x;

                scale = GetBattleContainerDragScale(dis);
            }
        }

        battleContainer.localPosition = new Vector3(battleContainer.localPosition.x + scale * (_nowPos.x - _lastPos.x), battleContainer.localPosition.y, battleContainer.localPosition.z);

        scale = 1;

        if (tmpBounds.extents.y < viewport.extents.y)
        {
            if (battleContainer.transform.position.y < viewport.center.y == _nowPos.y < _lastPos.y)
            {
                float dis = Mathf.Abs(battleContainer.transform.position.y - viewport.center.y);

                scale = GetBattleContainerDragScale(dis);
            }
        }
        else if (battleContainer.localPosition.y - tmpBounds.extents.y > viewport.min.y)
        {
            if (_nowPos.y > _lastPos.y)
            {
                float dis = battleContainer.localPosition.y - tmpBounds.extents.y - viewport.min.y;

                scale = GetBattleContainerDragScale(dis);
            }
        }
        else if (battleContainer.localPosition.y + tmpBounds.extents.y < viewport.max.y)
        {
            if (_nowPos.y < _lastPos.y)
            {
                float dis = viewport.max.y - battleContainer.localPosition.y - tmpBounds.extents.y;

                scale = GetBattleContainerDragScale(dis);
            }
        }

        battleContainer.localPosition = new Vector3(battleContainer.localPosition.x, battleContainer.localPosition.y + scale * (_nowPos.y - _lastPos.y), battleContainer.localPosition.z);
    }

    private void ResetBattleContainer()
    {
        Bounds tmpBounds = bounds;

        tmpBounds.Expand(boundFix);

        tmpBounds.extents = new Vector3(tmpBounds.extents.x * battleContainer.transform.localScale.x, tmpBounds.extents.y * battleContainer.transform.localScale.y, tmpBounds.extents.z * battleContainer.transform.localScale.z);

        if (tmpBounds.extents.x < viewport.extents.x)
        {
            if (battleContainer.transform.position.x != viewport.center.x)
            {
                battleContainer.transform.position = new Vector3(PublicTools.FloatLerp(battleContainer.transform.position.x, viewport.center.x, mapDragRevertFix), battleContainer.transform.position.y, battleContainer.transform.position.z);
            }
        }
        else if (battleContainer.localPosition.x - tmpBounds.extents.x > viewport.min.x)
        {
            float dis = battleContainer.localPosition.x - tmpBounds.extents.x - viewport.min.x;

            battleContainer.transform.position = new Vector3(battleContainer.transform.position.x - dis * mapDragRevertFix, battleContainer.transform.position.y, battleContainer.transform.position.z);
        }
        else if (battleContainer.localPosition.x + tmpBounds.extents.x < viewport.max.x)
        {
            float dis = viewport.max.x - battleContainer.localPosition.x - tmpBounds.extents.x;

            battleContainer.transform.position = new Vector3(battleContainer.transform.position.x + dis * mapDragRevertFix, battleContainer.transform.position.y, battleContainer.transform.position.z);
        }

        if (tmpBounds.extents.y < viewport.extents.y)
        {
            if (battleContainer.transform.position.y != viewport.center.y)
            {
                battleContainer.transform.position = new Vector3(battleContainer.transform.position.x, PublicTools.FloatLerp(battleContainer.transform.position.y, viewport.center.y, mapDragRevertFix), battleContainer.transform.position.z);
            }
        }
        else if (battleContainer.localPosition.y - tmpBounds.extents.y > viewport.min.y)
        {
            float dis = battleContainer.localPosition.y - tmpBounds.extents.y - viewport.min.y;

            battleContainer.transform.position = new Vector3(battleContainer.transform.position.x, battleContainer.transform.position.y - dis * mapDragRevertFix, battleContainer.transform.position.z);
        }
        else if (battleContainer.localPosition.y + tmpBounds.extents.y < viewport.max.y)
        {
            float dis = viewport.max.y - battleContainer.localPosition.y - tmpBounds.extents.y;

            battleContainer.transform.position = new Vector3(battleContainer.transform.position.x, battleContainer.transform.position.y + dis * mapDragRevertFix, battleContainer.transform.position.z);
        }
    }

    private float GetBattleContainerDragScale(float _v)
    {
        float result = 1 - _v / mapDragFix;

        if (result < 0)
        {
            result = 0;
        }

        return result;
    }

    public void GetMouseUp()
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
        if (!canAction)
        {
            return;
        }

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

            if (_hero.sds.shootSkills.Length > 0)
            {
                list = BattlePublicTools.GetCanThrowHeroPos(battle, _hero.hero);

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
    }

    public void ClickHeroCardShowMapUnitIcon(HeroCard _hero)
    {
        if (!canAction)
        {
            return;
        }

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
