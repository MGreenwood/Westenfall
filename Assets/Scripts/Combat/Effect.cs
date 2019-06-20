using System;

public class Effect{

    public enum EffectType { Basic, Crit, Burn, Poison, Bleed, Slow, Stun, Root, Heal};

    [Serializable]
    public struct AbilityEffect
    {
        public EffectType effectType;
        public int numTicks;
        public int value;

        public AbilityEffect(EffectType eType, int numTick, int val)
        {
            effectType = eType;
            numTicks = numTick;
            value = val;
        }
    }
}
