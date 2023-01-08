using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InventoryUi : MonoBehaviour
{
    public GameObject inventory;
    public bool inventoryUiEnabled;

    private void ToggleInventoryUi(){
        if(Input.GetKeyDown(KeyCode.I) || (Input.GetKeyDown(KeyCode.Escape) && inventoryUiEnabled)){
            inventoryUiEnabled = !inventoryUiEnabled;
        }
        inventory.SetActive(inventoryUiEnabled);
    }
}