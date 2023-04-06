using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDeActivation : MonoBehaviour
{   
    // enum for activation and deactivation
    public enum LaserState { Activate, Deactivate };
    public LaserState laserState = LaserState.Deactivate;
    public List<LaserEmitter> laserEmitters;
    public LaserEmitter startingEmitter;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (laserState == LaserState.Activate)
            {
                activateEmitter();
            }
            else if (laserState == LaserState.Deactivate)
            {
                deactivateAllEmitters();
            }
        }
    }

    void activateEmitter(){
        deactivateAllEmitters();
        
        startingEmitter.startEmitting(false);
    }

    void deactivateAllEmitters(){
        foreach(LaserEmitter laserEmitter in laserEmitters){
            laserEmitter.stopEmitting(false);
            laserEmitter.wasAlreadyActive = false;
        }
    }
}
