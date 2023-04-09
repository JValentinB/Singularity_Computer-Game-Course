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
    public static bool treeBossEntryOpened, treeBossDead, golemDead;

    void Start(){
        checkpoints = FindObjectsOfType<Checkpoint>();
        LoadStoryProgressLists();
    }

    private void LoadStoryProgressLists(){
        if(golemDead && GameObject.FindWithTag("GolemBoss")) UnityEngine.Object.Destroy(GameObject.FindWithTag("GolemBoss"));
        if(treeBossEntryOpened && GameObject.FindWithTag("BossEntry")) UnityEngine.Object.Destroy(GameObject.FindWithTag("BossEntry"));
        GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().dead = treeBossDead;

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

        if(GameObject.FindWithTag("BossEntry")) treeBossEntryOpened = false;
        else treeBossEntryOpened = true;
        if(GameObject.FindWithTag("GolemHeart")) golemDead = false;
        else golemDead = true;
        treeBossDead = GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().dead;
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

        var shape = particleObject.GetComponent<ParticleSystem>().shape;
        var character = player.Find("Character");
        shape.skinnedMeshRenderer = character.GetComponent<SkinnedMeshRenderer>();

        yield return new WaitForSeconds(1f);
        var forces = particleObject.GetComponent<ParticleSystem>().externalForces;
        forces.enabled = true;

        Destroy(particleObject, 3f);
    }
}
