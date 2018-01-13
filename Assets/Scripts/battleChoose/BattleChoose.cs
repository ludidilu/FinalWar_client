using System.Collections.Generic;
using UnityEngine;
using superList;
using System;
using tuple;

public class BattleChoose : UIWindow
{
    [SerializeField]
    private SuperList superList;

    private Action<BattleSDS> chooseCallBack;

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

    public override void OnEnter()
    {
        chooseCallBack = ((Tuple<Action<BattleSDS>>)data).first;

        superList.DisplayIndex(0);
    }

    private void Click(object _battleSDS)
    {
        chooseCallBack(_battleSDS as BattleSDS);

        UIManager.Instance.Hide(uid);
    }

    public void Quit()
    {
        UIManager.Instance.Hide(uid);
    }
}
