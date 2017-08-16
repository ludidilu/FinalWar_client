using System.IO;
public class HeroSDS_c {
    public static void Init(HeroSDS _csv, BinaryReader _br){
        _csv.attack = _br.ReadInt32();
        _csv.cost = _br.ReadInt32();
        _csv.heroType = _br.ReadInt32();
        _csv.hp = _br.ReadInt32();
        _csv.ID = _br.ReadInt32();
        _csv.power = _br.ReadInt32();
        _csv.shield = _br.ReadInt32();
        _csv.skill = _br.ReadInt32();
        int lengthauras = _br.ReadInt32();
        _csv.auras = new int[lengthauras];
        for(int i = 0 ; i < lengthauras ; i++){
            _csv.auras[i] = _br.ReadInt32();
        }
        _csv.name = _br.ReadString();
    }
}
