using UnityEngine;
using superList;
using UnityEngine.UI;

public class BattleChooseCell : SuperListCell
{
    [SerializeField]
    private Text mapName;

    [SerializeField]
    private CanvasGroup cg;

    public override bool SetData(object _data)
    {
        BattleSDS battleSDS = _data as BattleSDS;

        mapName.text = battleSDS.name;

        cg.alpha = battleSDS.guideID == 0 ? 0 : 1;

        return base.SetData(_data);
    }
}
