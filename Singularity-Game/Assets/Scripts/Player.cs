using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float walking_speed = 0.4f;
    [SerializeField] private float running_speed = 2.4f;
    [SerializeField] private float jumpforce = 400;
    private float speed;

    private float direction = -1;
    private Animator animator;
    private Rigidbody rigidbody;

    private float lastPosY;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        lastPosY = this.transform.position.y;
    }
    
    // Update is called once per frame
    void Update()
    {
        // press shift to run fast
        if (Input.GetKey(KeyCode.LeftShift)) speed = running_speed;
        else speed = walking_speed;

        // turn around
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) direction = 1;
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) direction = -1;
        this.transform.rotation = Quaternion.Euler(0, direction * 90, 0);

        float landing = (animator.GetCurrentAnimatorStateInfo(0).IsName("Landing")) ? 0.5f : 1;
        var velocity = direction * Vector3.forward * Input.GetAxis("Horizontal") * landing * speed;
        transform.Translate(velocity * Time.deltaTime);
        //rigidbody.AddForce(velocity * rigidbody.mass * 300);
        animator.SetFloat("Speed", velocity.magnitude);


        // Jumping
        if (animator.GetBool("Jumping") && animator.GetCurrentAnimatorStateInfo(0).IsName("Falling")) 
            animator.SetBool("Jumping", false);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jumping");
            animator.SetBool("Falling", true);
            rigidbody.AddForce(Vector3.up * jumpforce);
        }

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, 0);
    }
    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position + new Vector3(0,1,0) , new Vector3(0, -5, 0));
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            print(hit.collider.name + " " + hit.distance);
            if(hit.distance > 1.05)
                animator.SetBool("Falling", true);
            else animator.SetBool("Falling", false);
        }
    }
}
