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

    //private static List<(InvItem, int)> playerInventory;
    private static List<int> invItemID = new List<int>();
    private static List<int> invItemAmount = new List<int>();

    public static bool treeBossEntryOpened, treeBossDead, golemDead;

    void Start(){
        checkpoints = FindObjectsOfType<Checkpoint>();
        StartCoroutine(LoadStoryProgressLists());
    }

    private IEnumerator LoadStoryProgressLists(){
        var player = GameObject.FindWithTag("Player").GetComponent<Player>();

        var weaponModes = player.GetSavedWeaponModes();
        if(player.doubleJump && GameObject.FindWithTag("DoubleJumpCrystal")) UnityEngine.Object.Destroy(GameObject.FindWithTag("DoubleJumpCrystal"));
        if(weaponModes[1] && GameObject.FindWithTag("ShifterCrystal")) UnityEngine.Object.Destroy(GameObject.FindWithTag("ShifterCrystal"));
        if(weaponModes[2] && GameObject.FindWithTag("BlackHoleCrystal")) UnityEngine.Object.Destroy(GameObject.FindWithTag("BlackHoleCrystal"));
        if(weaponModes[0] && GameObject.FindWithTag("PullCrystal")){
            UnityEngine.Object.Destroy(GameObject.FindWithTag("PullCrystal"));
            UnityEngine.Object.Destroy(GameObject.FindWithTag("FallingRocks")); 
        }

        if(golemDead && GameObject.FindWithTag("GolemBoss")) UnityEngine.Object.Destroy(GameObject.FindWithTag("GolemBoss"));
        if(treeBossEntryOpened && GameObject.FindWithTag("BossEntry")) UnityEngine.Object.Destroy(GameObject.FindWithTag("BossEntry"));
        GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().dead = treeBossDead;
        if(GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().dead) GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().ToggleObjectsAtDeath();

        if(storyPartIndexCheckpoint == null){
           storyPartIndexCheckpoint = new List<int>();
            storyShownCheckpoint = new List<bool>();
        } else if(storyPartIndexCheckpoint.Count == 0){
            
        } else {
            foreach(Transform storyTrigger in GameObject.FindWithTag("StoryTextParent").transform){
                int index = storyPartIndexCheckpoint.FindIndex(a => a == storyTrigger.GetComponent<StoryTrigger>().storyPartIndex);
                storyTrigger.GetComponent<StoryTrigger>().storyShown = storyShownCheckpoint[index];
            }
        }

        yield return new WaitForSeconds(0.3f);

        if(invItemID != null && invItemID.Count != 0 && player.inventory.IsEmpty()){
            for(int i = 0; i < invItemID.Count; i++){
                player.GiveItem(player.inventory.GetItem(invItemID[i]), invItemAmount[i]);
            }
        }
        yield break;
    }

    private void SaveStoryProgress(){
        var player = GameObject.FindWithTag("Player").GetComponent<Player>();

        foreach(Transform storyTrigger in GameObject.FindWithTag("StoryTextParent").transform){
            storyPartIndexCheckpoint.Add(storyTrigger.GetComponent<StoryTrigger>().storyPartIndex);
            storyShownCheckpoint.Add(storyTrigger.GetComponent<StoryTrigger>().storyShown);
        }

        foreach(var item in player.inventory.stackedInventoryItems){
            invItemID.Add(item.Item1.id);
            invItemAmount.Add(item.Item2);
        }

        if(GameObject.FindWithTag("BossEntry")) treeBossEntryOpened = false;
        else treeBossEntryOpened = true;
        if(GameObject.FindWithTag("GolemHeart")) golemDead = false;
        else golemDead = true;
        treeBossDead = GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().dead;
    }

    private void OnTriggerEnter(Collider collision)
    {
        SaveStoryProgress();
        
        if (collision.tag == "Player" && !isCurrentCheckpoint)
        {
            var playerScript = collision.GetComponent<Player>();
            playerScript.currentHealth = playerScript.maxHealth;
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

        var shape = particleObject.GetComponent<ParticleSystem>().shape;
        var character = player.Find("Character");
        shape.skinnedMeshRenderer = character.GetComponent<SkinnedMeshRenderer>();

        yield return new WaitForSeconds(1f);
        var forces = particleObject.GetComponent<ParticleSystem>().externalForces;
        forces.enabled = true;

        Destroy(particleObject, 3f);
    }
}
