using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvDatabase
{
    public List<InvItem> databaseItems = new List<InvItem>(){
        new InvItem(0, "0", "0", null),
        new InvItem(1, "1", "1", null),
        new InvItem(2, "2", "2", null)
};
    

    public InvItem GetItem(int id){
        return databaseItems.Find(item => item.id == id);
    }
}