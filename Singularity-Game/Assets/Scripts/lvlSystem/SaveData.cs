using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    //Stat based
    public int health;
    public List<bool> unlockedWeaponModes;
    public bool doubleJumpBoots;
    public List<int> invItemID = new List<int>();
    public List<int> invItemAmount = new List<int>();
    public float[] lastCheckpoint;

    public SaveData(Player player){
        health = player.currentHealth;
        unlockedWeaponModes = player.GetSavedWeaponModes();
        doubleJumpBoots = player.doubleJump;

        lastCheckpoint = new float[3];
        lastCheckpoint[0] = player.getCheckPoint().x;
        lastCheckpoint[1] = player.getCheckPoint().y;
        lastCheckpoint[2] = player.getCheckPoint().z;

        foreach(var item in player.inventory.stackedInventoryItems){
            invItemID.Add(item.Item1.id);
            invItemAmount.Add(item.Item2);
        }
    }
}
