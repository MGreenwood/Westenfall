using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField]
    private Teleporter partner;

    float _noCollideTimer = 0.5f;

    struct TPObject
    {
        public GameObject player;
        public float timeStarted;

        public TPObject(GameObject pIn, float tIn)
        {
            player = pIn;
            timeStarted = tIn;
        }
    }

    List<TPObject> _noCollideList;

    private void Start()
    {
        _noCollideList = new List<TPObject>();
    }

    private void FixedUpdate()
    {
        for(int i = _noCollideList.Count - 1; i >=0; i--)
        {
            if(_noCollideList[i].timeStarted + _noCollideTimer < Time.fixedTime)
            {
                _noCollideList.Remove(_noCollideList[i]);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            foreach(TPObject p in _noCollideList)
            {
                if (p.player == other.gameObject)
                    return;
            }

            partner.AcceptPlayer(other.transform);
        }
    }

    public void AcceptPlayer(Transform player)
    {
        player.position = transform.position;
        _noCollideList.Add(new TPObject(player.gameObject, Time.fixedTime));
    }
}
