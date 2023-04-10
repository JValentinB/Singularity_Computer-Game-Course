using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roller_LR : MonoBehaviour
{
    [SerializeField] private float range = 20;
    [SerializeField] private float speed = 10;
    [SerializeField] private float rotationSpeed = 500;

    private bool backward = false;
    private float count = 0;
    private int dmg;

    enum Direction
    { forward, backward }
    [SerializeField] Direction direction;



    // Start is called before the first frame update
    void Start()
    {
        dmg = 50;
    }

    // Update is called once per frame
    void Update()
    {
        if (direction == Direction.forward)
        {
            MoveForward();
        }
        else if (direction == Direction.backward)
        {
            MoveBackward();
        }
    }
    void MoveForward()
    {
        if (count >= range)
        {
            backward = !backward;
            count = 0;
        }

        if (!backward)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            //transform.Translate(Vector3.right * speed * Time.deltaTime);
            transform.Translate(speed * Time.deltaTime, 0, 0, Space.World);
            count += speed * Time.deltaTime;
        }
        else if (backward)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            //transform.Translate(Vector3.left * speed * Time.deltaTime);
            transform.Translate(-speed * Time.deltaTime, 0, 0, Space.World);
            count += speed * Time.deltaTime;
        }
    }

    void MoveBackward()
    {
        if (count >= range)
        {
            backward = !backward;
            count = 0;
        }

        if (!backward)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            //transform.Translate(Vector3.right * speed * Time.deltaTime);
            transform.Translate(-speed * Time.deltaTime, 0, 0, Space.World);
            count += speed * Time.deltaTime;
        }
        else if (backward)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            //transform.Translate(Vector3.left * speed * Time.deltaTime);
            transform.Translate(speed * Time.deltaTime, 0, 0, Space.World);
            count += speed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        var obj = col.gameObject;
        if (obj.GetComponent<Damageable>())
        {
            obj.GetComponent<Damageable>().ApplyDamage(dmg);
            GetComponent<AudioSource>().Play();
        }
    }
}
