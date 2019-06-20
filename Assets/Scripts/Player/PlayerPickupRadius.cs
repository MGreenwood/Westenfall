using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickupRadius : MonoBehaviour
{
    int _itemMask;
    List<Collider> _itemsInside;
    [SerializeField]
    Inventory playerInventory;

    private void Start()
    {
        _itemMask = LayerMask.NameToLayer("GroundItem");
        _itemsInside = new List<Collider>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (_itemsInside.Count == 0)
                return;

            // get closest item in list
            float lowestDist = 500f;
            int index = -1;
            for(int i=0;i<_itemsInside.Count;i++) 
            {
                if (Vector3.Distance(_itemsInside[i].transform.position, transform.position) < lowestDist)
                    index = i;
            }
            
            Item itemToAdd = index != -1 ? _itemsInside[index].gameObject.GetComponent<GroundItem>().GetItem(): null;
            if(itemToAdd != null)
            {
                // add to inventory
                if(!playerInventory.AddItem(itemToAdd))
                {
                    Debug.Log("Inventory full, could not add item");
                    return;
                }

                // remove from ground
                // keep reference to collider
                Collider c = _itemsInside[index];
                _itemsInside.Remove(_itemsInside[index]);
                Destroy(c.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == _itemMask && !_itemsInside.Contains(other))
        {
            _itemsInside.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == _itemMask && _itemsInside.Contains(other))
        {
            _itemsInside.Remove(other);
        }
    }
}
