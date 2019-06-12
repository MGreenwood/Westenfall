using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public enum AbilityType { Magic, Physical }

    public string abilityName;
    public float cooldown;
    protected GameObject owner;

    [SerializeField] protected float range;
    [SerializeField] protected int cost;
    [SerializeField] protected float castTime;
    [SerializeField] protected int damage;

    [SerializeField] protected PlayerClass.ClassType Class;
    [SerializeField] protected string tooltipDescription;
    [SerializeField] protected string tooltipFlavorText;
    [SerializeField] protected AbilityType _abilityType;

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
}
