using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvDatabase
{
    public List<InvItem> databaseItems = new List<InvItem>(){
        new InvItem(0, "Weightless Quartz", "This crystal emits a soft pink glow and can be used to manipulate the local gravitational field to lift objects. Its power is limited to specific objects that contain this rare mineral. When in contact with Weightless Quartz, the stone resonates with the mineral's unique electromagnetic properties, allowing it to manipulate gravity around that object.", null),
        new InvItem(1, "Gravity Catalyst", "This vibrant Orange Crystal is a formidable tool for any adventurer. When activated, it can activate the dormant magic in fields scattered throughout the landscape, allowing the user to manipulate gravity and walk on walls and ceilings. With this incredible ability, the Gravity Catalyst is an asset in combat and a valuable aid for navigating even the most challenging terrain.", null),
        new InvItem(2, "Singularity Crystal", "This beautiful turquoise crystal is one of the most dangerous in the galaxy. When activated, it creates a small black hole that sucks in anything in its path, including enemies and objects. The Singularity Crystal can also shoot out multiple black holes, making it an excellent choice to clear out large areas quickly.", null),
        new InvItem(3, "Aether Crystal", "This enigmatic blue crystal is a versatile tool for any adventurer. When carried, it allows the user to perform a powerful double jump, enabling them to reach new heights and evade danger. Additionally, the crystal can fire deadly shards that can eliminate enemies from afar. But Its true capabilities are yet to be discovered... and can be yours with the purchase of the 'Not so naked anymore!' DLC for only 14.99$.", null)
    };

    public InvItem GetItem(int id){
        return databaseItems.Find(item => item.id == id);
    }
}