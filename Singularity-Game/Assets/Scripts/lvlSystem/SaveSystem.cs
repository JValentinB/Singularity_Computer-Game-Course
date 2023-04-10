using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSystem
{
    public static bool couldNotLoadGame;
    public static float loadingDelay;

    public static void SaveGame(Player player){
        SaveData saveData = new SaveData(player);

        string saveGamePath = Application.persistentDataPath + "/ugnmr_save.json";
        string jsonSaveGame = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(saveGamePath, jsonSaveGame);

        //Show SaveGame Path in Log
        Debug.Log(saveGamePath);
    }

    public static SaveData ReadSaveFile(){
        string saveGamePath = Application.persistentDataPath + "/ugnmr_save.json";

        if(!File.Exists(saveGamePath)){
            Debug.Log("Savefile doesn't exist!");
            return null;
        }

        string saveDataContent = File.ReadAllText(saveGamePath);
        return JsonUtility.FromJson<SaveData>(saveDataContent);
    }

    public static void LoadGame(){
        SaveData saveData = ReadSaveFile();
        if(saveData == null){ return; }

        SceneManager.LoadScene("Forest1.0_Valentin");

        if(SceneManager.GetActiveScene().name != "Forest1.0_Valentin"){
            loadingDelay = 1f;
            couldNotLoadGame = true;
            Debug.Log("Couldn't load game, wrong active scene!");
            return;
        }

        GameObject.Find("Main Camera").GetComponent<SceneControl>().reset_on_death();
        loadingDelay = 1f;
        couldNotLoadGame = true;

        Debug.Log("Scene successfully loaded!");
    }

    public static void LoadGameNoReset(){
        couldNotLoadGame = false;
        SaveData saveData = ReadSaveFile();
        if(saveData == null){ return; }

        if(SceneManager.GetActiveScene().name != "Forest1.0_Valentin"){
            couldNotLoadGame = true;
            return;
        }

        var player = GameObject.FindWithTag("Player").GetComponent<Player>();

        player.setCheckPoint(new Vector3(saveData.lastCheckpoint[0], saveData.lastCheckpoint[1], saveData.lastCheckpoint[2]));
        player.transform.position = new Vector3(saveData.lastCheckpoint[0], saveData.lastCheckpoint[1], saveData.lastCheckpoint[2]);

        if(!GameObject.FindWithTag("Player")){
            Debug.Log("Player doesnt exist!");
            return;
        }

        player.currentHealth = saveData.health;
        player.unlockedWeaponModes = saveData.unlockedWeaponModes;
        player.SetSavedWeaponModes(saveData.unlockedWeaponModes);
        player.doubleJump = saveData.doubleJumpBoots;

        //Not checking if inventory is empty. Should be empty since we reload scene
        if(player.inventory.IsEmpty()){
            for(int i = 0; i < saveData.invItemID.Count; i++){
                player.GiveItem(player.inventory.GetItem(saveData.invItemID[i]), saveData.invItemAmount[i]);
                //player.inventory.AddItem(player.inventory.GetItem(saveData.invItemID[i]), saveData.invItemAmount[i]);
            }
        }

        foreach(Transform storyTrigger in GameObject.FindWithTag("StoryTextParent").transform){
            int index = saveData.storyPartIndex.FindIndex(a => a == storyTrigger.GetComponent<StoryTrigger>().storyPartIndex);
            storyTrigger.GetComponent<StoryTrigger>().storyShown = saveData.storyShown[index];
        }

        if(saveData.doubleJumpBoots) UnityEngine.Object.Destroy(GameObject.FindWithTag("DoubleJumpCrystal"));
        if(saveData.unlockedWeaponModes[1]) UnityEngine.Object.Destroy(GameObject.FindWithTag("ShifterCrystal"));
        if(saveData.unlockedWeaponModes[2]) UnityEngine.Object.Destroy(GameObject.FindWithTag("BlackHoleCrystal"));
        if(saveData.unlockedWeaponModes[0]){
            UnityEngine.Object.Destroy(GameObject.FindWithTag("PullCrystal"));
            UnityEngine.Object.Destroy(GameObject.FindWithTag("FallingRocks")); 
        }

        if(saveData.golemDefeated) UnityEngine.Object.Destroy(GameObject.FindWithTag("GolemBoss"));
        if(saveData.treeBossEntryDestroyed) UnityEngine.Object.Destroy(GameObject.FindWithTag("BossEntry"));
        GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().dead = saveData.treeBossDefeated;

        Checkpoint.treeBossEntryOpened = saveData.treeBossEntryDestroyed;
        Checkpoint.treeBossDead = saveData.treeBossDefeated;
        Checkpoint.golemDead = saveData.golemDefeated;

        loadingDelay = -1f;

        Debug.Log("Savefile successfully loaded without reset!");
    }
}
