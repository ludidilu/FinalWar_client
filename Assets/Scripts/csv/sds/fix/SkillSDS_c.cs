using System.IO;
public class SkillSDS_c {
    public static void Init(SkillSDS _csv, BinaryReader _br){
        _csv.isStop = _br.ReadBoolean();
        _csv.ID = _br.ReadInt32();
        int lengtheffects = _br.ReadInt32();
        _csv.effects = new int[lengtheffects];
        for(int i = 0 ; i < lengtheffects ; i++){
            _csv.effects[i] = _br.ReadInt32();
        }
        _csv.comment = _br.ReadString();
    }
}
