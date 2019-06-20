public interface IStackable
{
    int AddToStack(); // will return the number of items that go above max stack
}

public interface IDroppableArea
{
    Inventory GetInventoryObject();
}

public interface IEquippableArea
{
    InventoryEquipmentSlot GetEquipmentSlot();
}
