using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Equipment : MonoBehaviour
{
    [SerializeField]
    InventoryEquipmentSlot[] _equipmentSlots;

    [SerializeField]
    Attributes _attributes;

    Transform _slot;
    Player _player;

    private void Start()
    {
        _player = GetComponent<Player>();
        _attributes = Instantiate(Resources.Load("ScriptableObjects/Attributes/BaselineAttributes")) as Attributes;
    }

    public InventoryEquipmentSlot GetSlotContents(int index)
    {
        if (index >= 0 && index < _equipmentSlots.Length)
            return _equipmentSlots[index];
        else return null;
    }

    void RecalculateStats()
    {
        // reset the attributes
        _attributes = Instantiate(Resources.Load("ScriptableObjects/Attributes/BaselineAttributes")) as Attributes;

        foreach (InventoryEquipmentSlot slot in _equipmentSlots)
        {
            if(slot.GetItem() is Weapon)
            {
                foreach(Weapon.Stat stat in slot.GetWeaponStats()) // for every weapon stat
                {
                    // find the matching weapon attribute
                    try
                    {
                        _attributes.IncrementWeaponStat(stat);
                        //_attributes.GetWeaponStats().Find(x => x._stat == stat._stat)._value += stat._value;
                    }
                    catch { }
                        
                    
                }
            }
            else if (slot.GetItem() is Armor)
            {
                foreach (Armor.Stat stat in slot.GetArmorStats())
                {
                    // find the matching armor attribute
                    try
                    {
                        _attributes.GetArmorStats().Find(x => x._stat == stat._stat)._value += stat._value;
                    }
                    catch { }
                }
            }
        }

        Player.instance.CalculateStats();
    }

    public Attributes.Stat[] GetBasicStats()
    {
        return _attributes.GetStats();
    }

    public void Equip(InventoryItemObject item)
    {
        int slot = Convert.ToInt32(item.GetItem().GetEquipmentSlot());
        _equipmentSlots[slot].SetItem(item);
        RecalculateStats();
    }

    public void Unequip(Item.EquipmentSlot slot)
    {
        switch(slot)
        {
            case Item.EquipmentSlot.Head:
                _equipmentSlots[0].SetItemNull();
                break;
            case Item.EquipmentSlot.Hands:
                _equipmentSlots[1].SetItemNull();
                break;
            case Item.EquipmentSlot.Weapon:
                _equipmentSlots[2].SetItemNull();
                break;
            case Item.EquipmentSlot.Chest:
                _equipmentSlots[3].SetItemNull();
                break;
            case Item.EquipmentSlot.Shield:
                _equipmentSlots[4].SetItemNull();
                break;
            case Item.EquipmentSlot.Feet:
                _equipmentSlots[5].SetItemNull();
                break;
        }
    }
}
