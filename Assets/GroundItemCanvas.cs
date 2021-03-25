using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GroundItemCanvas : MonoBehaviour
{
    public static GroundItemCanvas instance = null;
    [SerializeField]
    GameObject dropTagPrefab;
    List<Drop> _activeDrops;
    Camera cam;
    [SerializeField]
    Inventory playerInventory;

    [SerializeField]
    SphereCollider pickupCollider;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        _activeDrops = new List<Drop>();
        StartCoroutine(MoveDropNames());
    }

    public bool ClickedGroundTag(Transform t)
    {

        for (int i = _activeDrops.Count - 1; i >= 0; --i)
        {
            // remove them as they are destroyed
            if (_activeDrops[i].transform == t)
            {

                // are we within range of it?
                if (Vector3.Distance(_activeDrops[i].groundItem.position, pickupCollider.transform.position) > pickupCollider.radius)
                    return false;

                // add to inventory
                if(!playerInventory.AddItem(_activeDrops[i].item))
                {
                    Debug.Log("Inventory full, could not add item");
                    return false;
                }

                Destroy(_activeDrops[i].groundItem.gameObject);
                Destroy(_activeDrops[i].transform.gameObject);
                _activeDrops.RemoveAt(i);
                return true;
            }
        }

        return false;
    }

    class Drop
    {
        public Transform transform;
        public Item item;
        public Transform groundItem;

        public Drop(Transform t, Item i, Transform g)
        {
            transform = t;
            item = i;
            groundItem = g;
        }
    }

    public void RegisterItem(Item item, Transform groundItem)
    {
        GameObject drop = Instantiate(dropTagPrefab, transform);
        TextMeshProUGUI text = drop.GetComponentInChildren<TextMeshProUGUI>();
        Image img = drop.GetComponentInChildren<Image>();
        text.text = item.GetFullItemName();
        text.ForceMeshUpdate();
        Vector2 newSize = new Vector2(text.textBounds.size.x, text.textBounds.size.y);
        img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, newSize.x * 1.2f);
        img.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newSize.y * 1.2f);

        text.color = LookupDictionary.instance.colors[(int)item.GetRarity()];
        _activeDrops.Add(new Drop(drop.transform, item, groundItem));
    }

    IEnumerator MoveDropNames()
    {
        yield return new WaitForSeconds(1f);

        Camera cam = Camera.main;

        while (Application.isPlaying)
        {

            for (int i = _activeDrops.Count - 1; i >= 0; --i)
            {
                // remove them as they are destroyed
                if (_activeDrops[i].groundItem == null)
                {
                    Destroy(_activeDrops[i].transform.gameObject);
                    _activeDrops.RemoveAt(i);
                }
                else
                    _activeDrops[i].transform.position = cam.WorldToScreenPoint(_activeDrops[i].groundItem.position);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
