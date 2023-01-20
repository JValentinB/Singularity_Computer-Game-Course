using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class fade_to_black : MonoBehaviour
{
    public GameObject BlackOutSquare;

    // Update is called once per frame
    void Update()
    {
 
    }

    public IEnumerator FadeBlackOutSquare(bool fadeToBlack = true, float fadespeed = 1f)
    {
        BlackOutSquare = GameObject.Find("BlackOutSquare");
        Color objectColor = BlackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        while(BlackOutSquare.GetComponent<Image>().color.a < 1)
            {
                fadeAmount = objectColor.a + (fadespeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                BlackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        //yield return new WaitForSeconds(1);
        while (BlackOutSquare.GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadespeed/2 * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                BlackOutSquare.GetComponent<Image>().color = objectColor;
                yield return null;
            }
        
    }
}
