using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RarityUIBehavior : MonoBehaviour
{
    [SerializeField]
    RectTransform rarityImage;
    [SerializeField]
    Transform rarityBar;
    [SerializeField]
    Text rarityText;

    Vector3 startPos;

    //testing
    [SerializeField]
    Item itemTest;

    float cheatScalar = 0.001f;
    float lerpAmt = 0.028f;

    Coroutine currentRoll;
    int startingRarity;

    private void Awake()
    {
        startPos = rarityBar.localPosition;
    }

    public void Roll(Item item, float percent) // percent is where in the rarity it sits. 1-99, 99 being the best
    {
        if (currentRoll != null)
        {
            StopCoroutine(currentRoll);
            currentRoll = null;
        }

        float distance = (int)item.GetRarity() - startingRarity + percent;

        currentRoll = StartCoroutine(AnimateRoll(distance));
    }


    public void SetStartingRarity(int sRarity)
    {
        Reset();
        startingRarity = (int)sRarity;
        Vector3 endPos = startPos + (Vector3.left * rarityImage.sizeDelta.x * startingRarity);
        rarityBar.localPosition = endPos;
        CheckForNextRarity();
    }

    public void Reset()
    {
        if (currentRoll != null)
        {
            StopCoroutine(currentRoll);
            currentRoll = null;
        }

        rarityBar.localPosition = startPos + (Vector3.left * rarityImage.sizeDelta.x * startingRarity);
    }

    void Done()
    {
        print("Done rolling.");
    }

    void CheckForNextRarity()
    {
        int index = (int)(Vector3.Distance(startPos, rarityBar.localPosition) / rarityImage.sizeDelta.x);
        Item.Rarity r = (Item.Rarity)index;
        rarityText.text = r.ToString();
    }

    IEnumerator AnimateRoll( float dist)
    {
        Vector3 endPos = rarityBar.localPosition + Vector3.left * rarityImage.sizeDelta.x * dist;
        float cheatAmt = Vector3.Distance(endPos, startPos);

        while (Application.isPlaying)
        {
            rarityBar.localPosition = Vector3.Lerp(rarityBar.localPosition, endPos, lerpAmt);

            if(Vector3.Distance(rarityBar.localPosition, endPos) < cheatAmt * cheatScalar)
            {
                break;
            }

            CheckForNextRarity();
            yield return new WaitForFixedUpdate();        
        }

        Done();
    }
}
