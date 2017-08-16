using System.IO;
public class MapSDS_c {
    public static void Init(MapSDS _csv, BinaryReader _br){
        _csv.ID = _br.ReadInt32();
        int lengthheroID = _br.ReadInt32();
        _csv.heroID = new int[lengthheroID];
        for(int i = 0 ; i < lengthheroID ; i++){
            _csv.heroID[i] = _br.ReadInt32();
        }
        int lengthheroPos = _br.ReadInt32();
        _csv.heroPos = new int[lengthheroPos];
        for(int i = 0 ; i < lengthheroPos ; i++){
            _csv.heroPos[i] = _br.ReadInt32();
        }
        _csv.name = _br.ReadString();
    }
}
