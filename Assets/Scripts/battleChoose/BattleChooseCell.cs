using UnityEngine;
using superList;
using UnityEngine.UI;

public class BattleChooseCell : SuperListCell
{
    [SerializeField]
    private Text mapID;

    [SerializeField]
    private Text round;

    public override bool SetData(object _data)
    {
        BattleSDS battleSDS = _data as BattleSDS;

        mapID.text = battleSDS.mapID.ToString();

        round.text = battleSDS.maxRoundNum.ToString();

        return base.SetData(_data);
    }
}
