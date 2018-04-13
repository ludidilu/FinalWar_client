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
        int lengthauras = _br.ReadInt32();
        _csv.auras = new int[lengthauras];
        for(int i = 0 ; i < lengthauras ; i++){
            _csv.auras[i] = _br.ReadInt32();
        }
        int lengtheffects = _br.ReadInt32();
        _csv.effects = new int[lengtheffects];
        for(int i = 0 ; i < lengtheffects ; i++){
            _csv.effects[i] = _br.ReadInt32();
        }
        int lengthfeatures = _br.ReadInt32();
        _csv.features = new int[lengthfeatures];
        for(int i = 0 ; i < lengthfeatures ; i++){
            _csv.features[i] = _br.ReadInt32();
        }
        int lengthshootSkills = _br.ReadInt32();
        _csv.shootSkills = new int[lengthshootSkills];
        for(int i = 0 ; i < lengthshootSkills ; i++){
            _csv.shootSkills[i] = _br.ReadInt32();
        }
        int lengthsupportSkills = _br.ReadInt32();
        _csv.supportSkills = new int[lengthsupportSkills];
        for(int i = 0 ; i < lengthsupportSkills ; i++){
            _csv.supportSkills[i] = _br.ReadInt32();
        }
        _csv.icon = _br.ReadString();
        _csv.name = _br.ReadString();
    }
}
