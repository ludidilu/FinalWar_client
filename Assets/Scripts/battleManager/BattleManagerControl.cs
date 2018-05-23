using superFunction;
using System;
using System.Reflection;
using UnityEngine;
using superTween;
using System.Text.RegularExpressions;

public partial class BattleManager : MonoBehaviour
{
    private enum DownType
    {
        NULL,
        BACKGROUND,
        MAPUNIT
    }

    [SerializeField]
    private float showHeroDetailHoldTime;

    private DownType isDown = DownType.NULL;

    private bool hasMove = false;

    private MapUnit downMapUnit;

    private bool nowChooseHeroCanAction = false;

    private bool isDoingHeroAction = false;

    private bool mouseHasExited = false;

    private Vector2 downPos;

    private Vector2 lastPos;

    private int showHeroDetailTweenID = -1;

    private HeroCard m_nowChooseCard;

    private HeroCard GetNowChooseCard()
    {
        return m_nowChooseCard;
    }

    private void SetNowChooseCard(HeroCard _value)
    {
        if (_value == null)
        {
            heroDetail.Hide();

            ClearMapUnitIcon();
        }
        else
        {
            SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_CHOOSE_CARD);

            //heroDetail.Show(_value);

            ClickHeroCardShowMapUnitIcon(_value);
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
            heroDetail.Hide();

            ClearMapUnitIcon();
        }
        else
        {
            SuperFunction.Instance.DispatchEvent(eventGo, BATTLE_CHOOSE_HERO);

            //heroDetail.Show(_value);

            ClickHeroBattleShowMapUnitIcon(_value);
        }

        m_nowChooseHero = _value;

        nowChooseHeroCanAction = _canAction;
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
            SuperTween.Instance.RemoveWithTag(BATTLE_TWEEN_TAG, false);
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            foreach (HeroBattle hero in heroDic.Values)
            {
                hero.ShowFearValue();
            }
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

    public void GetMouseUp()
    {
        RemoveShowHeroDetailTween();

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

    private void BackgroundMove()
    {
        Vector3 nowPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (!hasMove && Vector2.Distance(nowPos, downPos) > moveThreshold)
        {
            hasMove = true;

            RemoveShowHeroDetailTween();
        }

        if (!isDoingHeroAction && hasMove)
        {
            MoveBattleContainer(lastPos, nowPos);
        }

        lastPos = nowPos;
    }

    private void MapUnitDown(MapUnit _mapUnit)
    {
        CheckShowHeroDetail(_mapUnit.index);

        downPos = lastPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        isDown = DownType.MAPUNIT;

        downMapUnit = _mapUnit;

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
        RemoveShowHeroDetailTween();

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

        if (battle.GetSummonContainsKey(_mapUnit.index))
        {
            ClearNowChooseCard();

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
                    bool b = UnsummonHero(_mapUnit.index);

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
            ClearNowChooseCard();

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
            bool b = SummonHero(_mapUnit.index, GetNowChooseCard().cardUid);

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

            ClearNowChooseCard();
        }
        else
        {
            ClearNowChooseHero();

            ClearNowChooseCard();
        }
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

    private void CheckShowHeroDetail(int _mapUnitIndex)
    {
        HeroBattle hero = null;

        if (battle.GetSummonContainsKey(_mapUnitIndex))
        {
            hero = summonHeroDic[_mapUnitIndex];
        }
        else if (battle.GetHeroMapContainsKey(_mapUnitIndex))
        {
            hero = heroDic[_mapUnitIndex];
        }

        if (hero != null)
        {
            Action dele = delegate ()
            {
                showHeroDetailTweenID = -1;

                GetMouseUp();

                heroDetail.Show(hero);
            };

            showHeroDetailTweenID = DelayCall(showHeroDetailHoldTime, dele);
        }
    }

    private void RemoveShowHeroDetailTween()
    {
        if (showHeroDetailTweenID != -1)
        {
            SuperTween.Instance.Remove(showHeroDetailTweenID);

            showHeroDetailTweenID = -1;
        }
    }

    private void ClickDesc(int _index, int _id)
    {
        if (_id > 0)
        {
            DescSDS sds = StaticData.GetData<DescSDS>(_id);

            string result = FixDesc(sds.desc, GetStrFix);

            descPanel.Show(result);
        }
    }

    private static readonly Regex reg = new Regex("@.*?@");

    public static string FixDesc(string _str, Func<string, string> _fixFunc)
    {
        MatchEvaluator me = delegate (Match _match)
        {
            string idStr = _match.Value.Substring(1, _match.Value.Length - 2);

            return _fixFunc(idStr);
        };

        string strFix = reg.Replace(_str, me);

        return strFix;
    }

    private string GetStrFix(string _str)
    {
        string[] strArr = _str.Split('.');

        switch (strArr[0])
        {
            case "BattleConst":

                Type t = battle.GetType().Assembly.GetType("FinalWar.BattleConst");

                FieldInfo fi = t.GetField(strArr[1]);

                return fi.GetValue(null).ToString();

            case "battle":

                t = battle.GetType();

                PropertyInfo pi = t.GetProperty(strArr[1]);

                return pi.GetValue(battle, null).ToString();

            default:

                throw new Exception("error GetStrFix:" + _str);
        }
    }

}