using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBarSlot : MonoBehaviour, ISkillBarArea, 
    IPointerEnterHandler, IPointerExitHandler

{
    [SerializeField]
    Image image;

    bool _pointerInside = false;

    [SerializeField]
    PlayerAbilities player;

    public enum AbilitySlot { Ability1, Ability2, Ability3, Movement }
    [SerializeField]
    AbilitySlot abilitySlot;

    public void SetAbility(Sprite sprite)
    {
        image.sprite = sprite;
    }

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
        SkillTreeNode.onRelease += PointerUp;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        _pointerInside = true;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        _pointerInside = false;
    }

    public void PointerUp(Ability ability)
    {
        if (_pointerInside)
        {
            // set the ability in the correct slot
            SetAbility(ability.GetIcon());

            // the first two slots are weapon abilities
            player.SetAbility(1 + (int)abilitySlot, ability);
        }
    }
}
