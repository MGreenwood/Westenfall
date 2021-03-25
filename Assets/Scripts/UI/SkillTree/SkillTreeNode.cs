using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillTreeNode : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, 
    IPointerDownHandler, IPointerUpHandler
{
    public static bool _dragging = false;

    bool _pointerInside = false;

    [SerializeField]
    Item.BasicStats statType;
    [SerializeField]
    int valueRequired;

    bool unlocked = false;

    [SerializeField]
    GameObject tooltipPrefab;

    [SerializeField]
    Ability _ability;

    GameObject tooltip;

    Transform lastParent;
    Vector3 _originalPosition;
    public delegate void OnRelease(Ability ability);
    public static OnRelease onRelease;

    public Image _image;
    bool _pointerDown;

    EventSystem _eventSystem;

    void Start()
    {
        _eventSystem = GetComponent<EventSystem>();
        _image = GetComponent<Image>();
        _originalPosition = transform.position;
    }

    private void Update()
    {
        if(_dragging)
        {
            transform.position = Input.mousePosition;
        }
    }

    public Ability GetAbility() => _ability;

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        _pointerInside = true;
        StartCoroutine(DisplayTooltip());
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
            tooltip.GetComponent<TooltipItem>().SetAbility(_ability);
        }
        else
        {
            // set the position
            tooltip.transform.position = transform.position + tooltipOffset;
            tooltip.transform.SetParent(TooltipCanvas.instance.transform);
            tooltip.transform.SetAsLastSibling();
            tooltip.SetActive(true);
        }

        // check if tooltip.x + 1/2 tooltip size goes over current windows x, if yes, display on left side of item
        while (_pointerInside && !_dragging)
        {
            yield return new WaitForFixedUpdate();
        }

        tooltip.SetActive(false);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        if (_pointerInside)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                _dragging = true;

                lastParent = transform.parent;
                transform.SetParent(TooltipCanvas.instance.transform);
                _image.raycastTarget = false;

                // make semi-transparent
                Color c = _image.color;
                c.a = 0.75f;
                _image.color = c;
            }
        }

        _pointerDown = true;
    }
    
    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        if (_dragging)
        {
            // return opaqueness
            Color c = _image.color;
            c.a = 1f;
            _image.color = c;

            _dragging = false;
            _pointerDown = false;

            _image.raycastTarget = true;

            // go back home
            transform.position = _originalPosition;
            transform.SetParent(lastParent);

            // create a copy of the ability and send her off
            Ability _abilityOut = Instantiate(_ability);
            onRelease.Invoke(_abilityOut);
        }
    }


}
