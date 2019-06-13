using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GroundItem : MonoBehaviour
{
    Camera _cam;
    [SerializeField]
    Item _item;

    [SerializeField]
    MeshRenderer planeRenderer;
    [ SerializeField, ColorUsageAttribute(true, true)]
    Color[] colors;
    [SerializeField]
    float[] width;

    [SerializeField]
    TextMeshProUGUI text;

    void Start()
    {
        _cam = Camera.main;
        GetComponent<Canvas>().worldCamera = _cam;
    }

    void Update()
    {
        transform.LookAt(transform.position + _cam.transform.rotation * Vector3.back, _cam.transform.rotation * Vector3.up);
    }

    public void SetItem(Item item)
    {
        _item = item;
        _item.init();
        int rarity = (int)item.GetRarity();
        planeRenderer.material.SetColor("_RarityColor", colors[rarity]);
        planeRenderer.material.SetFloat("_Width", width[rarity]);
        text.text = _item.GetItemName();
    }

    public Item GetItem() => _item;
}
