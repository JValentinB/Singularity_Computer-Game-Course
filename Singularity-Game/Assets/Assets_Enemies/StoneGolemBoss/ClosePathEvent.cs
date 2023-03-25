using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class ClosePathEvent : MonoBehaviour
{
    public float wall = -521.9175f; // x position of the wall

    private Transform player;
    private Rigidbody playerRigidbody;
    private bool playerInTrigger = false;

    private Rig rig;
    private Animator animator;
    private Vector3 velocity = Vector3.zero;
    private Coroutine fadeWeight;

    // Start is called before the first frame update
    void Start()
    {
        rig = transform.parent.GetComponent<Rig>();
        animator = transform.parent.parent.GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerRigidbody = player.GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (fadeWeight != null)
            {
                StopCoroutine(fadeWeight);
            }
            fadeWeight = StartCoroutine(fadingWeight(1f, 1f, true));
            StartCoroutine(Stampede());
            playerInTrigger = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (fadeWeight != null)
            {
                StopCoroutine(fadeWeight);
            }
            fadeWeight = StartCoroutine(fadingWeight(0f, 1f, false));
            playerInTrigger = false;
        }
    }

    IEnumerator fadingWeight(float endWeight, float duration, bool end)
    {
        float currentWeight = rig.weight;
        float time = 0;
        while (time < duration)
        {
            rig.weight = Mathf.Lerp(currentWeight, endWeight, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        rig.weight = endWeight;
        playerInTrigger = end;
    }

    // Use AnimationCurve to control weight of Rig
    public IEnumerator weightCurve(AnimationCurve curve, float length)
    {
        float time = 0;
        while (time < length)
        {
            rig.weight = curve.Evaluate(time / length);
            time += Time.deltaTime;
            yield return null;
        }
        rig.weight = curve.Evaluate(1);
    }

    IEnumerator Stampede()
    {
        yield return new WaitForSeconds(1.5f);
        animator.SetLayerWeight(1, 1.5f);
        animator.Play("GolemStomp", 1, 0);

        yield return new WaitForSeconds(0.5f);
        animator.SetLayerWeight(1, 0);
    }
}

