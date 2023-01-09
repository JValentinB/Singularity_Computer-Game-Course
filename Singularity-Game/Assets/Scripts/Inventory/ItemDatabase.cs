using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    public List<InventoryItem> databaseItems = new List<InventoryItem>(){
        new InventoryItem(0, "Box", "This is just a Box", null),
        new InventoryItem(1, "CooleBox", "This is just a CooleBox", null)
    };
    

    public InventoryItem GetItem(int id){
        return databaseItems.Find(item => item.id == id);
    }
}