using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorytextControl : MonoBehaviour
{
    [SerializeField] private int charIndex, storyIndex;
    [SerializeField] public int storyPartIndex;
    private TMPro.TextMeshProUGUI headerField, textField;
    private AudioSource textSound; 
    private GameObject player;

    private List<(string, string)> spaceStoryText = new List<(string, string)>();
    private string finalText;
    private bool writing;

    void Start()
    {
        textSound = GetComponent<AudioSource>();
        headerField = transform.Find("InfoHeader").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        textField = transform.Find("InfoDescription").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        player = GameObject.FindWithTag("Player");

        GetComponent<CanvasGroup>().alpha = 0f;
        storyIndex = 0;
        storyPartIndex = -1;
    }


    //To add a text, add a string tuple
    //First item is the name, second is the text
    public void AddStoryText(){
        spaceStoryText.Clear();
        storyIndex = 0;

        switch (storyPartIndex)
        {
            case 0:
                spaceStoryText.Add(
                    ("Otter", "What the heck?! What happened?\n'Arion', can you hear me! Where are you?")
                );
                spaceStoryText.Add(
                    ("Arion", "...")
                );
                spaceStoryText.Add(
                    ("Otter", "Damn it. This looks bad.\nI gotta find 'Arion'.")
                );
                spaceStoryText.Add(
                    ("Otter", "Right side looks like a dead end, so left it is!")
                );
                break;
            case 1:
                break;
            case 2:
                spaceStoryText.Add(
                    ("Otter", "Test works!")
                );
                break;
            case 3:
                break;
        }
    }

    public bool CheckStoryRequirements(int nextStoryPart){
        switch (nextStoryPart)
        {
            case 2:
                return player.GetComponent<Player>().unlockedWeaponModes[0];
            default:
                return true;
        }
    }

    public IEnumerator PlayStory(){
        //Lock player input
        GetComponent<CanvasGroup>().alpha = 1f;
        NextText();
        storyIndex++;
        
        while(true){
            if(Input.GetKeyDown(KeyCode.Space)){
                NextText();
                storyIndex++;
            }
            yield return null;
        }
    }

    private void NextText(){
        GetComponent<CanvasGroup>().alpha = 1;
        if(storyIndex > spaceStoryText.Count || (storyIndex == spaceStoryText.Count && !writing)){ 
            StopCoroutine(PlayStory());
            GetComponent<CanvasGroup>().alpha = 0f;
            //Unlock player input
            return;
        }

        if(writing){
            storyIndex--;
            writing = false;
            charIndex = finalText.Length;
            headerField.text = spaceStoryText[storyIndex].Item1;
            textField.text = spaceStoryText[storyIndex].Item2;
        } else if(storyIndex < spaceStoryText.Count){
            textField.text = "";
            charIndex = 0;
            headerField.text = spaceStoryText[storyIndex].Item1;
            finalText = spaceStoryText[storyIndex].Item2;
            ReproduceText();
            writing = true;
        }  
    }

    private void ReproduceText()
    {
        //if not readied all letters
        if (charIndex < finalText.Length)
        {
            //get one letter
            char letter = finalText[charIndex];

            //Actualize on screen
            textField.text += letter;

            //Play text sound
            textSound.Play();

            //set to go to the next
            charIndex += 1;
            StartCoroutine(PauseBetweenChars(letter));
        } else {
            writing = false;
        }
    }

    private IEnumerator PauseBetweenChars(char letter)
    {
        switch (letter)
        {
            case '.':
                yield return new WaitForSeconds(0.1f);
                ReproduceText();
                yield break;
            case ',':
                yield return new WaitForSeconds(0.09f);
                ReproduceText();
                yield break;
            case ' ':
                yield return new WaitForSeconds(0.08f);
                ReproduceText();
                yield break;
            default:
                yield return new WaitForSeconds(0.05f);
                ReproduceText();
                yield break;
        }
    }
}
