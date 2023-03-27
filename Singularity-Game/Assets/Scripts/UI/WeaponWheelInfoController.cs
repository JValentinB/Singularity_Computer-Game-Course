using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelInfoController : MonoBehaviour
{
    [SerializeField] private List<string> modeNames;
    [SerializeField] private List<string> modeDescriptions;
    [SerializeField] private GameObject infoHeader;
    [SerializeField] private GameObject infoDescription;
    private CanvasGroup canvasGroup;
    private InvDatabase invDatabase; 

    void Start(){
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        invDatabase = new InvDatabase();
        modeNames = new List<string>();
        modeDescriptions = new List<string>();
        for(var i = 0; i < 4; i++){
            InvItem modeItem = invDatabase.GetItem(i);
            Debug.Log(modeItem.itemName);
            modeNames.Add(modeItem.itemName);
            modeDescriptions.Add(modeItem.description);
        }
    }

    public void SetText(int modeId){
        if(modeId != -1){
            infoHeader.GetComponent<TMPro.TextMeshProUGUI>().text = modeNames[modeId];
            infoDescription.GetComponent<TMPro.TextMeshProUGUI>().text = modeDescriptions[modeId];
            canvasGroup.alpha = 1;
        } else {
            canvasGroup.alpha = 0;
            infoHeader.GetComponent<TMPro.TextMeshProUGUI>().text = "";
            infoDescription.GetComponent<TMPro.TextMeshProUGUI>().text = "Hover over a crystal for more information.";
        }
    }
}