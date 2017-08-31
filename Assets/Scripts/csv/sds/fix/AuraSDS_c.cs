using System.IO;
public class AuraSDS_c {
    public static void Init(AuraSDS _csv, BinaryReader _br){
        _csv.auraCondition = _br.ReadInt32();
        _csv.auraConditionData = _br.ReadInt32();
        _csv.auraTarget = _br.ReadInt32();
        _csv.auraTrigger = _br.ReadInt32();
        _csv.auraType = _br.ReadInt32();
        _csv.ID = _br.ReadInt32();
        int lengthauraConditionTarget = _br.ReadInt32();
        _csv.auraConditionTarget = new int[lengthauraConditionTarget];
        for(int i = 0 ; i < lengthauraConditionTarget ; i++){
            _csv.auraConditionTarget[i] = _br.ReadInt32();
        }
        int lengthauraData = _br.ReadInt32();
        _csv.auraData = new int[lengthauraData];
        for(int i = 0 ; i < lengthauraData ; i++){
            _csv.auraData[i] = _br.ReadInt32();
        }
        _csv.comment = _br.ReadString();
        _csv.eventName = _br.ReadString();
    }
}
