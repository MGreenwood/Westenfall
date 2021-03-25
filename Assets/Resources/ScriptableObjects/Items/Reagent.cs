using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "Items/Reagent")]
public class Reagent : Item
{
    public enum ReagentType { SalvagedGoods, NetherDust, Rune, Gemstone }

    [SerializeField]
    ReagentType _reagentType;
    
    [SerializeField]
    bool _stackable;

    [SerializeField]
    int _maxStackSize;

    int _stackSize;

    public int GetStackSize() => _stackSize;

    public override void init()
    {
        switch (_reagentType)
        {
            case ReagentType.SalvagedGoods:
                _itemSize = new ItemSize(2, 2);
                break;
            case ReagentType.NetherDust:
            case ReagentType.Rune:
                _itemSize = new ItemSize(1, 1);
                break;
            case ReagentType.Gemstone:
                _itemSize = new ItemSize(1, 2);
                break;
        }
    }

    public int AddToStack(int add)
    {
        // returns 0 if okay
        // returns extra if full stack
        int total = _stackSize + add;
        bool hasExtra = total > _maxStackSize;
        if (hasExtra)
        {
            _stackSize = _maxStackSize;
            return total - _maxStackSize;
        }

        _stackSize += add;
        return 0;
    }

    public bool RemoveFromStack(int val)
    {
        // returns true if okay
        // false if not enough
        if (val > _stackSize)
            return false;

        _stackSize -= val;
        return true;
    }

    public ReagentType GetReagentType() => _reagentType;

}
