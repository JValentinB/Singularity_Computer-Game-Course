using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreditsControl : MonoBehaviour
{ 
    [SerializeField] private bool startCredits;
    private AudioSource nameFieldSound;
    private List<string> creditRoles = new List<string>(){
        "Programming",
        "Game Design",
        "Audio Design",
        "Story",
        "Cinematics",
        "Special Thanks"
    };
    private List<List<string>> credits = new List<List<string>>();

    void Start()
    {
        nameFieldSound = GetComponent<AudioSource>();

        AddCreditsToList();
        InitializeTextFields();
        StartCoroutine(FadeIntoScene());
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(PlayCredits());
    }

    private void AddCreditsToList(){
        credits.Add( new List<string>(){
        "Timur",
        "Valentin",
        "Kevin",
        "Yunus",
        "Joshua",
        "Misheel"
        } );
        
        credits.Add( new List<string>(){
        "ChatGPT",
        "ChatGPT",
        "ChatGPT",
        "ChatGPT",
        "ChatGPT",
        "ChatGPT"
        } );

        credits.Add( new List<string>(){
        "Does",
        "Not",
        "Exist",
        "In",
        "The",
        "Game"
        } );

        credits.Add( new List<string>(){
        "No",
        "One",
        "Special",
        "To",
        "Thank",
        "To"
        } );
    }

    private void InitializeTextFields(){
        if(GameObject.FindWithTag("CreditRoleField").GetComponent<TMP_Text>()) Debug.Log("Not there");
        var roleFieldTmp = GameObject.FindWithTag("CreditRoleField").GetComponent<TMP_Text>();
        roleFieldTmp.color = new Color(roleFieldTmp.color.r, roleFieldTmp.color.g, roleFieldTmp.color.b, 0f);
        
        foreach(var nameField in GameObject.FindGameObjectsWithTag("CreditNameField")){
            var nameFieldTmp = nameField.GetComponent<TMP_Text>();
            nameFieldTmp.color = new Color(nameFieldTmp.color.r, nameFieldTmp.color.g, nameFieldTmp.color.b, 0f);
        }
    }

    private IEnumerator FadeIntoScene(){
        /* while(true){
            nameFieldSound.Play();
            yield return new WaitForSeconds(0.3f);
        } */
        yield break;
    }

    private IEnumerator PlayCredits(){
        if(!startCredits) yield break;
        startCredits = false;

        var roleField = GameObject.FindWithTag("CreditRoleField");
        var nameFields = GameObject.FindGameObjectsWithTag("CreditNameField");
        foreach(var role in creditRoles){

            //Fade in credit role
            var duration = 2f;
            var timer = 0f;
            roleField.GetComponent<TMP_Text>().text = role;
            var roleFieldTmp = roleField.GetComponent<TMP_Text>();

            while(duration > timer){
                roleFieldTmp.color = new Color(roleFieldTmp.color.r, roleFieldTmp.color.g, roleFieldTmp.color.b, Mathf.Lerp(0f, 1f, timer/duration));
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            //Fade in credit names
            List<int> usedIndex = new List<int>();
            
            for(int i = 0; i < credits[0].Count; i++){
                int randIndex = UnityEngine.Random.Range(0, credits[0].Count-1);
                while(usedIndex.Contains(randIndex)) randIndex = UnityEngine.Random.Range(0, credits[0].Count);
                usedIndex.Add(randIndex);

                duration = 0.2f;
                timer = 0f;

                if(role == "Story") nameFields[randIndex].GetComponent<TMP_Text>().text = credits[1][i];
                else if(role == "Cinematics") nameFields[randIndex].GetComponent<TMP_Text>().text = credits[2][i];
                else if(role == "Special Thanks") nameFields[randIndex].GetComponent<TMP_Text>().text = credits[3][i];
                else nameFields[randIndex].GetComponent<TMP_Text>().text = credits[0][i];

                var nameFieldTmp = nameFields[randIndex].GetComponent<TMP_Text>();

                nameFieldSound.Play();

                while(duration > timer){
                    nameFieldTmp.color = new Color(nameFieldTmp.color.r, nameFieldTmp.color.g, nameFieldTmp.color.b, Mathf.Lerp(0f, 1f, timer/duration));
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
            usedIndex.Clear();

            yield return new WaitForSeconds(3f);

            
            //Turn credit names invisible
            foreach(var nameField in GameObject.FindGameObjectsWithTag("CreditNameField")){
                var nameFieldTmp = nameField.GetComponent<TMP_Text>();
                nameFieldTmp.color = new Color(nameFieldTmp.color.r, nameFieldTmp.color.g, nameFieldTmp.color.b, 0f);
            }

            yield return new WaitForSeconds(1f);

            //Fade out credit role
            duration = 2f;
            timer = 0f;
            while(duration > timer){
                roleFieldTmp.color = new Color(roleFieldTmp.color.r, roleFieldTmp.color.g, roleFieldTmp.color.b, Mathf.Lerp(1f, 0f, timer/duration));
                timer += Time.deltaTime;
                yield return null;
            }
        }

        StartCoroutine(EndCredits());
    }

    private IEnumerator BeGoneCredits(GameObject nameField){
        var nameFieldTmp = nameField.GetComponent<TMP_Text>();
        float randDegree = UnityEngine.Random.Range(0f, 360f);

        nameFieldTmp.color = new Color(nameFieldTmp.color.r, nameFieldTmp.color.g, nameFieldTmp.color.b, 0f);

        yield break;
    }

    private IEnumerator EndCredits(){
        yield break;
    }
}
