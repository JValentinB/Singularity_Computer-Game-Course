using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingCliff : MonoBehaviour
{
    // enum EventTarget
    // { Self, Children }
    // [SerializeField] EventTarget eventTarget;
    enum TriggerEvent
    { StandingOnCliff, PullOut }
    [SerializeField] TriggerEvent triggerEvent;
    public int healthPoints = 100;
    public int damageTakenPerFrame = 2;
    public float fallingDelay = 5f;
    public float disableDelay = 5f;
    public float destroyingDelay = 20f;
    [Range(0f, 1f)]
    public float maxRumblingVolume = 1f;

    private bool isFalling = false;
    private List<Rigidbody> rigidbodies = new List<Rigidbody>();
    private float maxHealth;

    private AudioManager audioManager;
    private Coroutine rumblingCoroutine;
    private bool isRumbling = false;
    private Coroutine fadeOutCoroutine;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();

        rigidbodies.Add(GetComponent<Rigidbody>());
        // Add all children rigidbodies to the list if they have one
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Rigidbody rb = child.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rigidbodies.Add(rb);
            }
        }
        maxHealth = (float)healthPoints;
    }

    private void Update()
    {
        if (healthPoints <= 0 && !isFalling)
        {
            StartFalling();
            Destroy(this.gameObject, destroyingDelay);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (!isFalling && triggerEvent == TriggerEvent.StandingOnCliff && other.collider.CompareTag("Player"))
        {
            healthPoints -= damageTakenPerFrame;

            Debug.Log("Rumbling");
            if (fadeOutCoroutine != null)
                StopCoroutine(fadeOutCoroutine);
            if (!isRumbling)
                rumblingCoroutine = StartCoroutine(playSoundWithIncreasingVolume());

        }
    }
    private void OnCollisionExit(Collision other)
    {
        if (!isFalling && triggerEvent == TriggerEvent.StandingOnCliff && other.collider.CompareTag("Player"))
        {
            StopCoroutine(rumblingCoroutine);
            isRumbling = false;
            fadeOutCoroutine = StartCoroutine(audioManager.fadeOut(audioManager.environmentSounds, "CliffRumbling", 4f));
        }
    }

    public void pullOut()
    {
        if (triggerEvent == TriggerEvent.PullOut && !isFalling)
        {
            isFalling = true;
            Invoke("StartFalling", fallingDelay);
            Destroy(this.gameObject, destroyingDelay);
        }
    }

    private void StartFalling()
    {
        audioManager.Play(audioManager.environmentSounds, "CliffBreaking");

        isFalling = true;
        for (int i = 0; i < rigidbodies.Count; i++)
        {
            Rigidbody rb = rigidbodies[i];
            rb.isKinematic = false;
            // rb.AddForce(fall ingForce);
        }
    }

    void disableCollider()
    {
        for (int i = 0; i < rigidbodies.Count; i++)
        {
            Rigidbody rb = rigidbodies[i];
            rb.GetComponent<MeshCollider>().enabled = false;
        }
    }

    // play sound and depending on health points increase volume
    IEnumerator playSoundWithIncreasingVolume()
    {   
        isRumbling = true;
        float volume = audioManager.getSourceVolume(audioManager.environmentSounds, "CliffRumbling");
        audioManager.Play(audioManager.environmentSounds, "CliffRumbling");
        audioManager.setSourceVolume(audioManager.environmentSounds, "CliffRumbling", volume);
        while (volume < maxRumblingVolume)
        {
            // When health points are 0, volume is 1
            volume = maxRumblingVolume - maxRumblingVolume * ((float)healthPoints / maxHealth);
            audioManager.setSourceVolume(audioManager.environmentSounds, "CliffRumbling", volume);
            yield return null;
        }
        audioManager.setSourceVolume(audioManager.environmentSounds, "CliffRumbling", 0);
        audioManager.Stop(audioManager.environmentSounds, "CliffRumbling");
        isRumbling = false;
    }
}
