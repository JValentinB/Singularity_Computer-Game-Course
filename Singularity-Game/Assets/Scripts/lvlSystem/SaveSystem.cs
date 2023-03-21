using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSystem
{
    public static bool couldNotLoadGame;

    public static void SaveGame(Player player){
        SaveData saveData = new SaveData(player);

        string saveGamePath = Application.persistentDataPath + "/ugnmr_save.json";
        string jsonSaveGame = JsonUtility.ToJson(saveData);
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
            couldNotLoadGame = true;
            return;
        }

        var player = GameObject.FindWithTag("Player").GetComponent<Player>();

        player.setCheckPoint(new Vector3(saveData.lastCheckpoint[0], saveData.lastCheckpoint[1], saveData.lastCheckpoint[2]));

        GameObject.Find("Main Camera").GetComponent<SceneControl>().reset_on_death();

        player.currentHealth = saveData.health;
        player.weaponModes = saveData.weaponModes;
        player.doubleJump = saveData.doubleJumpBoots;

        //Not checking if inventory is empty. Should be empty since we reload scene
        for(int i = 0; i < saveData.invItemID.Count; i++){
            player.inventory.AddItem(player.inventory.GetItem(saveData.invItemID[i]), saveData.invItemAmount[i]);
        }

        Debug.Log("Savefile successfully loaded!");
    }
}
