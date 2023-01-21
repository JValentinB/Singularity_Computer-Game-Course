using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvDatabase : MonoBehaviour
{
    public List<InvItem> databaseItems = new List<InvItem>(){
        new InvItem(0, "Box", "This is just a Box", null),
        new InvItem(1, "CooleBox", "This is just a CooleBox", null)
    };
    

    public InvItem GetItem(int id){
        return databaseItems.Find(item => item.id == id);
    }
}