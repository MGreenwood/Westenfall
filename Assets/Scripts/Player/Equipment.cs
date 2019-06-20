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


    Transform _slot;
    Player _player;

    private void Start()
    {
        _player = GetComponent<Player>();
    }

    public InventoryEquipmentSlot GetSlotContents(int index)
    {
        if (index >= 0 && index < _equipmentSlots.Length)
            return _equipmentSlots[index];
        else return null;
    }

    public void Equip(InventoryItemObject item, ref Attributes playerAttributes)
    {
        Item.EquipmentSlot eSlot = item.GetItem().GetEquipmentSlot();
        int slot = Convert.ToInt32(eSlot);

        Unequip(eSlot, ref playerAttributes);

        // add the new stats
        if (item.GetItem()?.GetItemType() == Item.ItemType.Weapon)
        {
            foreach (Weapon.Stat stat in (item.GetItem() as Weapon).GetStats())
            {
                if (stat._isBasicStat)
                {
                    string enumString = stat._stat.ToString();
                    Attributes.StatTypes statType = (Attributes.StatTypes)Enum.Parse(typeof(Attributes.StatTypes), enumString);
                    playerAttributes.IncrementStat(new Attributes.Stat(statType, (int)stat._value));
                }
                else
                {
                    Weapon.Stat weapStat = playerAttributes.GetWeaponStats().Find(x => x._stat == stat._stat);
                    weapStat += stat;
                    playerAttributes.SetWeaponStat(weapStat);
                }
            }
        }
        else if (item.GetItem()?.GetItemType() == Item.ItemType.Armor)
        {
            foreach (Armor.Stat stat in (item.GetItem() as Armor).GetStats())
            {
                if (stat._isBasicStat)
                {
                    string enumString = stat._stat.ToString();
                    Attributes.StatTypes statType = (Attributes.StatTypes)Enum.Parse(typeof(Attributes.StatTypes), enumString);
                    playerAttributes.IncrementStat(new Attributes.Stat(statType, (int)stat._value));
                }
                else
                {
                    Armor.Stat armorStat = playerAttributes.GetArmorStats().Find(x => x._stat == stat._stat);
                    armorStat += stat;
                    playerAttributes.SetArmorStat(armorStat);
                }
            }
        }

        _equipmentSlots[slot].SetItem(item);
    }


    public void Unequip(Item.EquipmentSlot eSlot, ref Attributes playerAttributes)
    {
        // retrieve the slot that takes that type of item
        int slot = Convert.ToInt32(eSlot);

        // retrieve the old stats and remove them from the attributes
        if (_equipmentSlots[slot].GetItem()?.GetItemType() == Item.ItemType.Weapon)
        {
            foreach (Weapon.Stat stat in (_equipmentSlots[slot].GetItem() as Weapon).GetStats())
            {
                if (stat._isBasicStat)
                {
                    string enumString = stat._stat.ToString();
                    Attributes.StatTypes statType = (Attributes.StatTypes)Enum.Parse(typeof(Attributes.StatTypes), enumString);
                    playerAttributes.DecrementStat(new Attributes.Stat(statType, (int)stat._value));
                }
                else
                {
                    Weapon.Stat weapStat = playerAttributes.GetWeaponStats().Find(x => x._stat == stat._stat);
                    weapStat -= stat;
                    playerAttributes.SetWeaponStat(weapStat);
                }
            }

            _equipmentSlots[slot].SetItemNull();
        }
        else if (_equipmentSlots[slot].GetItem()?.GetItemType() == Item.ItemType.Armor)
        {
            foreach (Armor.Stat stat in (_equipmentSlots[slot].GetItem() as Armor).GetStats())
            {
                if (stat._isBasicStat)
                {
                    string enumString = stat._stat.ToString();
                    Attributes.StatTypes statType = (Attributes.StatTypes)Enum.Parse(typeof(Attributes.StatTypes), enumString);
                    playerAttributes.DecrementStat(new Attributes.Stat(statType, (int)stat._value));
                }
                else
                {
                    Armor.Stat armorStat = playerAttributes.GetArmorStats().Find(x => x._stat == stat._stat);
                    armorStat -= stat;
                    playerAttributes.SetArmorStat(armorStat);
                }
            }

            _equipmentSlots[slot].SetItemNull();
        } 

    }
}
