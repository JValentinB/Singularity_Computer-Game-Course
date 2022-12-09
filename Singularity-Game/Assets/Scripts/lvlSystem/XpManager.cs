using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XpManager : MonoBehaviour
{
    public int currentXp, targetXp, lvl;

    void Start(){
        lvl = 1;
        targetXp = (int)Mathf.Floor(lvl + 300 * Mathf.Pow(2, lvl/7))/4;
    }

    void Update(){
    }
    
    public static XpManager instance;

    private void Awake(){
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void GainXp(int xp){
        currentXp += xp;
        Debug.Log("Give Player " + xp + "XP!");
        while(currentXp >= targetXp){
            Debug.Log("lvl up!");
            lvl++;
            currentXp -= targetXp;
            targetXp += (int)Mathf.Floor(lvl + 300 * Mathf.Pow(2, lvl/7))/4;
        }
    }
}