using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModYOnValue : MonoBehaviour
{
    [SerializeField]
    Transform top, bottom;

    [SerializeField]
    Player player;

    public enum Valuetype { Health, Mana }

    [SerializeField]
    TextMeshProUGUI text;

    [SerializeField]
    Valuetype _valueType;

    private void Start()
    {
        player.SubscribeToHealthChanged(UpdateValues);
        player.SubscribeToManaChanged(UpdateValues);
        Invoke("UpdateValues", 1);
    }

    private void UpdateValues()
    {
        float percent;
        if (_valueType == Valuetype.Health)
        {
            percent = (float)player.GetCurrentHealth() / (float)player.GetMaxHealth();
            text.text = $"{player.GetCurrentHealth()} / {player.GetMaxHealth()}";
        }
        else
        {
            percent = (float)player.GetCurrentMana() / (float)player.GetMaxMana();
            text.text = $"{player.GetCurrentMana()} / {player.GetMaxMana()}";
        }

        transform.position = Vector3.Lerp(
            new Vector3(transform.position.x, bottom.position.y),
            new Vector3(transform.position.x, top.position.y),
            percent);
    }
}
