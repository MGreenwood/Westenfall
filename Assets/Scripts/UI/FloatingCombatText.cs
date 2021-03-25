using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingCombatText : MonoBehaviour
{
    public GameObject combatText;

    public Color[] textColors; // 0-basic   1-bleed   2-burn   3-poison   4-Crit
    public Transform damageCanvas;
    Camera cam;

    private void Start()
    {
        GetComponent<IDamageable>().damageTaken += ShowCombatText;
        cam = Camera.main;
    }

    private void ShowCombatText(float damage, Effect.AbilityEffect effect, bool crit)
    {
        if (UISettings.instance.CombatActive)
        {
            // show the combat text
            initCBT(damage, effect, crit);
        }
    }

    void initCBT(float damage, Effect.AbilityEffect effect, bool crit)
    {
        GameObject temp = Instantiate(combatText) as GameObject;
        temp.GetComponent<CombatTextDestroy>().SetParentWorldTransform(transform);
        RectTransform tempRect = temp.GetComponent<RectTransform>();


        temp.transform.SetParent(damageCanvas);
        tempRect.transform.localPosition = Vector3.zero;
        tempRect.transform.localScale = combatText.transform.localScale;

        switch (effect.effectType)
        {
            case Effect.EffectType.Basic:
                temp.GetComponent<TMPro.TextMeshProUGUI>().color = textColors[0];
                break;
            case Effect.EffectType.Bleed:
                temp.GetComponent<TMPro.TextMeshProUGUI>().color = textColors[1];
                break;
            case Effect.EffectType.Burn:
                temp.GetComponent<TMPro.TextMeshProUGUI>().color = textColors[2];
                break;
            case Effect.EffectType.Poison:
                temp.GetComponent<TMPro.TextMeshProUGUI>().color = textColors[3];
                break;            
        }

        if(crit)
            temp.GetComponent<TMPro.TextMeshProUGUI>().fontSize *= 1.2f;


        temp.GetComponent<TMPro.TextMeshProUGUI>().text = ((int)damage).ToString();

        temp.GetComponent<Animator>().SetTrigger("Hit");
    }
}
