using System;
using System.Collections.Generic;

[System.Serializable]
public class PlayerAttributes
{
    public enum StatTypes { Strength, Dexterity, Intellect, Stamina, Magic, NONE }

    public struct Stat
    {
        public StatTypes statType;
    }

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


