using System.IO;
public class AuraSDS_c {
    public static void Init(AuraSDS _csv, BinaryReader _br){
        _csv.conditionCompare = _br.ReadInt32();
        _csv.effectTarget = _br.ReadInt32();
        _csv.effectTargetNum = _br.ReadInt32();
        _csv.effectType = _br.ReadInt32();
        _csv.ID = _br.ReadInt32();
        _csv.priority = _br.ReadInt32();
        _csv.triggerTarget = _br.ReadInt32();
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
        int lengtheffectData = _br.ReadInt32();
        _csv.effectData = new int[lengtheffectData];
        for(int i = 0 ; i < lengtheffectData ; i++){
            _csv.effectData[i] = _br.ReadInt32();
        }
        _csv.desc = _br.ReadString();
        _csv.eventName = _br.ReadString();
        _csv.hud = _br.ReadString();
        int lengthremoveEventNames = _br.ReadInt32();
        _csv.removeEventNames = new string[lengthremoveEventNames];
        for(int i = 0 ; i < lengthremoveEventNames ; i++){
            _csv.removeEventNames[i] = _br.ReadString();
        }
    }
}
