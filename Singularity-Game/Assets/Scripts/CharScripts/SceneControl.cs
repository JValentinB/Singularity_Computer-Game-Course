using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    public Scene checkpoint;
    // Start is called before the first frame update
    void Start()
    {
        checkpoint = SceneManager.GetActiveScene();
    }


    public void set_checkpoint()
    {
        checkpoint = SceneManager.GetActiveScene();
    }


    public void reset_on_death()
    {
    
     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
       
        
    }
}
