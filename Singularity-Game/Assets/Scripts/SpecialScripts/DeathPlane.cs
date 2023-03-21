using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    public int damage = 99999;
    public float damageInterval = 1f;

    private Coroutine damageCoroutine;
    private bool damageTimeOut = false;

    void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<Damageable>() && !damageTimeOut)
        {
            damageCoroutine = StartCoroutine(makeDamage(col.GetComponent<Damageable>()));
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.GetComponent<Damageable>() && !damageTimeOut)
        {
            damageCoroutine = StartCoroutine(makeDamage(col.GetComponent<Damageable>()));
        }
    }

    IEnumerator makeDamage(Damageable damageable)
    {
        damageTimeOut = true;
        damageable.ApplyDamage(damage);
        yield return new WaitForSeconds(damageInterval);
        damageTimeOut = false;
    }
}
