using System.IO;
public class GuideSDS_c {
    public static void Init(GuideSDS _csv, BinaryReader _br){
        _csv.over = _br.ReadBoolean();
        int lengtheventResultArr = _br.ReadInt32();
        _csv.eventResultArr = new bool[lengtheventResultArr];
        for(int i = 0 ; i < lengtheventResultArr ; i++){
            _csv.eventResultArr[i] = _br.ReadBoolean();
        }
        _csv.ID = _br.ReadInt32();
        int lengtheventNameArr = _br.ReadInt32();
        _csv.eventNameArr = new string[lengtheventNameArr];
        for(int i = 0 ; i < lengtheventNameArr ; i++){
            _csv.eventNameArr[i] = _br.ReadString();
        }
        int lengthgameObjectNameArr = _br.ReadInt32();
        _csv.gameObjectNameArr = new string[lengthgameObjectNameArr];
        for(int i = 0 ; i < lengthgameObjectNameArr ; i++){
            _csv.gameObjectNameArr[i] = _br.ReadString();
        }
    }
}
