using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettings : MonoBehaviour
{
    public static UISettings instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public struct Combat
    {
        public bool showCombatText;

        public Combat(bool text_)
        {
            showCombatText = text_;
        }
    }
    Combat combatSettings;

    // Start is called before the first frame update
    void Start()
    {
        combatSettings = new Combat(true);
    }

    public void SetCombatText(bool on)
    {
        combatSettings.showCombatText = on;
    }

    public bool CombatActive
    {
        get { return combatSettings.showCombatText; }
    }
}
