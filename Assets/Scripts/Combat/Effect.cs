using System;

public class Effect{

    public enum EffectType { Basic, Crit, Burn, Poison, Bleed, Slow, Stun, Root, Heal};

    [Serializable]
    public struct AbilityEffect
    {
        public EffectType effectType;
        public float duration;
        public int value;

        public AbilityEffect(EffectType eType, float dur, int val)
        {
            effectType = eType;
            duration = dur;
            value = val;
        }
    }
}
