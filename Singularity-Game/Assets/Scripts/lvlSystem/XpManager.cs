using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XpManager : MonoBehaviour
{
    public int currentXp, targetXp, lvl;
    private float lerpTimer, delayTimer;
    public Image frontBar, backBar;

    void Start(){
        lvl = 1;
        targetXp = (int)Mathf.Floor(lvl + 300 * Mathf.Pow(2, lvl/7))/4;
        frontBar.fillAmount = currentXp/targetXp;
        backBar.fillAmount = currentXp/targetXp;
    }

    void Update(){
        UpdateXpUi();
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
        lerpTimer = 0f;
        delayTimer = 0f;
        while(currentXp >= targetXp){
            frontBar.fillAmount = 0f;
            backBar.fillAmount = 0f;
            Debug.Log("lvl up!");
            lvl++;
            currentXp -= targetXp;
            targetXp += (int)Mathf.Floor(lvl + 300 * Mathf.Pow(2, lvl/7))/4;
        }
    }

    public void UpdateXpUi(){
        float xpPercentage = (float)currentXp/(float)targetXp;
        float xpFillAmount = frontBar.fillAmount;
        if(xpFillAmount < xpPercentage){
            delayTimer += Time.deltaTime;
            backBar.fillAmount = xpPercentage;
            if(delayTimer > 3){
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer/4;
                frontBar.fillAmount = Mathf.Lerp(xpFillAmount, backBar.fillAmount, percentComplete);
            }

        }
        
        
    }
}