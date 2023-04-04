using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatteringRoots : MonoBehaviour
{
    public GameObject shatteredRoots;
    public float scale = 1f;

    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Boulder") // || collision.gameObject.tag == "Player")
        {
            Shatter();

            // shatter all other child roots
            foreach (Transform child in transform)
            {   
                ShatteringRoots shatteredChild = child.GetComponent<ShatteringRoots>();
                if (shatteredChild != null)
                {
                    shatteredChild.Shatter();
                }
            }

            collision.gameObject.GetComponent<Boulder>().destroyedRoots = true;
        }
    }

    public void Shatter()
    {
        if(shatteredRoots == null)
            return;

        if(audioSource != null)
            audioSource.Play();

        GameObject shattered = Instantiate(shatteredRoots, transform.position, transform.rotation);
        shattered.transform.localScale = transform.lossyScale * scale;

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 5);
        Destroy(shattered, 5);
    }
}
