using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AttackStrength : MonoBehaviour
{
    public Slider AttackSlider;

    public void AttackInit(int attackStrength)
    {
        slider.maxValue = attackStrength;
        slider.value = 0;
    }

    public void SetAttack(int attack)
    {
        slider.value = attack;
    }
}
