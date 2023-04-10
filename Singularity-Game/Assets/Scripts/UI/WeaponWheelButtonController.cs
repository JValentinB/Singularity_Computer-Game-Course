using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelButtonController : MonoBehaviour
{
    [SerializeField] private GameObject weaponWheelUi;
    [SerializeField] private WeaponWheelController upperPartBackgroundController;
    [SerializeField] private WeaponWheelController upperPartForegroundController;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Image image;
    [SerializeField] private Animator animOtherStone1, animOtherStone2, animOtherStone3;
    [SerializeField] private WeaponWheelInfoController InfoTextPanel;
    private Animator[] anims;
    private CanvasGroup weaponWheelUiCanvasGroup;
    public int id;
    private Animator anim;
    public float alphaThreshhold = 0.1f;
    private bool selected = false;
    private bool unlocked = false;
    // Start is called before the first frame update
    void Start()
    {
        anims = new Animator[]{animOtherStone1, animOtherStone2, animOtherStone3};
        weaponWheelUiCanvasGroup = weaponWheelUi.GetComponent<CanvasGroup>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        unlocked = uiManager.unlockedWeaponModes[id];
        ActivateAnimation();
        ActivateUpperAnimation();
    }

    public void Selected(){
        anim.SetBool("selected", true);
        foreach (Animator anim in anims)
        {
            anim.SetBool("selected", false);
        }
        selected = true;
        uiManager.modeId = id;
        uiManager.modeImage = image.sprite;
    }

    public void Deselected(){
        uiManager.modeId = 0;
    }

    public void HoverEnter(){
        anim.SetBool("hover", true);
        InfoTextPanel.SetText(id);
    }

    public void HoverExit(){
        anim.SetBool("hover", false);
        InfoTextPanel.SetText(-1);
    }

    public void ActivateAnimation(){
        var isActive = weaponWheelUiCanvasGroup.alpha == 1 && unlocked;
        anim.SetBool("active", isActive);
    }

    public void ActivateUpperAnimation(){
        if(selected){
            upperPartBackgroundController.Selected();
            upperPartForegroundController.Selected();
            selected = false;
        }
    }
}