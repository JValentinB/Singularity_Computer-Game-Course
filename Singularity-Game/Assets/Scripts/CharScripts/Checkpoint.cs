using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameObject bonfireParticles;

    private bool isCurrentCheckpoint = false;
    private Checkpoint[] checkpoints;
    private static List<int> storyPartIndexCheckpoint;
    private static List<bool> storyShownCheckpoint;

    void Start(){
        checkpoints = FindObjectsOfType<Checkpoint>();
        LoadStoryProgressLists();
    }

    private void LoadStoryProgressLists(){
        if(storyPartIndexCheckpoint == null){
           storyPartIndexCheckpoint = new List<int>();
            storyShownCheckpoint = new List<bool>();
            return;
        } else if(storyPartIndexCheckpoint.Count == 0){
            return;
        }
        foreach(Transform storyTrigger in GameObject.FindWithTag("StoryTextParent").transform){
            int index = storyPartIndexCheckpoint.FindIndex(a => a == storyTrigger.GetComponent<StoryTrigger>().storyPartIndex);
            storyTrigger.GetComponent<StoryTrigger>().storyShown = storyShownCheckpoint[index];
        }
    }

    private void SaveStoryProgress(){
        foreach(Transform storyTrigger in GameObject.FindWithTag("StoryTextParent").transform){
            storyPartIndexCheckpoint.Add(storyTrigger.GetComponent<StoryTrigger>().storyPartIndex);
            storyShownCheckpoint.Add(storyTrigger.GetComponent<StoryTrigger>().storyShown);
        }
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

            SaveStoryProgress();

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
