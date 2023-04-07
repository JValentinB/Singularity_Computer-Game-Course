using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float speed = 5;

    private Player player;
    [SerializeField] private bool active;
    [SerializeField] private bool up;
    [SerializeField] private bool down;
    [SerializeField] private GameObject leaveCollider;

    private bool playerInside;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        up = true;
        down = false;
        active = false;
        playerInside = false;
        leaveCollider.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (active && up)
        {
            MoveDown();
        }
        if (active && down)
        {
            MoveUp();
        }


    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.GetComponent<Player>())
        {
            setActive();
            setPlayerInside();
        }
        
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        leaveCollider.transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (playerInside)
        {
            player.transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
        
    }
    private void MoveUp()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
        leaveCollider.transform.Translate(Vector3.up * speed * Time.deltaTime);
        if (playerInside)
        {
            player.transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        
    }

    public void setActive()
    {
        active = true;
    }

    public void setNotActive()
    {
        active = false;
    }

    public void setUp()
    {
        up = true;
        down = false;
    }

    public void setDown()
    {
        up = false;
        down = true;
    }

    public bool getAct()
    {
        return active;
    }
    public bool getUp()
    {
        return up;
    }
    public bool getDown()
    {
        return down;
    }

    void setPlayerInside()
    {
        playerInside = true;
        leaveCollider.SetActive(true);
    }

    public void setPlayerOutside()
    {
        playerInside = false;
        leaveCollider.SetActive(false);
    }
}
