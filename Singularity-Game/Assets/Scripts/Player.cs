using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float walking_speed = 0.4f;
    [SerializeField] private float running_speed = 2.4f;
    private float speed;
    private float direction = -1;
    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // press shift to run fast
        if (Input.GetKey(KeyCode.LeftShift)) speed = running_speed;
        else speed = walking_speed;

        //turn around
        if (Input.GetKeyDown(KeyCode.A)) direction = 1;
        if (Input.GetKeyDown(KeyCode.D)) direction = -1;
        this.transform.rotation = Quaternion.Euler(0, direction * 90, 0);

        var velocity = direction * Vector3.forward * Input.GetAxis("Horizontal") * speed;
        transform.Translate(velocity * Time.deltaTime);
        animator.SetFloat("Speed", velocity.magnitude);
    }
}
