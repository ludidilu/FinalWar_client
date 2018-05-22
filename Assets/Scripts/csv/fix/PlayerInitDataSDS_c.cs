using System.IO;
public class PlayerInitDataSDS_c {
    public static void Init(PlayerInitDataSDS _csv, BinaryReader _br){
        _csv.addCardsNum = _br.ReadInt32();
        _csv.addMoney = _br.ReadInt32();
        _csv.deckCardsNum = _br.ReadInt32();
        _csv.defaultHandCardsNum = _br.ReadInt32();
        _csv.defaultMoney = _br.ReadInt32();
        _csv.ID = _br.ReadInt32();
    }
}
