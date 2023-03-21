using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class RigControlTreeBoss : MonoBehaviour
{
    public float smoothTime = 0.3f;

    private Transform player;
    private Transform playerFollower;
    private bool playerInTrigger = false;

    private Rig rig;
    private Vector3 velocity = Vector3.zero;
    private Coroutine fadeWeight;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rig>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerFollower = transform.Find("PlayerFollower");
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInTrigger)
        {
            playerFollower.position = Vector3.SmoothDamp(playerFollower.position, player.position, ref velocity, smoothTime);
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {   
            if (fadeWeight != null)
            {
                StopCoroutine(fadeWeight);
            }
            fadeWeight = StartCoroutine(fadingWeight(1, 2f, true));
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
            fadeWeight = StartCoroutine(fadingWeight(0, 2f, false));
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
}
