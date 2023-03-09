using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelController : MonoBehaviour
{
    [SerializeField] private GameObject weaponWheelUi;
    private CanvasGroup weaponWheelUiCanvasGroup;
    private Animator anim;
    
    // Start is called before the first frame update
    void Start()
    {
        weaponWheelUiCanvasGroup = weaponWheelUi.GetComponent<CanvasGroup>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Activate();
    }

    public void Selected(){
        anim.SetBool("selected", true);
    }

    public void Deselected(){
        anim.SetBool("selected", false);
    }

    public void Activate(){
        anim.SetBool("active", weaponWheelUiCanvasGroup.alpha == 1);
    }
}
