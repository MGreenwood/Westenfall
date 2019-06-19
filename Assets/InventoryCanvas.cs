using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCanvas : MonoBehaviour
{
    [SerializeField]
    Inventory _inventory;

    public static List<InventoryCanvasContainer> Inventories;
    public GraphicRaycaster _raycaster;

    public struct InventoryCanvasContainer
    {
        public InventoryCanvas inventoryCanvas;
        public Inventory inventory;

        public InventoryCanvasContainer(InventoryCanvas inventoryCanvasIn, Inventory inventoryIn)
        {
            inventoryCanvas = inventoryCanvasIn;
            inventory = inventoryIn;
        }
    }

    private void Awake()
    {
        if(Inventories == null)
        {
            Inventories = new List<InventoryCanvasContainer>();
        }

        _raycaster = GetComponent<GraphicRaycaster>();
        Inventories.Add(new InventoryCanvasContainer(this, _inventory));
        UIManager.instance._equipmentCanvas = GetComponent<Canvas>();
    }

    private void OnDisable()
    {
        Inventories.Remove(new InventoryCanvasContainer(this, _inventory));
    }
}
