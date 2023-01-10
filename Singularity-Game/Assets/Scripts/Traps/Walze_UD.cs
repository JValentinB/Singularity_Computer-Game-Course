using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walze_UD : MonoBehaviour
{
    [SerializeField] private float range = 20;
    [SerializeField] private float speed = 10;
    [SerializeField] private float rotationSpeed = 500;

    private float count = 0;
    private bool backward = false;

    enum Direction
    { vor, zurück }
    [SerializeField] Direction direction;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (direction == Direction.vor)
        {
            MoveForward();
        }
        else if (direction == Direction.zurück)
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
            transform.Translate(0, -speed * Time.deltaTime, 0, Space.World);
            count += speed * Time.deltaTime;
        }
        else if (backward)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            //transform.Translate(Vector3.left * speed * Time.deltaTime);
            transform.Translate(0, speed * Time.deltaTime, 0, Space.World);
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
            transform.Translate(0, speed * Time.deltaTime, 0, Space.World);
            count += speed * Time.deltaTime;
        }
        else if (backward)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            //transform.Translate(Vector3.left * speed * Time.deltaTime);
            transform.Translate(0, -speed * Time.deltaTime, 0, Space.World);
            count += speed * Time.deltaTime;
        }
    }
}
