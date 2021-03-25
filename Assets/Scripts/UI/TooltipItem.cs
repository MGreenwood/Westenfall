using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class TooltipItem : MonoBehaviour
{
    Item _item;
    Ability _ability;

    public enum TooltipType { Gear, Potion, Ability }
    [SerializeField]
    TooltipType toolType;

    [SerializeField]
    GameObject textPrefab; 
    [SerializeField]
    TextMeshProUGUI ItemName, ItemType, ItemSlot, ItemStats, ItemStatRequirements;

    public void SetItem(Item item)
    {
        if(item == null)
        {
            return;
        }

        _item = item;
        UpdateInfo();
        toolType = TooltipType.Gear;
    }

    public void SetAbility(Ability ability)
    {
        _ability = ability;
        toolType = TooltipType.Ability;
        UpdateInfo();
    }

    public TooltipType GetTooltipType() => toolType;

    void UpdateGear()
    {
        if(_item is Weapon)
        {

            Weapon weap = _item as Weapon;
            ItemName.text = weap.GetFullItemName();
            ItemName.color = LookupDictionary.instance.colors[(int)weap.GetRarity()];

            ItemType.text = weap.GetWeaponType().ToString();

            ItemStats.text = "";

            if (weap.GetMaxDamage() != 0f)
                ItemStats.text += "Min: " + (int)weap.GetMinDamage() + "  Max: " + (int)weap.GetMaxDamage() + "\n\n";
            if (weap.GetMagicDamage() != 0f)
                ItemStats.text += "Magic Damage: " + (int)weap.GetMagicDamage() + "\n\n";

            foreach(Weapon.Stat stat in weap.GetStats())
            {
                if(stat._value > 0f)
                {
                    string val = stat._flatValue ? "" : "%";
                    ItemStats.text += $"{stat._stat}: {stat._value} {val}\n";
                }
            }
            foreach(Attributes.Stat stat in weap.GetStatRequirements())
            {
                ItemStatRequirements.text += $"{stat.statType}: {stat.value}\n";
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
            foreach(Attributes.Stat stat in armor.GetStatRequirements())
            {
                ItemStatRequirements.text += $"{stat.statType}: {stat.value}\n";
            }
        }

    }

    void UpdateAbility()
    {
        ItemName.text = _ability.abilityName;
        ItemStats.text = _ability.GetDescription();
    }

    void UpdateInfo()
    {
        if (toolType == TooltipType.Gear)
            UpdateGear();
        else if (toolType == TooltipType.Ability)
            UpdateAbility();

        ItemStats.ForceMeshUpdate();
        Vector2 newSize = new Vector2(ItemStats.textBounds.size.x, ItemStats.textBounds.size.y);
        ItemStats.rectTransform.sizeDelta = newSize * 1.2f; // make it a little bigger for safety
        ItemStats.transform.parent.GetComponent<RectTransform>().sizeDelta = newSize * 1.4f;
        ItemStats.transform.parent.localPosition = Vector3.zero;
        //ItemStats.transform.localPosition = Vector3.zero;
        GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x * 1.5f);
        GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y  + (GetComponent<Image>().rectTransform.sizeDelta.y * 0.3f));
        // so sorry
    }

}
