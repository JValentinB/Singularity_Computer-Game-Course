using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Fireball : MonoBehaviour
{
    [SerializeField] private ParticleSystem [] particles = new ParticleSystem[2];
    [SerializeField] private float range = 10;
    [SerializeField] private float speed = 1;
    private float count = 0;
    private float exploCount = 0;
    private float exploTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        particles[0].transform.position = transform.position;
        particles[1].transform.position = transform.position;
    }

    // Update is called once per frame
    void Update() 
    {
        particles[0].transform.position = transform.position;
        particles[1].transform.position = transform.position;
        if (count < range)
        {
            particles[0].Play();
            transform.Translate(Vector3.up * speed * Time.deltaTime);
            count += speed * Time.deltaTime;
        }
        else if (count >= range && exploCount < exploTime)
        {
            particles[0].Stop();
            particles[1].Play();
            exploCount += 1 * Time.deltaTime;
        }
        else if (count >= range && exploCount >= exploTime)
        {
            Destroy(gameObject);
        }
    }


}
