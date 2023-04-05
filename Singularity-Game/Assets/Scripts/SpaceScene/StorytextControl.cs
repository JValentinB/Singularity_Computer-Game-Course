using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StorytextControl : MonoBehaviour
{
    [SerializeField] private int storyIndex, charIndex;
    private GameObject Spaceship;
    private ShipControl ShipScript;
    private TMPro.TextMeshProUGUI headerField, textField;
    private AudioSource textSound;

    private List<(string, string)> spaceStoryText = new List<(string, string)>();
    private string finalText;
    private bool writing;
    public bool startStory;

    void Start()
    {
        Spaceship = GameObject.FindWithTag("Spaceship");
        ShipScript = Spaceship.GetComponent<ShipControl>();
        textSound = GetComponent<AudioSource>();
        headerField = transform.Find("InfoHeader").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        textField = transform.Find("InfoDescription").gameObject.GetComponent<TMPro.TextMeshProUGUI>();

        GetComponent<CanvasGroup>().alpha = 0f;
        AddStoryText();
        storyIndex = 0;
    }

    void Update()
    {
        PlayStory();
    }

    //To add a text, add a string tuple
    //First item is the name, second is the text
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
        spaceStoryText.Add(
            ("Otter", "Next date? What are you talking about?")
        );
        spaceStoryText.Add(
            ("Arion", "2 days ago you matched with User 'SweetAngel_93' on your partner finding application called 'Gravitynder'.\nI took the liberty to arrange a date for you. You would have messed it up anyway.")
        );
        spaceStoryText.Add(
            ("Otter", "What?! Why didn't you tell me!")
        );
        spaceStoryText.Add(
            ("Arion", "Well, you didn't ask.")
        );
        spaceStoryText.Add(
            ("Otter", "I should have sold you to the scrap dealer on Tardus...\n'Arion', change the course to the location of our Tinder match.")
        );
        spaceStoryText.Add(
            ("Arion", "Changing course to 'Othrys'.")
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
        if(storyIndex > spaceStoryText.Count || (storyIndex == spaceStoryText.Count && !writing)){ 
            StartCoroutine(ContinueGame());
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

    private IEnumerator ContinueGame(){
        ShipScript.lockPlayerControl = true;
        GetComponent<CanvasGroup>().alpha = 0f;
        GameObject.FindWithTag("SpaceCamera").GetComponent<CameraControlSpace>().followPlayer = false;

        yield return new WaitForSeconds(2f);
        
        var shipTargetPosY = Spaceship.transform.position.y + 20f;

        while(shipTargetPosY >= Spaceship.transform.position.y){
            Spaceship.transform.Translate(Vector3.down * Spaceship.GetComponent<ShipControl>().SpaceShipSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("Ready to move Scenes!");
        SceneManager.LoadScene("Forest1.0_Valentin");
    }
}