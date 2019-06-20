using System;
using UnityEngine;

public abstract class Item : UnityEngine.ScriptableObject
{
    public enum ItemType  { Armor, Weapon, Potion }
    public enum Rarity { White, Green, Blue, Purple, Orange, Yellow, Red }
    public enum EquipmentSlot { Head, Hands, Weapon, Chest, Shield, Feet}

    public struct ItemSize
    {
        public int _width;
        public int _height;

        public ItemSize(int width, int height)
        {
            _width = width;
            _height = height;
        }
    }

    [SerializeField]
    protected string _itemName;
    [SerializeField]
    protected string _flavorText;
    [Space(15)]
    [SerializeField]
    protected Rarity _rarity;
    [SerializeField]
    protected Sprite _inventorySprite;
    [SerializeField]
    EquipmentSlot _equipmentSlot;

    [SerializeField]
    GameObject prefab;

    [SerializeField]
    protected ItemType _itemType;
    protected ItemSize _itemSize;

    [SerializeField]
    int _levelRequirement = 0;

    public virtual string GetItemName()
    {
        return _itemName;
    }

    public int GetLevelRequirement() => _levelRequirement;

    public virtual void init()
    {

    }

    public ItemSize GetItemSize()
    {
        return _itemSize;
    }

    public Sprite GetSprite()
    {
        return _inventorySprite;
    }

    public Rarity GetRarity() => _rarity;
    public EquipmentSlot GetEquipmentSlot() => _equipmentSlot;

    public ItemType GetItemType() => _itemType;
}
