﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public enum AbilityType { Magic, Melee, Ranged }

    //                         left c   right c    qer    space 2
    public enum AbilitySlot { Primary, Secondary, Power, Movement } 

    public string abilityName;
    public float cooldown;
    protected GameObject owner;

    [SerializeField] protected PlayerClass.ClassType _classRequirement;
    [SerializeField] protected float range;
    [SerializeField] protected int cost;
    [SerializeField] protected float castTime;
    [SerializeField] protected int damage;

    [SerializeField] protected string tooltipDescription;
    [SerializeField] protected string tooltipFlavorText;
    [SerializeField] protected Sprite _abilityIcon;
    [SerializeField] protected AbilityType _abilityType;
    [SerializeField] protected AbilitySlot _abilitySlot;

    [SerializeField] protected bool _causesKnockback;
    [SerializeField, Tooltip("1, 2, or 3 describing the severity of the knockback")] protected int _knockbackPower; // 1, 2, or 3
    [SerializeField, Tooltip("If true, the ability will immediately use mana and is not cancelable")] protected bool animationLocked = false;

    public Ability() { }

    public float CastTime { get { return castTime;}}

    public virtual bool Cast() { return false; }

    public virtual void SetOwner(GameObject owner_)
    {
        owner = owner_;
    }

    public float GetRange() => range;
    public int GetCost() => cost; 

    public GameObject GetOwner() => owner;
    public Sprite GetIcon() => _abilityIcon;
    public string GetDescription() => tooltipDescription;
}
