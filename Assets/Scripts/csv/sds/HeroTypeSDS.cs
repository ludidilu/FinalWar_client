﻿using FinalWar;

public partial class HeroTypeSDS : CsvBase, IHeroTypeSDS
{
    public bool canAttack;

    public int attackSpeed;
    public int defenseSpeed;
    public int supportSpeed;

    public int attackTimes;
    public int thread;
    public int fearValue;

    public int fearType;

    public int fearAttackWeight;
    public int fearDefenseWeight;

    public int GetID()
    {
        return ID;
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }

    public int GetAttackSpeed()
    {
        return attackSpeed;
    }

    public int GetDefenseSpeed()
    {
        return defenseSpeed;
    }

    public int GetSupportSpeed()
    {
        return supportSpeed;
    }

    public int GetAttackTimes()
    {
        return attackTimes;
    }

    public int GetThread()
    {
        return thread;
    }

    public int GetFearValue()
    {
        return fearValue;
    }

    public FearType GetFearType()
    {
        return (FearType)fearType;
    }

    public int GetFearAttackWeight()
    {
        return fearAttackWeight;
    }

    public int GetFearDefenseWeight()
    {
        return fearDefenseWeight;
    }
}
