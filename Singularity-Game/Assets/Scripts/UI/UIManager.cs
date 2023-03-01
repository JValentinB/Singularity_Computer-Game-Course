using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private CanvasGroup invUI;

    void Start(){
        invUI.blocksRaycasts = false;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.I) || (invUI.alpha == 1 && Input.GetKeyDown(KeyCode.Escape)))
        {
            OpenCloseInventory();
        }
    }

    public void OpenCloseInventory()
    {
        invUI.alpha = invUI.alpha == 1 ? 0 : 1;
        invUI.blocksRaycasts = invUI.blocksRaycasts == true ? false : true;
    }

}