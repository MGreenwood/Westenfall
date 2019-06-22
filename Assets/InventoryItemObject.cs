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
    Transform lastParent;

    float tooltipDelay = 0.5f;

    [SerializeField]
    GameObject groundItemPrefab;

    [SerializeField]
    GameObject tooltipPrefab;

    GameObject tooltip;
    Player player;

    bool isEquipped = false;

    public void init(Item item, Inventory.Index index)
    {
        _item = item;
        _index = index;
    }

    private void Start()
    {
        _eventSystem = GetComponent<EventSystem>();
        _image = GetComponent<Image>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
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

        StartCoroutine(DisplayTooltip());

        // else display previous tooltip
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _pointerInside = false;
            
    }

    IEnumerator DisplayTooltip()
    {
        yield return new WaitForSeconds(tooltipDelay);

        // if tooltip is null, generate a new one
        if (tooltip == null)
        {
            tooltip = Instantiate(tooltipPrefab, transform.position + new Vector3(10, 0, 0), Quaternion.identity);
            tooltip.transform.SetParent(TooltipCanvas.instance.transform);
            //tooltip.transform.SetAsLastSibling();
            tooltip.GetComponent<TooltipItem>().SetItem(_item);
        }
        else
        {
            // set the position
            tooltip.transform.position = transform.position;
            tooltip.transform.SetParent(TooltipCanvas.instance.transform);
            tooltip.transform.SetAsLastSibling();
            tooltip.SetActive(true);
        }

        // check if tooltip.x + 1/2 tooltip size goes ovber current windows x, if yes, display on left side of item
        while (_pointerInside && !_dragging)
        {
            yield return new WaitForFixedUpdate();
        }

        tooltip.SetActive(false);
    }

    public Image GetImage() => _image;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (_pointerInside)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                //determine the type of item
                if (_item is Weapon)
                {

                    if (isEquipped)
                    {
                        // item is equipped, either do nothing or unequip

                    }
                    else if (!isEquipped)                        
                    {
                        isEquipped = true;
                        player.GetInventory().RemoveFromSlot(_item, _index);

                        if (!player.EquipWeapon(this))
                        {
                            player.GetInventory().AddToSlot(_item, _index);
                        }
                    }
                }
                else if (_item is Armor)
                {
                    if (isEquipped)
                    {
                        // item is equipped, either do nothing or unequip

                    }
                    else if (!isEquipped)
                    {
                        isEquipped = true;
                        player.GetInventory().RemoveFromSlot(_item, _index);

                        if (!player.EquipArmor(this))
                        {
                            player.GetInventory().AddToSlot(_item, _index);
                        }
                    }
                }                
            }
            else
            {

                _dragging = true;
                _originalPosition = transform.localPosition;
                DraggedObject = this;

                // reparent to show on top of other items
                //_image.transform.SetAsLastSibling();
                lastParent = _image.transform.parent;
                _image.transform.SetParent(TooltipCanvas.instance.transform);
                _image.raycastTarget = false;

                // make semi-transparent
                Color c = _image.color;
                c.a = 0.75f;
                _image.color = c;
            }
        }

        _pointerDown = true;
    }

    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (_dragging)
        {
            bool foundHome = false;
            bool foundDroppableArea = false;

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
                            if (isEquipped)
                            {
                                //unequip the item!
                                inv.inventory.SetParentAndSize(this);
                                player.Unequip(_item.GetEquipmentSlot());
                                isEquipped = false;
                            }
                            else
                            {
                                _image.transform.SetParent(lastParent);
                            }

                            // move the item
                            inv.inventory.UpdatePosition(this, newIndex);
                            _index = newIndex;
                            foundHome = true;
                        }
                        foundDroppableArea = true;
                    }
                }
            }

            // check the equipment areas, maybe trying to equip
            //TODO TODO


            if (!foundHome)
            {
                if (!UIManager.instance._isOverUI) // not over UI, drop item on ground
                {
                    Vector3 dropPos = player.transform.position;
                    GameObject groundItem = Instantiate(groundItemPrefab, dropPos, Quaternion.identity);
                    groundItem.GetComponent<GroundItem>().SetItem(_item);

                    if(!isEquipped) // if equipped, dont need to remove from slot because its not in an inventory
                        lastParent.GetComponent<Inventory>().RemoveFromSlot(_item, _index);
                    //TODO
                    //transform.parent.GetComponent<Inventory>().RemoveFromSlot(_item, _index);
                    Destroy(gameObject);
                }
                else // still within UI, return to original position
                {
                    // else return to original position
                    _image.transform.SetParent(lastParent);
                    transform.localPosition = _originalPosition;
                }
            }

            // return opaqueness
            Color c = _image.color;
            c.a = 1f;
            _image.color = c;

            _dragging = false;
            DraggedObject = null;
            _pointerDown = false;

            _image.raycastTarget = true;
        }
    }
}
