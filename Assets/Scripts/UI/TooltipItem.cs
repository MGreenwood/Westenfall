using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TooltipItem : MonoBehaviour
{
    Item _item;

    [SerializeField]
    TextMeshProUGUI ItemName, ItemType, ItemSlot, ItemStats;

    public void SetItem(Item item)
    {
        _item = item;
        UpdateInfo();
    }

    void UpdateInfo()
    {
        if(_item is Weapon)
        {
            Weapon weap = _item as Weapon;
            ItemName.text = weap.GetItemName();
            ItemName.color = LookupDictionary.instance.colors[Convert.ToInt32(weap.GetRarity())];
            ItemType.text = weap.GetWeaponType().ToString();
            ItemSlot.text = weap.GetEquipmentSlot().ToString();

            ItemStats.text = "";
            foreach(Weapon.Stat stat in weap.GetStats())
            {
                if(stat._value > 0f)
                {
                    string val = stat._flatValue ? "" : "%";
                    ItemStats.text += $"{stat._stat}: {stat._value} {val}\n";
                }
            }
        }
        else if(_item is Armor)
        {
            Armor armor = _item as Armor;
            ItemName.text = armor.GetItemName();
            ItemName.color = LookupDictionary.instance.colors[Convert.ToInt32(armor.GetRarity())];
            ItemType.text = armor.GetItemType().ToString(); 
            ItemSlot.text = armor.GetEquipmentSlot().ToString();

            ItemStats.text = "";
            foreach (Armor.Stat stat in armor.GetStats())
            {
                if (stat._value > 0f)
                {
                    ItemStats.text += $"{stat._stat}: {stat._value}\n";
                }
            }
        }
    }

}
