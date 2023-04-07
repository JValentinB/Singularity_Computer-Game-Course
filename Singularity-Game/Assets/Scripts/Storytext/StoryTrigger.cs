using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryTrigger : MonoBehaviour
{
    [Header("Choose which part the collider should show")]
    [SerializeField] private int storyPartIndex;
    [SerializeField] private bool storyShown;
    [SerializeField] private bool oneTimePlay;
    private StorytextControl storyController;

    // Update is called once per frame
    void Start()
    {
        storyController = GameObject.FindWithTag("StoryField").GetComponent<StorytextControl>();
    }

    void OnTriggerStay(Collider col){
        if(!storyShown && col.tag == "Player" && storyController.CheckStoryRequirements(storyPartIndex)){
            storyShown = true;
            storyController.storyPartIndex = storyPartIndex;
            storyController.AddStoryText();
            StartCoroutine(storyController.PlayStory());
        }
    }

    void OnTriggerExit(Collider col){
        if(!storyShown && col.tag == "Player" && storyController.CheckStoryRequirements(storyPartIndex)){
            if(oneTimePlay) return;
            storyShown = false;
        }
    }
}
