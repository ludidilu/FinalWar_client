using System.IO;
public class EffectSDS_c {
    public static void Init(EffectSDS _csv, BinaryReader _br){
        _csv.effect = _br.ReadInt32();
        _csv.effectPriority = _br.ReadInt32();
        _csv.ID = _br.ReadInt32();
        int lengthdata = _br.ReadInt32();
        _csv.data = new int[lengthdata];
        for(int i = 0 ; i < lengthdata ; i++){
            _csv.data[i] = _br.ReadInt32();
        }
    }
}
