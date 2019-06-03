using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CastBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    Canvas canvas;

    bool casting = false;

    public RectTransform maskRT;
    Vector2 startingSize;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;

        startingSize = maskRT.sizeDelta;

        // register with event
        PlayerAbilities.onCast += StartCast;
        PlayerAbilities.onCastCancel += CancelCast;
    }

    void CancelCast()
    {
        casting = false;
        canvas.enabled = false;
        StopAllCoroutines();
    }

    void StartCast(string abilityName, float castTime, PlayerAbilities.CastingComplete castingComplete)
    {

        canvas.enabled = true;
        maskRT.sizeDelta = new Vector2(0, maskRT.sizeDelta.y);

        casting = true;
        StartCoroutine(Cast(abilityName, castTime, castingComplete));
    }

    IEnumerator Cast(string abilityName, float castTime, PlayerAbilities.CastingComplete castingComplete)
    {
        float startTime = Time.fixedTime;
        float endTime = startTime + castTime;


        Vector2 size = maskRT.sizeDelta;

        float percent;

        while (casting)
        {
            percent = (Time.fixedTime - startTime) / castTime;
            size.x = startingSize.x * percent;
            maskRT.sizeDelta = size;

            text.text = abilityName + "  " + string.Format("{0:F1}", castTime - castTime * percent);

            if (percent >= 1.0f)
            {
                castingComplete?.Invoke();

                casting = false;
                canvas.enabled = false;
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
