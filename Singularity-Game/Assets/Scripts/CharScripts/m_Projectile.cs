using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class m_Projectile : MonoBehaviour
{
    [SerializeField] private GameObject rockPiece;
    [SerializeField] private GameObject rockBreakObject;
    [SerializeField] private float speed;
    public bool freeze = false;
    [HideInInspector] public Vector3 direction;
    public int dmg;

    //Move up, then move to player part
    private Vector3 startPosition;
    private bool init;
    private float MoveUpAmount;
    private float WaitBeforeMovingAmount;
    public bool waitBeforeAttack;
    private float counter;

    void Start()
    {
        startPosition = transform.position;
        init = true;
        MoveUpAmount = 3f;
        WaitBeforeMovingAmount = 0.5f;
        waitBeforeAttack = false;
        counter = 0f;

        this.speed = 22f;
        this.dmg = 50;
        // this.direction = new Vector3(0f, 0.5f, 0f);
    }

    void Update()
    {
        MovedUp();
        WaitCounter();
        Move();
    }

    private void OnTriggerEnter(Collider col)
    {
        var obj = col.gameObject;
        if (!col.isTrigger && obj.GetComponent<Damageable>())
        {
            obj.GetComponent<Damageable>().ApplyDamage(dmg);
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

    public void setDir(Vector3 dir)
    {
        this.direction = Vector3.Normalize(dir - transform.position);
        this.direction.z = 0;
        freeze = false;
    }

    public void OnDeath()
    {
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
            Destroy(pieceClone, 3f);
        }
    }

    private void MovedUp()
    {
        if (init)
        {
            if (Vector3.Distance(transform.position, startPosition) < MoveUpAmount) return;

            init = false;
            setDir(GameObject.FindWithTag("Staffstone").transform.position);
            waitBeforeAttack = true;
        }
    }

    private void WaitCounter()
    {
        if (!waitBeforeAttack) return;

        if (counter >= WaitBeforeMovingAmount) waitBeforeAttack = false;
        counter += Time.deltaTime;
    }

    private void Move()
    {
        if (freeze || waitBeforeAttack) return;
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        transform.Rotate(1.5f, 1.5f, 1.5f, Space.Self);
    }
}
