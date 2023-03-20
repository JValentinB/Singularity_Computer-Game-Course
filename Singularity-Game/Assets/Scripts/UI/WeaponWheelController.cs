using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelController : MonoBehaviour
{
    [SerializeField] private GameObject weaponWheelUi;
    [SerializeField] private int activeModes;
    private CanvasGroup weaponWheelUiCanvasGroup;
    private Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        activeModes = 0;
        weaponWheelUiCanvasGroup = weaponWheelUi.GetComponent<CanvasGroup>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        ActivateAnimation();
    }

    public void Selected(){
        anim.SetBool("selected", true);
    }

    public void ActivateAnimation(){
        anim.SetBool("active", weaponWheelUiCanvasGroup.alpha == 1);
    }
}
