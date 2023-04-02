using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotButton : MonoBehaviour
{
    [SerializeField] private WeaponWheelInfoController InfoTextPanel;
    [SerializeField] private Animator InfoTextAnimator;
    [SerializeField] private InvUI invUi;
    private int id;
    private GameObject slot;

    void Start(){
        slot = this.transform.parent.gameObject;
    }

    void Update(){
        GetId();
    }

    private void GetId(){
        id = invUi.GetItemIdInSlot(slot);
    }

    public void HoverEnter(){
        InfoTextPanel.SetText(id);
        InfoTextAnimator.SetBool("active", true);
    }

    public void HoverExit(){
        InfoTextAnimator.SetBool("active", false);
    }
}
