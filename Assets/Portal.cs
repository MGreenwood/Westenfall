using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour 
{
    [SerializeField]
    int minRooms, maxRooms, difficulty;

    Transform player;

    [SerializeField]
    float interactionDistance = 11f;

    bool toLoad = false;

    private void Start()
    {
        player = FindObjectOfType<Player>().transform;
        StartCoroutine(WaitForLoad());
    }
    private void OnMouseDown()
    {
        if(Vector3.Distance(player.position, transform.position) < interactionDistance)
            toLoad = true;
    }

    IEnumerator WaitForLoad()
    {
        while (!toLoad)
            yield return new WaitForFixedUpdate();

        GameManager.instance.LoadDungeon(minRooms, maxRooms, difficulty);
    }

}
