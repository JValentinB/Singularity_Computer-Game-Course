using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public InvManager inventory;

    private void OnTriggerEnter(Collider col){
        if(col.tag == "Player"){
            InvManager playerInventory = col.gameObject.GetComponent<InvManager>();
            
            foreach(var item in inventory.stackedInventoryItems){
                playerInventory.AddItem(item.Item1, item.Item2);
                Debug.Log("Added Item: " + item.Item1.itemName + ", " + item.Item2 + "x");
            }

            Destroy(gameObject);
        }
    }
}
