using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombFruit : MonoBehaviour
{
    public BlackHoleContainer blackholeContainer;
    [SerializeField] private GameObject explosionObject;
    [SerializeField] private float speed = 15f;
    public int damage = 50;
    private Vector3 target = Vector3.down;


    [HideInInspector] public bool isMoving = false;

    void Start()
    {
        blackholeContainer = GameObject.FindWithTag("BlackholeContainer").GetComponent<BlackHoleContainer>();
        speed = 15f;
    }

    void Update()
    {
        if (isMoving)
            Move();
    }

    public void setDir(Vector3 target)
    {
        this.target = Vector3.Normalize(target - transform.position);
        this.target.z = 0;
    }

    public void OnDeath()
    {
        TreeBoss treeBoss = GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>();
        treeBoss.ApplyDamage(damage);
        treeBoss.stunned = true;
        
        var explosion = Instantiate(explosionObject, transform.position, Quaternion.identity);
        explosion.GetComponent<ParticleSystem>().Play();
        explosion.GetComponent<AudioSource>().Play();
        Destroy(gameObject);
        Destroy(explosion, 2f);
    }

    private void Move()
    {
        if (blackholeContainer.active && blackholeContainer.blackhole != null)
            target = blackholeContainer.blackhole.transform.position;

        Vector3 direction = target - transform.position;
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.Rotate(1.5f, 1.5f, 1.5f, Space.Self);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.GetComponent<BlackHoleContainer>())
        {
            GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().ApplyDamage(damage);
            OnDeath();
        }
    }

    public void setTarget(Vector3 target)
    {
        this.target = target;
    }
}
