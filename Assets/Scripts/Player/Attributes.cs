using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Attributes")]
public class Attributes : ScriptableObject
{
    public enum StatTypes { Strength, Dexterity, Intellect, Stamina, Magic, NONE }

    [Serializable]
    public struct Stat
    {
        public StatTypes statType;
        public float value;
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


