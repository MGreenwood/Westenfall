using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookupDictionary : MonoBehaviour
{
    // This should be a database eventually. Gathering everything here will make it easier to implement the db later
    // TODO
    public static LookupDictionary instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    // UI
    //
    public const float tooltipDelay = 0.2f;
    // end UI
    //

    // Player 
    //
    public int maxLevel = 100;
    // 
    // End Player

    [ColorUsageAttribute(true, true), Tooltip("Colors for each rarity tier")]
    public Color[] colors;

    // the fallback images of an item if not specified
    [Serializable]
    public struct ItemImage
    {
        public Item.ItemType itemType;
        public Sprite image;
    }
    public ItemImage[] ItemImageDefaults;

    // Combat 
    //
    public const float _knockbackForce = 20f;
    // end Combat
    //


    // Smith
    //
    [SerializeField]
    float smithExperienceMultiplier, startingExperieceRequired; // Required experience per smith's level to reach the next level

    int maxSmithLevel = 10;

    public static readonly int[] recommendedTierLevels = { 1, 30, 45, 60, 85, 100 };
    public static readonly int[] minNumStatsForRarity = { 1, 2, 3, 3, 4, 6 };
    public static readonly int[] maxNumStatsForRarity = { 1, 3, 4, 5, 6, 8 };
    public enum Tiers { TierI, TierII, TierIII, TierIV, TierV }

    public static readonly Item.Rarity[,] rarityRangeForTier = {    { Item.Rarity.Common, Item.Rarity.Uncommon },
                                                                    { Item.Rarity.Uncommon, Item.Rarity.UltraRare },
                                                                    { Item.Rarity.Rare, Item.Rarity.Epic },
                                                                    { Item.Rarity.UltraRare, Item.Rarity.Mythic },
                                                                    { Item.Rarity.Epic, Item.Rarity.Mythic } };

    public int MaxSmithLevel() => maxSmithLevel;

    // these are the chances to upgrade the rarity of an item during craft
    // index 0 is the chance to upgrade by 1
    // index 1 is the chance to upgrade by 2
    public static readonly float[][] chanceToUpgradeRarity = {new float[]{ 20f, 12f }, // common
                                                              new float[]{ 16f, 10f }, // uncommon
                                                              new float[]{ 11f, 8f }, // rare
                                                              new float[]{ 8f, 5f }, // ultrarare
                                                              new float[]{ 4f, 0f },  // epic
                                                              new float[]{ 0f, 0f }}; // mythic
 
    // Item Names
    public static readonly string[,] prefixes = {
                                    { "Shabby", "Weak" },
                                    { "Solid", "Strong" },
                                    {"Reinforced", "Well Crafted" },
                                    {"Incredible", "Complex" },
                                    {"Illustrious", "Rumored" },
                                    {"Godlike", "Impossible" } };

    public static readonly string[,] suffixes = {
                                    { "of power", "of strength" },               // Strength
                                    { "of quickness", "of speed" },              // Dexterity
                                    { "of regeneration", "of mental strength" }, // Spirit
                                    { "of life", "of longevity" },               // Stamina
                                    { "of the void", ", the arcane" } };         // Magic

    // End Item Names
    // end smith
    //
}
