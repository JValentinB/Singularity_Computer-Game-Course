using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject bonfireParticles;


    private bool isCurrentCheckpoint = false;
    private Checkpoint[] checkpoints;

    void Start(){
        checkpoints = FindObjectsOfType<Checkpoint>(); 
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player" && !isCurrentCheckpoint)
        {
            var playerScript = collision.GetComponent<Player>();
            playerScript.setCheckPoint(transform.position);
            playerScript.setFirstTime();
            playerScript.SetSavedWeaponModes(playerScript.unlockedWeaponModes);

            foreach (Checkpoint checkpoint in checkpoints)
            {
                checkpoint.isCurrentCheckpoint = false;
            }
            isCurrentCheckpoint = true;
            StartCoroutine(activationAnimation(collision.transform));
        }
    }

    IEnumerator activationAnimation(Transform player){
        GameObject particleObject = (GameObject)Instantiate(bonfireParticles, player.position, Quaternion.identity);

        var shape = particleObject  .GetComponent<ParticleSystem>().shape;
        var character = GameObject.Find("Character");
        shape.skinnedMeshRenderer = character.GetComponent<SkinnedMeshRenderer>();

        yield return new WaitForSeconds(1f);
        var forces = particleObject.GetComponent<ParticleSystem>().externalForces;
        forces.enabled = true;

        Destroy(particleObject, 3f);
    }
}
