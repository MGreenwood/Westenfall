using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemObject : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, 
    IPointerDownHandler, IPointerUpHandler
{
    private Item _item;
    private Inventory.Index _index;
    private bool _pointerInside = false;
    private bool _pointerDown = false;
    private bool _dragging = false;

    private Vector3 _originalPosition;

    public static InventoryItemObject DraggedObject = null;
    EventSystem _eventSystem;
    Image _image;

    public void init(Item item, Inventory.Index index)
    {
        _item = item;
        _index = index;
    }

    private void Start()
    {
        _eventSystem = GetComponent<EventSystem>();
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        if(_dragging)
        {
            transform.position = Input.mousePosition;

            // show the object outline at calculated index displaying if in a droppable area
            // green if space available in that inventory
            // red if not

        }
    }

    public Item GetItem()
    {        
        return _item;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        _pointerInside = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _pointerInside = false;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (_pointerInside)
        {
            _dragging = true;
            _originalPosition = transform.localPosition;
            DraggedObject = this;

            // make semi-transparent
            Color c = _image.color;
            c.a = 0.75f;
            _image.color = c;
        }

        _pointerDown = true;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if(_dragging)
        {
            

            bool foundHome = false;

            // check if inside a droppable area
            foreach (InventoryCanvas.InventoryCanvasContainer inv in InventoryCanvas.Inventories)
            {
                GraphicRaycaster raycaster = inv.inventoryCanvas._raycaster;
                PointerEventData pointerEventData = new PointerEventData(_eventSystem);

                pointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();

                raycaster.Raycast(pointerEventData, results);
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.GetComponent<IDroppableArea>() != null)
                    {
                        Inventory.Index newIndex;
                        if (inv.inventory.TryMove(_item, _index, pointerEventData.position, out newIndex))
                        {
                            // move the item
                            inv.inventory.UpdatePosition(this, newIndex);

                            Debug.Log(string.Format("Index {0},{1} at position {2},{3}", 
                                newIndex._x, newIndex._y, pointerEventData.position.x, pointerEventData.position.y));

                            foundHome = true;
                        }
                    }
                }
            }

            if (!foundHome)
            {
                // else return to original position
                transform.localPosition = _originalPosition;
            }

            // return opaqueness
            Color c = _image.color;
            c.a = 1f;
            _image.color = c;
        }

        _dragging = false;
        DraggedObject = null;
        _pointerDown = false;
    }
}
