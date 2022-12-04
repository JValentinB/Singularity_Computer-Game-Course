using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField ]GameObject player;
    [SerializeField] int damagePerHit = 40;
    public int currentAttack = 0;
    public int maxAttack = 100;
    private float attackTimer = 0;
    private GameObject activeBar;

    public AttackStrength attack; 
    // Start is called before the first frame update
    void Start()
    {
        activeBar = GameObject.Find("AttackStrengthBar");
        activeBar.SetActive(false);
        attack.AttackInit(maxAttack);
        StartCoroutine(waiter());
    }

    // Update is called once per frame
    void Update()
    {
        LoadAttack();
        ReleaseAttack();
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (other.gameObject.layer == 6 && player.GetComponent<Animator>().GetInteger("Attack") > 0)
        {
            enemy.takeDamage(damagePerHit);
            enemy.tookDamage = true;
            other.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    private void LoadAttack()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            activeBar.SetActive(true);
            attackTimer += Time.deltaTime;
            currentAttack = (int) Mathf.Floor(attackTimer);
            attack.SetAttack(currentAttack);
            //waiter();
        }
    }

    private void ReleaseAttack()
    {
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            EnemyPull();
            attackTimer = 0;
            currentAttack = 0;
            attack.SetAttack(currentAttack);
            activeBar.SetActive(false);
        }
    }

    void EnemyPull()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward));
        LayerMask hitLayer = LayerMask.NameToLayer("Enemy");

        Debug.DrawRay(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward), Color.red, 5);

        float sphereRadius = 2;
        float maxDistance = 20;
        int layerMask = (1 << hitLayer);
        if (Physics.SphereCast(ray, sphereRadius, out hit, maxDistance, layerMask))
        {
            Debug.Log("There is an enemy!");
            Rigidbody enemy = hit.rigidbody;
            enemy.AddForce(transform.TransformDirection(Vector3.back) * 50 * currentAttack);
        }
        else
        {
            Debug.Log("There is no enemy!");
        }

    }


    IEnumerator waiter()
    {
        //yield on a new YieldInstruction that waits for 5 seconds.
        yield return new WaitForSeconds(3);
    }
}
