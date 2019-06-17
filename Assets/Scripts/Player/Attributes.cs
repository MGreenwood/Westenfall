using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Attributes")]
public class Attributes : ScriptableObject
{
    public enum StatTypes { Strength, Dexterity, Spirit, Stamina, Magic, NONE }

    public const float Bonus_Mod = 0.2f;
    public const float StamToHP = 12.4f;
    public const float MagicToMP = 14.8f;

    [Serializable]
    public struct Stat
    {
        public StatTypes statType;
        public int value;
    }

    [SerializeField]
    List<Stat> _stats;

    bool _statsDirty = true;
     
    public Stat GetStat(StatTypes statType)
    {
        foreach(Stat s in _stats)
        {
            if(s.statType == statType)
            {
                if (_statsDirty)
                    CalculateStats();

                return s;
            }
        }

        return new Stat();
    }

    private void CalculateStats()
    {
        // use gear to determine stats
    }
}


