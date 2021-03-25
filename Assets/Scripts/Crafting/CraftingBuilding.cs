using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CraftingBuilding : MonoBehaviour
{
    int _level;
    int _experience;

    class ReagentTotal
    {
        public Reagent.ReagentType rType;
        public int stackSize;
        public Item.Rarity rarity;

        public ReagentTotal(Reagent.ReagentType rt, int ss, Item.Rarity r)
        {
            rType = rt;
            stackSize = ss;
            rarity = r;
        }
    }
    List<ReagentTotal> reagentsAdded;

    // Start is called before the first frame update
    void Start()
    {
        reagentsAdded = new List<ReagentTotal>();
    }
    
    public void AddReagent(Reagent reagent)
    {
        int index = -1;
        for(int i = 0; i < reagentsAdded.Count; ++i)
        {
            if(reagentsAdded[i].rType == reagent.GetReagentType())
            {
                index = i;
                break;
            }
        }
        if (index == -1)
            reagentsAdded.Add(new ReagentTotal(reagent.GetReagentType(), reagent.GetStackSize(), reagent.GetRarity()));
        else
            reagentsAdded[index].stackSize += reagent.GetStackSize();

        CalculateBonus();
    }

    void CalculateBonus()
    {
        foreach(ReagentTotal rt in reagentsAdded)
        {
            
        }

    }
}
