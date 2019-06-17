using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    // Inventory will index from the top left x+ ->  y+ |
    //                                                  v
    [SerializeField]
    // TESTING TODO
    Text indexTest;


    [SerializeField]
    int _width = 0, _height = 0; // number of horizontal and vertical slots in the inventory
    [SerializeField]
    private Image _selectionArea; // the area representing the useable inventory
    [SerializeField]
    RectTransform firstTile;

    private bool _pointerInside = false;
    private bool _pointerDown = false;
    Vector3[] corners; // the corners of the inventory rect
    Vector2 rectStart;

    public Image itemImagePrefab;

    public struct Index
    {
        public int _x;
        public int _y;

        public Index(int x, int y)
        {
            _x = x; // column
            _y = y; // row
        }
    }
    enum SlotStatus { Available, TakenMain, TakenExtended}
    struct InventorySlot
    {
        public Item _item;
        public SlotStatus _slotStatus;

        public InventorySlot(Item item, SlotStatus slotStatus)
        {
            _item = item;
            _slotStatus = slotStatus;
        }
    }

    float localSizeOfTile; // the width/height of 1 tile in the inventory
    float globalSizeOfTile;
    InventorySlot[,] _inventory; // accessed by column , row

    private void Start()
    {
        _inventory = new InventorySlot[_width, _height]; // column / row

        RecalculateBounds();
    }

    public Image GetSelectionArea()
    {
        return _selectionArea;
    }

    public void RecalculateBounds()
    {
        RectTransform rt = _selectionArea.rectTransform;

        corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        rectStart = corners[1];
        localSizeOfTile = _selectionArea.rectTransform.sizeDelta.x / _width;

        globalSizeOfTile = (corners[2].x - corners[1].x) / _width;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            AddItem(Resources.Load<Item>("ScriptableObjects/Items/Weapons/Melee/Swords/ShortSword") as Weapon);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddItem(Resources.Load<Armor>("ScriptableObjects/Items/Armor/Head/SoftCap") as Armor);
        }

        // Display inventory index over mouse
        if (GameManager.instance.Debug)
        {
            // if inside inventory
            Vector3 mousePos = Input.mousePosition;

            if (mousePos.x > corners[1].x && mousePos.x < corners[2].x &&
                mousePos.y < corners[1].y && mousePos.y > corners[0].y)
            {
                indexTest.transform.position = Input.mousePosition + new Vector3(0f, 10f);
                Index indexxx = IndexAtPosition(Input.mousePosition);
                indexTest.text = string.Format("Index = {0},{1}", indexxx._x, indexxx._y);
            }
            else
            {
                indexTest.text = "";
            }

        }
    }

    Vector3 PositionAtIndex(int column, int row)
    {
        return firstTile.localPosition + new Vector3(column, -row) * localSizeOfTile;
    }

    // index is x position, y position
    Index IndexAtPosition(Vector3 position)
    {        

        Vector2 offset = new Vector2(position.x - corners[1].x, corners[1].y - position.y);

        Index ndx = new Index((int)(offset.x / globalSizeOfTile),(int)(offset.y / globalSizeOfTile));

        return ndx;
    }

    public bool AddItem(Item item)
    {
        item.init();

        // add to first available
        for (int r=0;r<_height;r++)
        {
            for (int c=0;c<_width;c++)
            {
                Index nx = new Index(c, r);
                if (TryFit(item, nx))
                {
                    AddToSlot(item, nx);
                    CreateImage(nx);

                    return true;
                }
            }
        }

        return false;
    }

    // Add item to this inventory from other inventory or equipment slot
    // this returns the size the image must be adjusted to
    public bool AddItem(Item item, Vector3 clickPos, ref Image img)
    {
        // item already exists, do not need to init
        // must set the size of the image

        // if cannot place where clicked, add to first available
        for (int r = 0; r < _height; r++)
        {
            for (int c = 0; c < _width; c++)
            {
                Index nx = new Index(c, r);
                if (TryFit(item, nx))
                {
                    AddToSlot(item, nx);
                    //CreateImage(nx);
                    // set size of existing image
                    int wid = item.GetItemSize()._width;
                    int hig = item.GetItemSize()._height;
                    img.rectTransform.localScale = Vector3.one;
                    img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSizeOfTile * wid);
                    img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSizeOfTile * hig);
                    
                    // set position of image
                    img.rectTransform.localPosition = PositionAtIndex(nx._x, nx._y) - HalfTile();

                    return true;
                }
            }
        }
        return false;
    }

    public void AddToSlot(Item item, Index index)
    {
        // This should only be called AFTER confirming an item fits in a slot
        // index is top-left-most slot of item
        _inventory[index._x, index._y] = new InventorySlot(item, SlotStatus.TakenMain);
        bool skippedFirst = false;

        // fill in the rest as Taken Extended
        for(int x = index._x; x < index._x + item.GetItemSize()._width; x++)
        {
            for(int y = index._y; y < index._y + item.GetItemSize()._height; y++)
            {
                if (!skippedFirst)
                    skippedFirst = true;
                else
                {
                    _inventory[x, y] = new InventorySlot(item, SlotStatus.TakenExtended);
                }
            }
        }
    }

    public void RemoveFromSlot(Item item, Index index)
    {
        Item.ItemSize itemSize = item.GetItemSize();

        for (int x = index._x; x < index._x + itemSize._width; x++)
        {
            for (int y = index._y; y < index._y + itemSize._height; y++)
            {
                _inventory[x, y] = new InventorySlot(null, SlotStatus.Available);
            }
        }
    }

    void CreateImage(Index index)
    {
        // set parent
        Image img = Instantiate(itemImagePrefab);
        img.transform.SetParent(transform);

        Item item = _inventory[index._x, index._y]._item;
        img.GetComponent<InventoryItemObject>().init(item, index);

        // set size of image
        int wid = item.GetItemSize()._width;
        int hig = item.GetItemSize()._height;
        img.rectTransform.localScale = Vector3.one;
        img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, localSizeOfTile * wid);
        img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, localSizeOfTile * hig);

        // set position of image
        img.rectTransform.localPosition = PositionAtIndex(index._x, index._y) - HalfTile();

        // set sprite of image
        img.sprite = item.GetSprite();
    }

    // size of tile +x -y
    public Vector3 HalfTile() => new Vector3(localSizeOfTile * 0.5f, -localSizeOfTile * 0.5f);

    public bool TryMove(Item item, Index index, Vector3 clickPos, out Index newIndex)
    {
        // index points to top left of object        
        newIndex = IndexAtPosition(clickPos);
        
        if (TryFit(item, newIndex))
        {
            RemoveFromSlot(item, index);
            AddToSlot(item, newIndex);
            return true;
        }
        
        return false;
    }

    public void UpdatePosition(InventoryItemObject itemObject, Index ndx)
    {
        itemObject.transform.localPosition = PositionAtIndex(ndx._x, ndx._y) - HalfTile();
        //OutputInventoryState(); // TODO Take this out
    }

    public void OutputInventoryState()
    {
        for(int r=0;r<_height;r++)            
        {
            string line = "";
            for(int c=0;c<_width;c++)
            {
                line += _inventory[c, r]._slotStatus == SlotStatus.Available ? "a, " : "t, ";
            }
            Debug.Log(line);
        }
    }

    // returns a vector for placing item image, +x -y
    Vector3 OffsetForLocalSize(Item.ItemSize itemSize)
    {
        float halfWidth = itemSize._width * localSizeOfTile * 0.5f;
        float halfHeight= itemSize._height * localSizeOfTile * 0.5f;

        return new Vector3(halfWidth, -halfHeight);
    }

    Vector3 OffsetForGlobalSize(Item.ItemSize itemSize)
    {
        float halfWidth = itemSize._width * globalSizeOfTile * 0.5f;
        float halfHeight = itemSize._height * globalSizeOfTile * 0.5f;

        return new Vector3(halfWidth, -halfHeight);
    }

    bool TryFit(Item item, Index index)
    {
        for (int r = index._y; r < index._y + item.GetItemSize()._height - 1; r++)
        {
            for (int c = index._x; c < index._x + item.GetItemSize()._width - 1; c++)
            {
            
                if(c >= _inventory.GetLength(0) -1 || r >= _inventory.GetLength(1) -1) // outside of inventory bounds
                {
                    return false;
                }

                if(_inventory[c,r]._slotStatus != SlotStatus.Available)// && _inventory[c,r]._item != item)
                {
                    return false;
                }
            }
        }

        return true;
    }

    
}
