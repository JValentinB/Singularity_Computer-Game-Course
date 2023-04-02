using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public int currentHealth, maxHealth;
    private float lerpTimer, delayTimer;
    public Image frontBar, backBar;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = 1;
        currentHealth = maxHealth;
        frontBar.fillAmount = currentHealth/maxHealth;
        backBar.fillAmount = currentHealth/maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthUi();
    }

    public void UpdateHealthUi(){
        float healthPercentage = (float)currentHealth/(float)maxHealth;
        float healthFillAmount = frontBar.fillAmount;
        if(healthFillAmount < healthPercentage){
            delayTimer += Time.deltaTime;
            backBar.fillAmount = healthPercentage;
            if(delayTimer > 3){
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer/4;
                frontBar.fillAmount = Mathf.Lerp(healthFillAmount, backBar.fillAmount, percentComplete);
            }

        }
    }
}
