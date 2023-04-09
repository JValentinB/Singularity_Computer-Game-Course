using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterEnemyControl : MonoBehaviour
{
    [SerializeField] public int maxHealth, currentHealth;
    public float targetDistanceToShip;
    private Vector3 initPos;
    public bool syncToShip, init;

    private GameObject Spaceship;
    [SerializeField] private GameObject explosionObject;

    void Start()
    {
        init = true;
        maxHealth = 100;
        currentHealth = maxHealth;

        Spaceship = GameObject.FindWithTag("Spaceship");
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(Initialize());
        /* StartCoroutine(LookAtShip()); */
        Move();
    }

    private void Move(){
        if(!syncToShip && init) return;

        //transform.Translate(velocity * Time.deltaTime, Space.World);
    }

    private IEnumerator Initialize(){
        if(!init) yield break;
        
        while(targetDistanceToShip > Mathf.Abs(transform.position.y - Spaceship.transform.position.y)){
            transform.Translate(Vector3.right * 5f * Time.deltaTime, Space.World);
            yield return null;
        }

        init = false;
        syncToShip = true;
    }

    /* private IEnumerator LookAtShip(){
        if(init) yield break;

        Quaternion lookRotation = Quaternion.LookRotation(Spaceship.transform.position - transform.position);

        float time = 0f;
        while(time < 1){
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            time += Time.deltaTime * 1f;

            yield return null;
        }
    } */

    private void OnDeath(){
        var explosion = Instantiate(explosionObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col){
        if(col.tag == "Spaceship"){
            Debug.Log("ship collision!");
            //Make damage effect on spaceship
            OnDeath();
        } else if(col.tag == "Lasershot"){
            Debug.Log("laser collision!");
            currentHealth -= 20;
            if(currentHealth <= 0) OnDeath();
        }
    }
}
