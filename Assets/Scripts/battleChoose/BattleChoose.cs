using System.Collections.Generic;
using UnityEngine;
using superList;
using System;

public class BattleChoose : UIBase
{
    [SerializeField]
    private SuperList superList;

    private Action<BattleSDS> chooseCallBack;

    public override bool IsFullScreen()
    {
        return false;
    }

    public override void Init()
    {
        base.Init();

        superList.CellClickHandle = Click;

        List<BattleSDS> list = new List<BattleSDS>();

        Dictionary<int, BattleSDS> dic = StaticData.GetDic<BattleSDS>();

        IEnumerator<BattleSDS> enumerator = dic.Values.GetEnumerator();

        while (enumerator.MoveNext())
        {
            if (enumerator.Current.isPve)
            {
                list.Add(enumerator.Current);
            }
        }

        superList.SetData(list);
    }

    public override void OnEnter<U>(U _data)
    {
        chooseCallBack = _data as Action<BattleSDS>;
    }

    private void Click(object _battleSDS)
    {
        chooseCallBack(_battleSDS as BattleSDS);
    }

    public void Quit()
    {
        UIManager.Instance.Hide(this);
    }
}
