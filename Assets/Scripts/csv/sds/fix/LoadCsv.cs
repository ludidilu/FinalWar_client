using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
public class LoadCsv {
    public static Dictionary<Type,IDictionary> Init(byte[] _bytes) {
        MemoryStream ms = new MemoryStream(_bytes);
        BinaryReader br = new BinaryReader(ms);
        Dictionary<Type,IDictionary> dic = new Dictionary<Type,IDictionary>();
        Dictionary<int,AuraSDS> AuraSDSDic = new Dictionary<int,AuraSDS>();
        int lengthAuraSDS = br.ReadInt32();
        for(int i = 0 ; i < lengthAuraSDS ; i++){
            AuraSDS unit = new AuraSDS();
            AuraSDS_c.Init(unit,br);
            unit.Fix();
            AuraSDSDic.Add(unit.ID,unit);
        }
        dic.Add(typeof(AuraSDS),AuraSDSDic);
        Dictionary<int,EffectSDS> EffectSDSDic = new Dictionary<int,EffectSDS>();
        int lengthEffectSDS = br.ReadInt32();
        for(int i = 0 ; i < lengthEffectSDS ; i++){
            EffectSDS unit = new EffectSDS();
            EffectSDS_c.Init(unit,br);
            unit.Fix();
            EffectSDSDic.Add(unit.ID,unit);
        }
        dic.Add(typeof(EffectSDS),EffectSDSDic);
        Dictionary<int,HeroSDS> HeroSDSDic = new Dictionary<int,HeroSDS>();
        int lengthHeroSDS = br.ReadInt32();
        for(int i = 0 ; i < lengthHeroSDS ; i++){
            HeroSDS unit = new HeroSDS();
            HeroSDS_c.Init(unit,br);
            unit.Fix();
            HeroSDSDic.Add(unit.ID,unit);
        }
        dic.Add(typeof(HeroSDS),HeroSDSDic);
        Dictionary<int,HeroTypeSDS> HeroTypeSDSDic = new Dictionary<int,HeroTypeSDS>();
        int lengthHeroTypeSDS = br.ReadInt32();
        for(int i = 0 ; i < lengthHeroTypeSDS ; i++){
            HeroTypeSDS unit = new HeroTypeSDS();
            HeroTypeSDS_c.Init(unit,br);
            unit.Fix();
            HeroTypeSDSDic.Add(unit.ID,unit);
        }
        dic.Add(typeof(HeroTypeSDS),HeroTypeSDSDic);
        Dictionary<int,MapSDS> MapSDSDic = new Dictionary<int,MapSDS>();
        int lengthMapSDS = br.ReadInt32();
        for(int i = 0 ; i < lengthMapSDS ; i++){
            MapSDS unit = new MapSDS();
            MapSDS_c.Init(unit,br);
            unit.Fix();
            MapSDSDic.Add(unit.ID,unit);
        }
        dic.Add(typeof(MapSDS),MapSDSDic);
        Dictionary<int,SkillSDS> SkillSDSDic = new Dictionary<int,SkillSDS>();
        int lengthSkillSDS = br.ReadInt32();
        for(int i = 0 ; i < lengthSkillSDS ; i++){
            SkillSDS unit = new SkillSDS();
            SkillSDS_c.Init(unit,br);
            unit.Fix();
            SkillSDSDic.Add(unit.ID,unit);
        }
        dic.Add(typeof(SkillSDS),SkillSDSDic);
        br.Close();
        ms.Close();
        ms.Dispose();
        return dic;
    }
}