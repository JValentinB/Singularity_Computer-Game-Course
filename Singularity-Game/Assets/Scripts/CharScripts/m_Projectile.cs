using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class m_Projectile : MonoBehaviour
{
    [SerializeField] private GameObject rockPiece;
    [SerializeField] private GameObject rockBreakObject;
    [SerializeField] private float speed = 22f;
    public int damage = 20;
    public float stoppingTime = 2f;
    public bool freeze = false;
    public float MoveUpAmount = 5f;
    [HideInInspector] public Vector3 direction;

    //Move up, then move to player part
    private Vector3 startPosition;

    private bool init = true;
    private bool destroyed = false;

    void Start()
    {
        startPosition = transform.position;
        init = true;

        StartCoroutine(rockMovement());

        GameObject rockBreakClone = Instantiate(rockBreakObject, transform.position, Quaternion.identity);
        rockBreakClone.GetComponent<AudioSource>().Play();
        Destroy(rockBreakClone, 2f);
    }

    private void OnTriggerEnter(Collider col)
    {
        var obj = col.gameObject;
        if (!col.isTrigger && obj.GetComponent<Damageable>())
        {
            obj.GetComponent<Damageable>().ApplyDamage(damage);
            OnDeath();
        }
        else if (freeze && !obj.GetComponent<Projectile>())
        {
            OnDeath();
        }
        else if (!col.isTrigger && !init)
        {
            OnDeath();
        }
    }

    IEnumerator rockMovement()
    {
        while(Vector3.Distance(transform.position, startPosition) < MoveUpAmount){
            Move();
            yield return null;
        }
        init = false;
        
        yield return new WaitForSeconds(0.9f * stoppingTime);
        setDir(GameObject.FindWithTag("Staffstone").transform.position);
        yield return new WaitForSeconds(0.1f * stoppingTime);

        while(!destroyed){
            Move();
            yield return null;
        }
    }

    public void setDir(Vector3 dir)
    {
        this.direction = Vector3.Normalize(dir - transform.position);
        this.direction.z = 0;
        freeze = false;
    }

    public void OnDeath()
    {   
        destroyed = true;
        createPieces();
        GameObject rockBreakClone = Instantiate(rockBreakObject, transform.position, Quaternion.identity);
        rockBreakClone.GetComponent<AudioSource>().Play();

        Destroy(gameObject);
        Destroy(rockBreakClone, 2f);
    }

    private void createPieces()
    {
        Vector3 pos = transform.position;
        for (int i = 0; i < 5; i++)
        {
            Vector3 piecePos = new Vector3(pos.x + Random.value, pos.y + Random.value, pos.z + Random.value);
            GameObject pieceClone = Instantiate(rockPiece, piecePos, transform.rotation);
            pieceClone.transform.localScale *= Random.Range(0.75f, 4f);
            Destroy(pieceClone, 3f);
        }
    }

    private void Move()
    {
        if (freeze) return;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.Rotate(1.5f, 1.5f, 1.5f, Space.Self);
    }

    public void increaseDamage(int increase){
        damage += increase;
    }
}
