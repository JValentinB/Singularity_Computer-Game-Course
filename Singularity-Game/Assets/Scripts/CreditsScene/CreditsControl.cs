using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsControl : MonoBehaviour
{ 
    [SerializeField] private bool startCredits;
    private GameObject Spaceship;
    private TMP_Text thanksTmp, thanksTmp1, thanksTmp2, fuckoffTmp;
    private AudioSource nameFieldSound;
    private List<string> credits = new List<string>();
    private List<string> creditRoles = new List<string>(){
        "Singularity Group",
        "Additional Art",
    };

    void Start()
    {
        Spaceship = GameObject.FindWithTag("Spaceship");
        Spaceship.GetComponent<ShipControl>().lockPlayerControl = true;
        nameFieldSound = GetComponent<AudioSource>();
        thanksTmp = transform.Find("ThanksForPlaying").gameObject.GetComponent<TMP_Text>();
        thanksTmp1 = transform.Find("ThanksForPlaying (1)").gameObject.GetComponent<TMP_Text>();
        thanksTmp2 = transform.Find("ThanksForPlaying (2)").gameObject.GetComponent<TMP_Text>();
        fuckoffTmp = transform.Find("FuckOffField").gameObject.GetComponent<TMP_Text>();
        GetComponent<Image>().color = new Color(0f, 0f, 0f, 255f);

        AddCreditsToList();
        InitializeTextFields();
        StartCoroutine(FadeBlackOutSquare(false));
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(PlayCredits());
    }

    private void AddCreditsToList(){
        credits.Add( "Valentin Blum\n\nTimur Aydin\n\nKevin Plessing\n\nJoshua Tardus\n\nYunus Saracoglu\n\nMisheel Ganbold" );
        
        credits.Add( "Triki Minut\n\nTamino Vay\n\nCGPitbull" );
    }

    private void InitializeTextFields(){
        thanksTmp.color = new Color(thanksTmp.color.r, thanksTmp.color.g, thanksTmp.color.b, 0f);
        thanksTmp1.color = new Color(thanksTmp.color.r, thanksTmp.color.g, thanksTmp.color.b, 0f);
        thanksTmp2.color = new Color(thanksTmp.color.r, thanksTmp.color.g, thanksTmp.color.b, 0f);
        fuckoffTmp.color = new Color(fuckoffTmp.color.r, fuckoffTmp.color.g, fuckoffTmp.color.b, 0f);
        var roleFieldTmp = GameObject.FindWithTag("CreditRoleField").GetComponent<TMP_Text>();
        roleFieldTmp.color = new Color(roleFieldTmp.color.r, roleFieldTmp.color.g, roleFieldTmp.color.b, 0f);
        
        foreach(var nameField in GameObject.FindGameObjectsWithTag("CreditNameField")){
            var nameFieldTmp = nameField.GetComponent<TMP_Text>();
            nameFieldTmp.color = new Color(nameFieldTmp.color.r, nameFieldTmp.color.g, nameFieldTmp.color.b, 0f);
        }
    }

    private IEnumerator PlayCredits(){
        if(!startCredits) yield break;
        startCredits = false;

        yield return new WaitForSeconds(0.5f);

        var duration = 2f;
        var timer = 0f;

        while(duration > timer){
            thanksTmp.color = new Color(thanksTmp.color.r, thanksTmp.color.g, thanksTmp.color.b, Mathf.Lerp(0f, 1f, timer/duration));
            thanksTmp1.color = new Color(thanksTmp.color.r, thanksTmp.color.g, thanksTmp.color.b, Mathf.Lerp(0f, 1f, timer/duration));
            thanksTmp2.color = new Color(thanksTmp.color.r, thanksTmp.color.g, thanksTmp.color.b, Mathf.Lerp(0f, 1f, timer/duration));
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        timer = 0f;
        while(duration > timer){
            thanksTmp.color = new Color(thanksTmp.color.r, thanksTmp.color.g, thanksTmp.color.b, Mathf.Lerp(1f, 0f, timer/duration));
            thanksTmp1.color = new Color(thanksTmp.color.r, thanksTmp.color.g, thanksTmp.color.b, Mathf.Lerp(1f, 0f, timer/duration));
            thanksTmp2.color = new Color(thanksTmp.color.r, thanksTmp.color.g, thanksTmp.color.b, Mathf.Lerp(1f, 0f, timer/duration));
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        var roleField = GameObject.FindWithTag("CreditRoleField");
        var nameFields = GameObject.FindGameObjectsWithTag("CreditNameField");
        foreach(var role in creditRoles){
            roleField.GetComponent<TMP_Text>().text = role;
            var roleFieldTmp = roleField.GetComponent<TMP_Text>();

            var nameFieldTmp = nameFields[0].GetComponent<TMP_Text>();
            if(role == "Singularity Group") nameFields[0].GetComponent<TMP_Text>().text = credits[0];
            else if(role == "Additional Art") nameFields[0].GetComponent<TMP_Text>().text = credits[1];

            //Fade in credit role
            duration = 1f;
            timer = 0f;

            while(duration > timer){
                roleFieldTmp.color = new Color(roleFieldTmp.color.r, roleFieldTmp.color.g, roleFieldTmp.color.b, Mathf.Lerp(0f, 1f, timer/duration));
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            duration = 1f;
            timer = 0f;
            while(duration > timer){
                nameFieldTmp.color = new Color(nameFieldTmp.color.r, nameFieldTmp.color.g, nameFieldTmp.color.b, Mathf.Lerp(0f, 1f, timer/duration));
                timer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(2f);

            duration = 1f;
            timer = 0f;
            while(duration > timer){
                nameFieldTmp.color = new Color(nameFieldTmp.color.r, nameFieldTmp.color.g, nameFieldTmp.color.b, Mathf.Lerp(1f, 0f, timer/duration));
                roleFieldTmp.color = new Color(roleFieldTmp.color.r, roleFieldTmp.color.g, roleFieldTmp.color.b, Mathf.Lerp(1f, 0f, timer/duration));
                timer += Time.deltaTime;
                yield return null;
            }
        }

        StartCoroutine(EndCredits());
    }

    private IEnumerator EndCredits(){

        Spaceship.GetComponent<ShipControl>().lockPlayerControl = false;

        var duration = 1f;
        var timer = 0f;
        while(duration > timer){
            if(IsShipOffScreen()){
                SceneManager.LoadScene("Space");
                yield break;
            }
            fuckoffTmp.color = new Color(fuckoffTmp.color.r, fuckoffTmp.color.g, fuckoffTmp.color.b, Mathf.Lerp(0f, 1f, timer/duration));
            timer += Time.deltaTime;
            yield return null;
        }

        while(true){
            if(IsShipOffScreen()){
                SceneManager.LoadScene("Space");
                yield break;
            }
            yield return null;
        }
        yield break;
    }

    private bool IsShipOffScreen(){
        if(Spaceship.transform.position.x >= 15f || Spaceship.transform.position.x <= -15f 
        || Spaceship.transform.position.y >= 9f || Spaceship.transform.position.y <= -9f){
            return true;
        }
        return false;
    }

    //Not used anymore
    private IEnumerator BeGoneCredits(GameObject nameField){
        var nameFieldTmp = nameField.GetComponent<TMP_Text>();
        float randDegree = UnityEngine.Random.Range(-60f, 60f);
        Vector3 eulerAngle = nameFieldTmp.rectTransform.eulerAngles;
        Vector3 oldPos = nameFieldTmp.rectTransform.position;

        var duration = 0.7f;
        var timer = 0f;
        var textSpeed = 800f;
        while(duration > timer){
            eulerAngle = new Vector3(0f, 0f, Mathf.SmoothStep(nameFieldTmp.rectTransform.rotation.z, randDegree, timer/duration));
            nameFieldTmp.rectTransform.eulerAngles = eulerAngle;
            if(nameFieldTmp.rectTransform.localPosition.x < 0) nameFieldTmp.rectTransform.Translate(Vector3.left * textSpeed * Time.deltaTime);
            else nameFieldTmp.rectTransform.Translate(Vector3.right * textSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        duration = 0.5f;
        timer = 0f;

        nameFieldTmp.color = new Color(nameFieldTmp.color.r, nameFieldTmp.color.g, nameFieldTmp.color.b, 0f);
        nameFieldTmp.rectTransform.localPosition = oldPos;
        nameFieldTmp.rectTransform.eulerAngles = Vector3.zero;
        yield break;
    }

    private IEnumerator FadeBlackOutSquare(bool fadeToBlack = true, float fadespeed = 1f)
    {
        Color objectColor = GetComponent<Image>().color;
        float fadeAmount;
        if (fadeToBlack)
        {
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, 0f);
            while (GetComponent<Image>().color.a < 1)
            {
                fadeAmount = objectColor.a + (fadespeed * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
        else
        {
            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, 1f);
            while (GetComponent<Image>().color.a > 0)
            {
                fadeAmount = objectColor.a - (fadespeed / 2 * Time.deltaTime);

                objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
                GetComponent<Image>().color = objectColor;
                yield return null;
            }
        }
        startCredits = true;
    }
}
