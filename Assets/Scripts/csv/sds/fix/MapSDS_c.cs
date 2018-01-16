using System.IO;
public class MapSDS_c {
    public static void Init(MapSDS _csv, BinaryReader _br){
        _csv.ID = _br.ReadInt32();
        _csv.name = _br.ReadString();
        int lengthfearAction = _br.ReadInt32();
        _csv.fearAction = new string[lengthfearAction];
        for(int i = 0 ; i < lengthfearAction ; i++){
            _csv.fearAction[i] = _br.ReadString();
        }
        int lengthhero = _br.ReadInt32();
        _csv.hero = new string[lengthhero];
        for(int i = 0 ; i < lengthhero ; i++){
            _csv.hero[i] = _br.ReadString();
        }
    }
}
