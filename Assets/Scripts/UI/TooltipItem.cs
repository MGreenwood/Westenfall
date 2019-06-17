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
        }
        else if(_item is Armor)
        {

        }
    }

}
