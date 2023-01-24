using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    public InvManager inventory;

    private void OnTriggerEnter(Collider col){
        if(col.tag == "Player"){
            var player = col.gameObject.GetComponent<Player>();
            
            foreach(var item in inventory.stackedInventoryItems){
                player.GiveItem(item.Item1, item.Item2);
                Debug.Log("Added Item: " + item.Item1.itemName + ", " + item.Item2 + "x");
            }

            Destroy(gameObject);
        }
    }
}
