using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rootSpike : MonoBehaviour
{   
    public int damage = 30;
    public float growSize = 1f;
    public float rumblingTime = 2f;
    public float growTime = 0.2f;
    public float waitTime = 0.5f;
    public float retractionTime = 0.5f;

    private Animator animator;
    private ParticleSystem ps;
    Collider spikeCollider;
    ObjectSounds objectSounds;

    Vector3 localScale;
    Vector3 startPosition;
    private bool started = false;
    // private bool isGrowing = false;

    [HideInInspector] public Vector3 growingDirection;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        ps = GetComponentInChildren<ParticleSystem>();
        spikeCollider = GetComponentInChildren<Collider>();
        objectSounds = GetComponent<ObjectSounds>();

        spikeCollider.enabled = false;

        float randomScale = Random.Range(0.5f, 1f);
        localScale = transform.localScale * randomScale;
        transform.localScale = localScale * 0.1f;

        startPosition = transform.position;

        growSize *= randomScale;
    }

    void Update(){
        if(!started){
            started = true;
            StartCoroutine(GrowRootSpike());
            objectSounds.setRandomSourcePitch("Growing", 0.1f);
        }
    }

    private IEnumerator GrowRootSpike()
    {
        ps.Play();
        StartCoroutine(objectSounds.PlayForTime("Crumbling", 0.1f, rumblingTime));

        yield return new WaitForSeconds(rumblingTime);

        objectSounds.Play("Growing");
        StartCoroutine(rootScale(0.1f, 1f));

        yield return new WaitForSeconds(growTime / 4);

        spikeCollider.enabled = true;
    }

    IEnumerator rootScale(float startScale, float endScale)
    {
        // isGrowing = true;

        transform.localScale = localScale * startScale;

        float time = 0f;
        while (time < growTime)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(startScale, endScale, time / growTime);
            transform.localScale = localScale * scale;

            if (growingDirection == Vector3.up)
            {
                float position = Mathf.Lerp(startPosition.y, startPosition.y + growSize, time / growTime);
                transform.position = new Vector3(transform.position.x, position, transform.position.z);
            }
            else if(growingDirection == Vector3.down){
                float position = Mathf.Lerp(startPosition.y, startPosition.y - growSize, time / growTime);
                transform.position = new Vector3(transform.position.x, position, transform.position.z);
            }
            else if (growingDirection == Vector3.right)
            {
                float position = Mathf.Lerp(startPosition.x, startPosition.x + growSize, time / growTime);
                transform.position = new Vector3(position, transform.position.y, transform.position.z);
            }
            else
            {
                float position = Mathf.Lerp(startPosition.x, startPosition.x - growSize, time / growTime);
                transform.position = new Vector3(position, transform.position.y, transform.position.z);
            }

            yield return null;
        }
        transform.localScale = localScale * endScale;

        yield return new WaitForSeconds(waitTime);
        spikeCollider.enabled = false;

        time = 0f;
        while (time < retractionTime)
        {
            time += Time.deltaTime;
            float scale = Mathf.Lerp(endScale, startScale, time / retractionTime);
            transform.localScale = localScale * scale;

            if (growingDirection == Vector3.up)
            {
                float position = Mathf.Lerp(startPosition.y + growSize, startPosition.y, time / retractionTime);
                transform.position = new Vector3(transform.position.x, position, transform.position.z);
            }
            else if(growingDirection == Vector3.down){
                float position = Mathf.Lerp(startPosition.y - growSize, startPosition.y, time / retractionTime);
                transform.position = new Vector3(transform.position.x, position, transform.position.z);
            }
            else if (growingDirection == Vector3.right)
            {
                float position = Mathf.Lerp(startPosition.x + growSize, startPosition.x, time / retractionTime);
                transform.position = new Vector3(position, transform.position.y, transform.position.z);
            }
            else
            {
                float position = Mathf.Lerp(startPosition.x - growSize, startPosition.x, time / retractionTime);
                transform.position = new Vector3(position, transform.position.y, transform.position.z);
            }

            yield return null;
        }
        transform.localScale = localScale * startScale;

        Destroy(gameObject, 0.5f);
    }

    public void increaseDamage(int increase){
        damage += increase;
    }
}
