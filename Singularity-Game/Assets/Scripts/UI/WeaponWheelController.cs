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
        Activate();
        ActivatedMode();
    }

    public void Selected(){
        activeModes++;
    }

    public void Deselected(){
        activeModes--;
    }

    private void ActivatedMode(){
        anim.SetBool("selected", activeModes > 0);
    }
    

    public void Activate(){
        anim.SetBool("active", weaponWheelUiCanvasGroup.alpha == 1);
    }
}
