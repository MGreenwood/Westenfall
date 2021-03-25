using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class InventoryEquipmentSlot : MonoBehaviour, IEquippableArea
{
    [SerializeField]
    Item.ItemType _itemType;
    [SerializeField]
    Weapon.SlotType _weaponSlot;
    [SerializeField]
    Item.EquipmentSlot _armorSlot;

    InventoryItemObject _inventoryItem;

    [SerializeField]
    Item _item;


    public InventoryEquipmentSlot GetEquipmentSlot()
    {
        return this;
    }

    public void SetItem(InventoryItemObject item)
    {
        _item = item.GetItem();
        item.transform.SetParent(transform.parent);

        Image image = GetComponent<Image>();

        item.SetScale(image.rectTransform.localScale, image.rectTransform.sizeDelta);
        item.transform.position = image.transform.position;

        _inventoryItem = item;
    }

    public void SetItemNull()
    {
        _item = null;
    }

    public Item GetItem() => _item;
    public InventoryItemObject GetInventoryItemObject() => _inventoryItem;

    public Weapon.Stat[] GetWeaponStats()
    {
        return ((Weapon)_item).GetStats().ToArray();
    }

    public Armor.Stat[] GetArmorStats()
    {
        return ((Armor)_item).GetStats().ToArray();
    }
}
