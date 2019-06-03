using UnityEngine;
using UnityEditor;

public class Potion : Item
{
    public enum Effect { Heal, Mana }
    [SerializeField]
    public delegate void OnActivation();
    OnActivation _onActivation;

    private Effect _effect;
    private bool _stackable;
    private int _maxStack;

    public void Activate()
    {
        _onActivation?.Invoke();
    }

    public void SubscribeOnActivation(OnActivation onActivation)
    {
        _onActivation += onActivation;
    }
}
