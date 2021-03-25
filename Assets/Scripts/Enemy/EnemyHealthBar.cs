using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour {

    public Enemy enemy; // must set the enemy in perfab

    Vector2 startingSize;

    Vector2 currentSize;

	// Use this for initialization
	void Start () {
        startingSize = GetComponent<RectTransform>().sizeDelta;
        GetComponent<Image>().enabled = false;
        enemy.healthChanged += UpdateHealth; // subscribe to hp change events
    }

    void UpdateHealth()
    {
        if (enemy.Health == enemy.MAX_HEALTH)
            GetComponent<Image>().enabled = false;
        else
            GetComponent<Image>().enabled = true;

        currentSize = GetComponent<RectTransform>().sizeDelta;
        float healthPercent = enemy.Health / enemy.MAX_HEALTH;

        Vector2 newRect = new Vector2(startingSize.x * healthPercent, startingSize.y);
        GetComponent<RectTransform>().sizeDelta = newRect;
    }
}
