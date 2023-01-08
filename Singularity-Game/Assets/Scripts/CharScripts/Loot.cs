using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public InvManager inventory;

    private void OnTriggerEnter(Collider col){
        int overflow = 0;
        if(col.tag == "Player"){
            InvManager playerInventory = col.gameObject.GetComponent<InvManager>();
            foreach(var item in inventory.stackedInventoryItems){
                overflow = playerInventory.AddItem(item.Item1, item.Item2);
                inventory.RemoveItem(item.Item1, item.Item2 - overflow);
            }
            if(inventory.IsEmpty()){
                Destroy(this);
            }
        }
    }
}
