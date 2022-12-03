using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public int currentAttack = 0;
    public int maxAttack = 4;
    private float attackTimer = 0;

    public AttackStrength attack; 
    // Start is called before the first frame update
    void Start()
    {
        attack.AttackInit(maxAttack);
        StartCoroutine(waiter());
    }

    // Update is called once per frame
    void Update()
    {
        LoadAttack(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.layer == 6)
        {
            other.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void LoadAttack()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            attackTimer += Time.deltaTime;
            currentAttack = (int) Mathf.Floor(attackTimer);
            attack.SetAttack(currentAttack);
            waiter();
        }
    }

    private void RealeaseAttack()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            attackTimer = 0;
            currentAttack
        }
    }

    IEnumerator waiter()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(3);
    }
}
