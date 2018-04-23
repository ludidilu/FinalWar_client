using System.IO;
public class BattleSDS_c {
    public static void Init(BattleSDS _csv, BinaryReader _br){
        _csv.isPve = _br.ReadBoolean();
        _csv.addCardsNum = _br.ReadInt32();
        _csv.addMoney = _br.ReadInt32();
        _csv.deckCardsNum = _br.ReadInt32();
        _csv.defaultHandCardsNum = _br.ReadInt32();
        _csv.defaultMoney = _br.ReadInt32();
        _csv.guideID = _br.ReadInt32();
        _csv.ID = _br.ReadInt32();
        _csv.mapID = _br.ReadInt32();
        _csv.maxRoundNum = _br.ReadInt32();
        int lengthmCards = _br.ReadInt32();
        _csv.mCards = new int[lengthmCards];
        for(int i = 0 ; i < lengthmCards ; i++){
            _csv.mCards[i] = _br.ReadInt32();
        }
        int lengthoCards = _br.ReadInt32();
        _csv.oCards = new int[lengthoCards];
        for(int i = 0 ; i < lengthoCards ; i++){
            _csv.oCards[i] = _br.ReadInt32();
        }
        _csv.name = _br.ReadString();
    }
}
