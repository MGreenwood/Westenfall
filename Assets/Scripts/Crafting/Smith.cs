using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class Smith : CraftingBuilding
{
    [SerializeField]
    TMP_Dropdown dd_itemType, dd_itemSpecific, dd_itemTier;

    [SerializeField]
    TMP_Dropdown armorClass;

    [SerializeField]
    GameObject tooltipPrefab;
    [SerializeField]
    Transform tooltipParent;
    [SerializeField]
    RectTransform tooltipLocation;
    RectTransform rect; // the rect transform of the tooltip
    GameObject tooltip;

    [SerializeField]
    RarityUIBehavior rarityBehavior;

    public Item testItem;
    Item currentItem;
    ItemRoller itemRoller;

    [SerializeField]
    Canvas[] relevantCanvas;

    Inventory playerInventory;
    GameObject GroundItemPrefab;

    private void Start()
    {
        PopulateSpecific();
        armorClass.gameObject.SetActive(false); // hide the armor class dropdown until it is selected
        UpdatePreview();
        itemRoller = GetComponent<ItemRoller>();
        playerInventory = FindObjectOfType<Player>().GetInventory();
    }

    private void OnMouseDown()
    {
        foreach(Canvas c in relevantCanvas)
        {
            c.enabled = !c.enabled;
        }
    }

    public void PopulateSpecific()
    {
        List<string> options = new List<string>();
        switch (dd_itemType.value)
        {
            case 0: // Weapons
                dd_itemSpecific.ClearOptions();
                foreach(Weapon.WeaponType t in Enum.GetValues(typeof (Weapon.WeaponType)))
                {
                    options.Add(t.ToString());
                    armorClass.gameObject.SetActive(false);
                }

                dd_itemSpecific.AddOptions(options);
                break;
            case 1: // Armors
                dd_itemSpecific.ClearOptions();
                if (armorClass.GetComponent<TMP_Dropdown>().options.Count == 0)
                    PopulateArmorClass();

                foreach(Armor.EquipmentSlot t in Enum.GetValues(typeof (Armor.EquipmentSlot)))
                {
                    if (t.ToString() == "Weapon")
                        continue;
                    options.Add(t.ToString());
                    armorClass.gameObject.SetActive(true);
                }

                dd_itemSpecific.AddOptions(options);
                break;
        }
    }

    public void Craft()
    {
        float placement = 0f;
        bool isArmor = dd_itemType.value == 1;
        if (isArmor)
        {
            currentItem = itemRoller.RollArmor(ref placement, armorClass.value, dd_itemTier.value);
        }
        else
        {
            currentItem = itemRoller.RollWeapon(ref placement, dd_itemSpecific.value, dd_itemTier.value);

        }

        rarityBehavior.Roll(currentItem, placement);
        // apply the default item image for the item type
        // TODO this needs to be expanded to specific item types within each type
        currentItem.SetInventorySprite(LookupDictionary.instance.ItemImageDefaults.First(iType => iType.itemType == currentItem.GetItemType()).image);
        DisplayTooltip();

        // give player the item
        if (!playerInventory.AddItem(currentItem))
        {
            // drop on ground
            Transform groundItem = Instantiate(GroundItemPrefab, playerInventory.transform.position, Quaternion.identity).transform;
            groundItem.GetComponent<GroundItem>().SetItem(currentItem);
            GroundItemCanvas.instance.RegisterItem(currentItem, groundItem.transform);


        }
    }

    // testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UpdatePreview();
        }
    }


    public void UpdatePreview()
    {
        rarityBehavior.SetStartingRarity((int)LookupDictionary.rarityRangeForTier[dd_itemTier.value, 0]);
        DisplayTooltip();
    }

    void DisplayTooltip()
    {
        // if tooltip is null, generate a new one
        if (tooltip == null)
        {
            tooltip = Instantiate(tooltipPrefab, tooltipLocation.position, Quaternion.identity);
            tooltip.transform.SetParent(tooltipParent);
            rect = tooltip.GetComponent<RectTransform>();
            
            //rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tooltipLocation.lossyScale.x);
            //rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tooltipLocation.lossyScale.y);

            // Janky hack mate... I hate UIs...TODO
            rect.GetComponent<RectTransform>().localScale = new Vector3(tooltipLocation.localScale.x * 1.6f, tooltipLocation.localScale.y, tooltipLocation.localScale.z);
            //tooltip.GetComponent<RectTransform>().sizeDelta = tooltipLocation.sizeDelta;
        }

        tooltip.GetComponent<TooltipItem>().SetItem(currentItem);
    }

    private void PopulateArmorClass() // I'm not filling this in editor in case they change (they probably will)
    {
        List<string> options = new List<string>();
        TMP_Dropdown armorClassDropdown = armorClass.GetComponent<TMP_Dropdown>();
        armorClassDropdown.ClearOptions();

        foreach (Armor.ArmorClass ac in Enum.GetValues(typeof(Armor.ArmorClass)))
        {
            options.Add(ac.ToString());            
        }

        armorClassDropdown.AddOptions(options);
    }
}
