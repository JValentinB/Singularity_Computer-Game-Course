using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StorytextControl : MonoBehaviour
{
    private TMPro.TextMeshProUGUI headerField, textField;
    private List<(string, string)> spaceStoryText = new List<(string, string)>();
    private string finalText;
    private bool writing;
    [SerializeField] private int storyIndex, charIndex;
    public bool startStory;
    private GameObject Spaceship;
    private ShipControl ShipScript;

    void Start()
    {
        Spaceship = GameObject.FindWithTag("Spaceship");
        ShipScript = Spaceship.GetComponent<ShipControl>();

        GetComponent<CanvasGroup>().alpha = 0f;
        headerField = transform.Find("InfoHeader").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        textField = transform.Find("InfoDescription").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        
        AddStoryText();
        storyIndex = 0;
    }

    void Update()
    {
        PlayStory();
    }

    private void AddStoryText(){
        spaceStoryText.Add(
            ("Otter", "'Arion', give me a status report of our ship and our current course.")
        );
        spaceStoryText.Add(
            ("Arion", "All is good, everything is fine.")
        );
        spaceStoryText.Add(
            ("Otter", "Oh how helpful. Thank you very much 'Arion'.")
        );
        spaceStoryText.Add(
            ("Arion", "Fine... Ships vitals are at 100%. We have sufficient fuel for our course including an optional trip to the next Spacediner.\nOur storage contains 345 kg of 'Gravitanium crystals'.\nNone of them are damaged.")
        );
        spaceStoryText.Add(
            ("Otter", "Sounds good but I would rather drink from the toilet than eating at an outer rim Spacediner.")
        );
        spaceStoryText.Add(
            ("Arion", "But it may be a good place for your next date.")
        );

    }

    private void PlayStory(){
        if(!startStory || storyIndex < 0 || spaceStoryText.Count <= 0) return;

        if(Input.GetKeyDown(KeyCode.Space)){
            NextText();
            storyIndex++;
        }
    }

    private void NextText(){
        GetComponent<CanvasGroup>().alpha = 1;

        if(writing){
            storyIndex--;
            writing = false;
            charIndex = finalText.Length;
            headerField.text = spaceStoryText[storyIndex].Item1;
            textField.text = spaceStoryText[storyIndex].Item2;
        } else if(storyIndex == spaceStoryText.Count) {
            ContinueGame();
        } else {
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

    private void ContinueGame(){
        ShipScript.lockPlayerControl = false;
        GameObject.FindWithTag("SpaceCamera").GetComponent<CameraControlSpace>().followPlayer = true;
    }
}