using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRoller : MonoBehaviour
{
    const string BASE_PATH = "ScriptableObjects/Items/";

    float[] tierMultipliers = { 1f, 3f, 6f, 11f, 18f, 29f, 45f};

    Item currentItem;
    int numToAdd = 0;
    float minStatVal = 0f, maxStatVal = 0f, multiplier = 0f;

    public void GetBaseProperties(ref float placement)
    {
        // determine the rarity
        int currentRarity = (int)currentItem.GetRarity();
        int newRarity = currentRarity;
        float[] chances = LookupDictionary.chanceToUpgradeRarity[currentRarity];

        float random = UnityEngine.Random.Range(0f, 100f);

        for(int i = chances.Length - 1; i >= 0; --i)
        {
            if(random < chances[i])
            {
                newRarity += i + 1;
                currentItem.SetRarity((Item.Rarity)newRarity);
                break;
            }
        }

        // where in the rarity it sits
        placement = UnityEngine.Random.Range(0f, 100f) / 100f;


        int minStats = LookupDictionary.minNumStatsForRarity[newRarity];
        int maxStats = LookupDictionary.maxNumStatsForRarity[newRarity];
        int numStats = UnityEngine.Random.Range((int)minStats, (int)maxStats + 1);

        numToAdd = Mathf.Clamp(numStats, 0, 8); // 8 should be a const somewhere

        minStatVal = ((currentItem.GetTier() + 1) * tierMultipliers[currentRarity]     * ((newRarity + 1) / (currentRarity + 1)));
        maxStatVal = ((currentItem.GetTier() + 1) * tierMultipliers[currentRarity + 1] * ((newRarity + 1) / (currentRarity + 1)));
        multiplier = Mathf.Lerp(minStatVal, maxStatVal, placement);
    }

    public Item RollWeapon(ref float placement, int weaponType, int tier)
    {
        currentItem = Instantiate((Item)Resources.Load(BASE_PATH + "Weapon/BaseWeapon", typeof(Item)));
        currentItem.SetRarity(LookupDictionary.rarityRangeForTier[tier, 0]);
        GetBaseProperties(ref placement);

        AddBasicStats();

        List<Weapon.Stat> statsToAdd = new List<Weapon.Stat>();
        int numOptions = Enum.GetNames(typeof(Weapon.Stats)).Length;

        ((Weapon)currentItem).SetWeaponType((Weapon.WeaponType)weaponType);
        currentItem.SetTier(tier);

        switch((Weapon.WeaponType)weaponType)
        {
            case Weapon.WeaponType.Hatchet:
            case Weapon.WeaponType.Bow:
            case Weapon.WeaponType.Daggers:
            case Weapon.WeaponType.Hammer:
            case Weapon.WeaponType.LongSword:
            case Weapon.WeaponType.ShortSword:
            case Weapon.WeaponType.Greataxe:
                {
                    // set attack damage
                    float minDamage = (10f * ((int)currentItem.GetRarity() + 1)) * ((int)currentItem.GetRarity() + 1);
                    float maxDamage = (10f * ((int)currentItem.GetRarity() + 2)) * ((int)currentItem.GetRarity() + 2);
                    float damage = Mathf.Lerp(minDamage, maxDamage, placement);

                    ((Weapon)currentItem).SetAttackDamage(damage * 0.8f, damage * 1.2f); // TODO
                }
            break;
            case Weapon.WeaponType.Staff:
                {
                    // set magic damage
                    float minDamage = (10f * ((int)currentItem.GetRarity() + 1)) * ((int)currentItem.GetRarity() + 1);
                    float maxDamage = (10f * ((int)currentItem.GetRarity() + 2)) * ((int)currentItem.GetRarity() + 2);
                    float damage = Mathf.Lerp(minDamage, maxDamage, placement);
                    ((Weapon)currentItem).SetMagicDamage(damage);
                }
            break;
            case Weapon.WeaponType.Mace:
            case Weapon.WeaponType.Wand:
                {
                    // set magic damage
                    float minDamage = (10f * ((int)currentItem.GetRarity() + 1)) * ((int)currentItem.GetRarity() + 1);
                    float maxDamage = (10f * ((int)currentItem.GetRarity() + 2)) * ((int)currentItem.GetRarity() + 2);
                    float damage = Mathf.Lerp(minDamage, maxDamage, placement);
                    ((Weapon)currentItem).SetMagicDamage(damage / 2f);
                }
            break;
        }


        // roll the stats to add
        for (int i = 0; i < numToAdd; ++i)
        {
            int rand = UnityEngine.Random.Range(0, numOptions);

            float min = multiplier * 0.75f;
            float max = multiplier * 1.25f;
            float val = UnityEngine.Random.Range(min, max);
            statsToAdd.Add(new Weapon.Stat((Weapon.Stats)rand, (int)val, true));
        }

        // combine duplicate stats
        if (statsToAdd.Count > 1)
        {
            for(int outer = 0; outer < numToAdd - 1; ++outer)
            {
                for(int inner = outer + 1; inner < numToAdd; ++inner)
                {
                    if(statsToAdd[outer]._stat == statsToAdd[inner]._stat &&
                        statsToAdd[outer]._flatValue == statsToAdd[inner]._flatValue)
                    {
                        statsToAdd[outer]._value += statsToAdd[inner]._value;
                        statsToAdd.RemoveAt(inner);
                        numToAdd--;
                    }
                }
            }
        }

        // add the stats to current item
        foreach(Weapon.Stat stat in statsToAdd)
        {
            ((Weapon)(currentItem)).AddStat(stat);
        }

        // Set the name of the item based on stats TODO
        // should be done in lookupDictionary
        currentItem.SetPrefix(LookupDictionary.prefixes[(int)(currentItem.GetRarity()), 0]);
        currentItem.SetSuffix(LookupDictionary.suffixes[(int)(currentItem.GetRarity()), 0]);
        currentItem.SetName(Enum.GetName(typeof(Weapon.WeaponType), ((Weapon)currentItem).GetWeaponType()));
        currentItem.init();

        return currentItem;
    }

    public Item RollArmor(ref float placement, int armorClass, int tier)
    {
        currentItem = Instantiate((Item)Resources.Load(BASE_PATH + "Armor/BaseArmor", typeof(Item)));
        GetBaseProperties(ref placement);

        AddBasicStats();

        List<Armor.Stat> statsToAdd = new List<Armor.Stat>();
        int numOptions = Enum.GetNames(typeof(Armor.Stats)).Length;

        ((Armor)currentItem).SetArmorClass((Armor.ArmorClass)armorClass);
        currentItem.SetTier(tier);

        // roll the stats to add
        for (int i = 0; i < numToAdd; ++i)
        {
            int rand = UnityEngine.Random.Range(0, numOptions);

            float min = multiplier * 0.75f;
            float max = multiplier * 1.25f;
            float val = UnityEngine.Random.Range(min, max);
            statsToAdd.Add(new Armor.Stat((Armor.Stats)rand, (int)val, true));
        }

        // combine duplicate stats
        if (statsToAdd.Count > 1)
        {
            for(int outer = 0; outer < numToAdd - 1; ++outer)
            {
                for(int inner = outer + 1; inner < numToAdd; ++inner)
                {
                    if(statsToAdd[outer]._stat == statsToAdd[inner]._stat &&
                        statsToAdd[outer]._flatValue == statsToAdd[inner]._flatValue)
                    {
                        statsToAdd[outer]._value += statsToAdd[inner]._value;
                        statsToAdd.RemoveAt(inner);
                        numToAdd--;
                    }
                }
            }
        }

        // add the stats to current item
        foreach(Armor.Stat stat in statsToAdd)
        {
            ((Armor)(currentItem)).AddStat(stat);
        }

        // Set the name of the item based on stats TODO
        // should be done in lookupDictionary
        currentItem.SetPrefix(LookupDictionary.prefixes[(int)(currentItem.GetRarity()), 0]);
        currentItem.SetSuffix(LookupDictionary.suffixes[(int)(currentItem.GetRarity()), 0]);
        currentItem.SetName(Enum.GetName(typeof(Item.EquipmentSlot), ((Armor)currentItem).GetEquipmentSlot()));

        return currentItem;
    }

    public void AddBasicStats()
    {
        print("Don't forget to add basic stats"); // TODO



    }
}
