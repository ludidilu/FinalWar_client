using System.IO;
public class EffectSDS_c {
    public static void Init(EffectSDS _csv, BinaryReader _br){
        _csv.conditionCompare = _br.ReadInt32();
        _csv.effect = _br.ReadInt32();
        _csv.ID = _br.ReadInt32();
        _csv.priority = _br.ReadInt32();
        int lengthconditionData = _br.ReadInt32();
        _csv.conditionData = new int[lengthconditionData];
        for(int i = 0 ; i < lengthconditionData ; i++){
            _csv.conditionData[i] = _br.ReadInt32();
        }
        int lengthconditionType = _br.ReadInt32();
        _csv.conditionType = new int[lengthconditionType];
        for(int i = 0 ; i < lengthconditionType ; i++){
            _csv.conditionType[i] = _br.ReadInt32();
        }
        int lengthdata = _br.ReadInt32();
        _csv.data = new int[lengthdata];
        for(int i = 0 ; i < lengthdata ; i++){
            _csv.data[i] = _br.ReadInt32();
        }
        _csv.desc = _br.ReadString();
    }
}
