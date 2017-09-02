using System.IO;
public class HeroTypeSDS_c {
    public static void Init(HeroTypeSDS _csv, BinaryReader _br){
        _csv.attackSpeed = _br.ReadInt32();
        _csv.attackTimes = _br.ReadInt32();
        _csv.defenseSpeed = _br.ReadInt32();
        _csv.fearType = _br.ReadInt32();
        _csv.ID = _br.ReadInt32();
        _csv.supportSpeed = _br.ReadInt32();
        _csv.supportSpeedBonus = _br.ReadInt32();
        _csv.thread = _br.ReadInt32();
        _csv.name = _br.ReadString();
    }
}
