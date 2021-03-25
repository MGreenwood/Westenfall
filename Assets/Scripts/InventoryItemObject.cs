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
    private bool _dragging = false;

    [SerializeField]
    private GameObject _rarityOutline;
    [SerializeField]
    private GameObject _itemSprite;

    private Vector3 _originalPosition;

    public static InventoryItemObject DraggedObject = null;
    EventSystem _eventSystem;
    Image _image;
    Transform lastParent;


    [SerializeField]
    GameObject groundItemPrefab;

    [SerializeField]
    GameObject tooltipPrefab;

    GameObject tooltip;
    Player player;

    RectTransform rect;

    bool isEquipped = false;

    public void init(Item item, Inventory.Index index)
    {
        _item = item;
        _index = index;

        Color rarityColor = LookupDictionary.instance.colors[(int)item.GetRarity()];
        _rarityOutline.GetComponent<Image>().color = new Color(rarityColor.r, rarityColor.g, rarityColor.b, 0.7f);
        _image = _itemSprite.GetComponent<Image>();
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _eventSystem = GetComponent<EventSystem>();

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
        yield return new WaitForSeconds(LookupDictionary.tooltipDelay);

        Vector3 tooltipOffset = new Vector3(-Screen.width / 8f, 0, 0);

        // if tooltip is null, generate a new one
        if (tooltip == null)
        {
            tooltip = Instantiate(tooltipPrefab, transform.position + tooltipOffset, Quaternion.identity);
            tooltip.transform.SetParent(TooltipCanvas.instance.transform);
            //tooltip.transform.SetAsLastSibling();
            tooltip.GetComponent<TooltipItem>().SetItem(_item);
        }
        else
        {
            // set the position
            tooltip.transform.position = transform.position + tooltipOffset;
            tooltip.transform.SetParent(TooltipCanvas.instance.transform);
            tooltip.transform.SetAsLastSibling();
            tooltip.SetActive(true);
        }

        // check if tooltip.x + 1/2 tooltip size goes ovber current windows x, if yes, display on left side of item
        while (_pointerInside && !_dragging)
        {
            yield return new WaitForEndOfFrame();
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
                        // item is equipped, either do nothing or unequip TODO

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
                            isEquipped = false;
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
                lastParent = transform.parent;
                transform.SetParent(TooltipCanvas.instance.transform);
                GetComponent<Image>().raycastTarget = false;

                // make semi-transparent
                Color c = _image.color;
                c.a = 0.75f;
                _image.color = c;
            }
        }
    }

    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;
    }

    public void SetSize(float width, float height)
    {
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        
        _rarityOutline.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        _rarityOutline.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        _itemSprite.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        _itemSprite.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public void SetScale(Vector3 localScale, Vector2 sizeDelta)
    {
        rect.localScale = localScale;
        rect.sizeDelta = sizeDelta;

        _rarityOutline.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);// localScale;
        _rarityOutline.GetComponent<RectTransform>().sizeDelta = sizeDelta;

        _itemSprite.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f); // localScale;
        _itemSprite.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (_dragging)
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
                            if (isEquipped)
                            {
                                //unequip the item!
                                inv.inventory.SetParentAndSize(this);
                                if (_item is Weapon)
                                {
                                    player.Unequip(Item.EquipmentSlot.Weapon);
                                }
                                else
                                {
                                    player.Unequip(((Armor)_item).GetEquipmentSlot());
                                }

                                isEquipped = false;
                            }
                            else
                            {
                                transform.SetParent(lastParent);
                            }

                            // move the item
                            inv.inventory.UpdatePosition(this, newIndex);
                            _index = newIndex;
                            foundHome = true;
                        }
                    }
                }
            }

            // check the equipment areas, maybe trying to equip
            //TODO TODO

            bool toDestroy = false;

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
                    //Destroy(gameObject);
                    toDestroy = true;
                }
                else // still within UI, return to original position
                {
                    // else return to original position
                    transform.SetParent(lastParent);
                    transform.localPosition = _originalPosition;
                }
            }

            // return opaqueness
            Color c = _image.color;
            c.a = 1f;
            _image.color = c;

            _dragging = false;
            DraggedObject = null;

            GetComponent<Image>().raycastTarget = true;

            if (toDestroy)
                Destroy(gameObject);
        }
    }

    public GameObject GetRarityOutline() => _rarityOutline;
}
