using System;
using UnityEngine;

public abstract class Item : UnityEngine.ScriptableObject
{
    public enum ItemType  { Armor, Weapon, Potion, Reagent }
    public enum Rarity { Common, Uncommon, Rare, UltraRare, Epic, Mythic, Unique, Heirloom }
    public enum EquipmentSlot { Head, Hands, Weapon, Chest, Shield, Feet }
    public enum BasicStats { Strength, Dexterity, Spirit, Stamina, Magic }

    
    public class ItemSize
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
    protected int _tier;
    [SerializeField]
    protected Sprite _inventorySprite;

    [SerializeField]
    GameObject prefab;

    protected ItemType _itemType;
    [SerializeField]
    protected ItemSize _itemSize;

    string _prefix = "", _suffix = "";

    [SerializeField]
    int _levelRequirement = 0;

    public virtual string GetFullItemName()
    {
        return _prefix + " " + _itemName + " " + _suffix;
    }

    public virtual string SetPrefix(string p) => _prefix = p;
    public virtual string SetSuffix(string s) => _suffix = s;
    public virtual string SetName(string n) => _itemName = n;


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

    public ItemType GetItemType() => _itemType;
    public void SetRarity(Rarity r) => _rarity = r;
    public int GetTier() => _tier;
    public void SetTier(int tierIn) => _tier = tierIn;
    public void SetInventorySprite(Sprite sprite) => _inventorySprite = sprite;
}
