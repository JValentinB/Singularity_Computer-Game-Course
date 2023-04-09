using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StorytextControl : MonoBehaviour
{
    [SerializeField] private int charIndex, storyIndex;
    [SerializeField] public int storyPartIndex;
    private TMPro.TextMeshProUGUI headerField, textField;
    private AudioSource textSound; 
    private GameObject player;
    public Coroutine storyCoroutine;

    public static List<int> storyPartIndexCheckpoint;
    public static List<bool> storyShownCheckpoint;

    private List<(string, string)> spaceStoryText = new List<(string, string)>();
    private string finalText;
    private bool writing, visitedArion, stopText;

    public static bool uwuMode;

    [SerializeField] public CanvasGroup healthBar, weaponModeDisplay, weaponWheel;

    void Start()
    {
        textSound = GetComponent<AudioSource>();
        headerField = transform.Find("InfoHeader").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        textField = transform.Find("InfoDescription").gameObject.GetComponent<TMPro.TextMeshProUGUI>();
        player = GameObject.FindWithTag("Player");

        GetComponent<CanvasGroup>().alpha = 0f;
        storyIndex = 0;
        storyPartIndex = -1;
    }


    //To add a text, add a string tuple
    //First item is the name, second is the text
    public void AddStoryText(){
        spaceStoryText.Clear();
        storyIndex = 0;

        switch (storyPartIndex)
        {
            case 0:
                spaceStoryText.Add(
                    ("P. Otter", "What the heck?! What happened?\nArion, can you hear me! Where are you?")
                );
                spaceStoryText.Add(
                    ("Arion", "...")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Damn it. This looks bad.\nI gotta find Arion.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Right side looks like a dead end, so left it is!")
                );
                break;
            case 1:
                spaceStoryText.Add(
                    ("P. Otter", "Doesn't look like I can continue here right now!")
                );
                break;
            case 2:
                spaceStoryText.Add(
                    ("P. Otter", "Arion, come in. Do you read me?")
                );
                spaceStoryText.Add(
                    ("Arion", "...")
                );
                spaceStoryText.Add(
                    ("P. Otter", "I guess the system was shut down after crashing!\nAnd... Arion can you here me now?")
                );
                spaceStoryText.Add(
                    ("Arion", "Ahem. Finally, P. Otter.\nIt's about time you decided to turn me on.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Can you tell me again what we're looking for?")
                );
                spaceStoryText.Add(
                    ("Arion", "Of course, as I already told you before, we require a power regulator.\nIt's basic knowledge that any competent spaceship captain would know.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Alright, where should we start looking first then, Arion?")
                );
                spaceStoryText.Add(
                    ("Arion", "Based on my superior analysis, we should search the nearby clearing first.\nThere's a high probability of finding the power regulator there.\nBut don't worry, P. Otter, I'm sure you'll learn from this experience and become a better captain someday.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Let's just get this over with.\nAnd watch your attitude, Arion.")
                );
                visitedArion = true;
                break;
            case 3:
                spaceStoryText.Add(
                    ("Arion", "Well rip, P. Otter. Have fun down there!")
                );
                break;
            case 4:
                spaceStoryText.Add(
                    ("P. Otter", "Arion, what's this crystal?")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh, look who finally noticed something. That is a Weightless Quartz crystal, P. Otter.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Weightless Quartz? What can I do with it?")
                );
                spaceStoryText.Add(
                    ("Arion", "This crystal emits a soft pink glow and can be used to manipulate the local gravitational field to lift objects.\nIts power is limited to specific objects that contain this rare mineral.\n When in contact with Weightless Quartz, the stone resonates with the mineral's unique electromagnetic properties, allowing it to manipulate gravity around that object.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Interesting. I wonder what I can lift with this.")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh, I don't know, maybe the weight of your indecisiveness?")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Very funny, Arion.")
                );
                spaceStoryText.Add(
                    ("Arion", "I know. I'm hilarious. Now, hurry up and find something to lift. I'm getting bored here.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "How about that rock over there?")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh, sure. Let's lift a rock. How exciting.")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh right, and for the actual player. You have to right click on the objects with the purple crystals to move them.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "What?")
                );
                spaceStoryText.Add(
                    ("Arion", "What?")
                );
                break;
            case 5:
                spaceStoryText.Add(
                    ("P. Otter", "What's with that laser? Looks dangerous.")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh, you think so? Congratulations, you've managed to state the obvious.\nThat laser beam is capable of melting through porous rock, so you'll want to be careful around it.\nBut don't worry, I'm sure you can handle it.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Okay, I'll be careful.")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh, I'm sure you will. But don't worry, I'll be here to guide you through it.\nNow, see those floating stones over there? They contain weightless quartz, which can bend the laser beam.\nYou'll need to use them to make your way across the cliff safely.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Alright, let's do this.")
                );
                spaceStoryText.Add(
                    ("Arion", "Just remember, one wrong move and you'll be nothing but a pile of molten otter.")
                );
                break;
            case 6:
                spaceStoryText.Add(
                    ("Arion", "P. Otter, it looks like there's is something way more interesting to our right, than wherever you're planning to go right now!")
                );
                break;
            case 7:
                spaceStoryText.Add(
                    ("P. Otter", "Another crystal, huh? What does this one do?")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh, just another enigmatic blue crystal with unknown powers. Some believe it can control time or create portals to other dimensions.\nBut who knows? Maybe it just helps you jump a little farther.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Jump farther? That's it?")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh, I'm sorry. Were you hoping for something more exciting?\nMaybe you were expecting a crystal that would do your laundry or make you breakfast in the morning?")
                );
                spaceStoryText.Add(
                    ("P. Otter", "...Let's just grab the crystal and move on.")
                );
                spaceStoryText.Add(
                    ("Arion", "As you wish, Captain.\nBut don't forget, with great power comes great responsibility... or in this case, the ability to jump a little higher.")
                );
                break;
            case 8:
                spaceStoryText.Add(
                    ("P. Otter", "What is this thing? And how am I supposed to defeat it?")
                );
                spaceStoryText.Add(
                    ("Arion", "That, my dear Otter, is a stone golem. And even a child knows how to take down a golem.\nIt's as easy as aiming your laser at its heart. But of course, if you're too scared to do it, I can always take care of it for you.\nDon't worry, I won't even charge you for my services. Consider it my way of doing you a favor.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Why do I have to go through all this trouble just to meet someone for a date?\nCan't we just meet at a coffee shop like normal Otters?")
                );
                spaceStoryText.Add(
                    ("Arion", "Ah, the classic question of the clueless hero. Why does he have to do all of this?\nBecause, my dear Otter, nothing in life comes easy. You want to meet your date?\nYou have to prove that you're worthy enough to get there. And destroying this golem is just the first step in proving that worthiness.")
                );
                break;
            case 9:
                spaceStoryText.Add(
                    ("Arion", "Well, well, well, it seems our little friend here has encountered a problem. The golem has destroyed the floating stones that were supposed to bend the laser.\nWhat a shame. But don't worry, my dear P. Otter, you just have to use your brain for once and find another way to bend that laser.\nAnd no, I won't do it for you. That would be too easy, wouldn't it?")
                );
                break;
            case 10:
                spaceStoryText.Add(
                    ("P. Otter", "What's this crystal for, Arion?")
                );
                spaceStoryText.Add(
                    ("Arion", "Ah, the Singularity Crystal! It's one of the most dangerous crystals in the galaxy.\nAnd I'm not just saying that to scare you, Otter.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Uh, thanks for the warning. So what does it do?")
                );
                spaceStoryText.Add(
                    ("Arion", "When activated, it creates a small black hole that sucks in anything in its path. That means enemies, objects, and even light itself will be drawn towards it.\nAnd if you're feeling extra adventurous, you can even shoot out multiple black holes with it.\nTalk about clearing out large areas quickly!")
                );
                spaceStoryText.Add(
                    ("P. Otter", "But what about me? Will I get sucked in too?")
                );
                spaceStoryText.Add(
                    ("Arion", "Don't worry, Otter. I made sure to program your suit to resist the gravitational pull of the Singularity Crystal.\nYou'll be able to wield its power without getting sucked in like a helpless little plankton.")
                );
                break;
            case 11:
                spaceStoryText.Add(
                    ("Arion", "Ah, the Gravity Catalyst, a personal favorite of mine. This little gem packs a powerful punch with its lethal projectiles.\nTrust me, you'll want to keep this one handy when facing tough enemies.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "And what about its ability to manipulate gravity and walk on walls and ceilings?")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh, that? Just a small feature to help with traversing challenging terrain.\nBut be warned, it takes a skilled adventurer to master the gravity manipulation. Don't go getting yourself stuck on a ceiling now, Otter.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "I'll be sure to be careful. So, how do I activate the crystal?")
                );
                spaceStoryText.Add(
                    ("Arion", "Simply channel your energy into the crystal and it'll do the rest.\nBut be warned, it can only be activated in specific fields scattered throughout the landscape.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Specific fields? What do you mean?")
                );
                spaceStoryText.Add(
                    ("Arion", "The crystal resonates with certain areas of the environment that contain latent magical energy.\nThese fields are rare and difficult to find, but once you do, you'll be able to access the crystal's full potential.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "I suppose. But I have to admit, all this power makes me a little nervous.")
                );
                spaceStoryText.Add(
                    ("Arion", "Nervous? Ha! That's what separates the real adventurers from the amateurs. You've got to be bold, Otter.\nTake risks, push your limits, and embrace the power of the Gravity Catalyst. Trust me, it won't disappoint.")
                );
                break;
            case 12:
                spaceStoryText.Add(
                    ("Arion", "Ah, a gravitational field. This is where the real fun begins, my dear Otter. Just shoot one of its charges towards the field, and you'll be walking on walls in no time.")
                );
                break;
            case 13:
                spaceStoryText.Add(
                    ("Arion", "Hold on, P. Otter. My scans are picking up something powerful guarding the area.\nWe need to be cautious...This is incredible! My scans show that the power regulator is just beyond that life form.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "What do you mean by powerful? Another golem?")
                );
                spaceStoryText.Add(
                    ("Arion", "No, it appears to be a sentient being, with a body that resembles a tree. But don't let its appearance fool you, P. Otter.\n It's highly intelligent and incredibly powerful. I've never seen anything like it.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "What do we do? Can we just walk past it?")
                );
                spaceStoryText.Add(
                    ("Arion", "I wouldn't recommend it. It's highly territorial and fiercely protective of its home.\nWe'll have to find a way to communicate with it and negotiate our way past it.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "How do we do that? It's not like we speak tree.")
                );
                spaceStoryText.Add(
                    ("Arion", "Actually, I may have a solution. I've been working on a language translation program that can translate virtually any language.\nLet me see if I can modify it to work with this life form's unique language. Give me a moment.")
                );
                spaceStoryText.Add(
                    ("Arion", "...")
                );
                spaceStoryText.Add(
                    ("Arion", "There, that should do it. Now, all we need to do is approach it and speak.\nIt should be able to understand us, and we should be able to understand it.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "I hope this works. I don't want to anger it and end up as plant food.")
                );
                spaceStoryText.Add(
                    ("Arion", "Just in case, let me analyze its movements and weapons first. Then, we'll formulate a strategy based on its weaknesses.\nRemember, timing and precision are crucial.")
                );
                spaceStoryText.Add(
                    ("Arion", "...")
                );
                spaceStoryText.Add(
                    ("Arion", "Well well well, looks like we have a big, bad tree guarding our path. But fear not, my dear Otter, for we have the Singularity Crystal!\nWith just a snap of your fingers, you can suck in those pesky little stones and pelt them right back at this creature.\nIt's almost too easy, really. Just be careful of those roots. We don't want to become pincushions, now do we?")
                );
                spaceStoryText.Add(
                    ("Arion", "...")
                );
                spaceStoryText.Add(
                    ("Arion", "Okay now. Don't worry, P. Otter. I'll handle the negotiations. Just follow my lead.")
                );
                break;
            case 14:
                spaceStoryText.Add(
                    ("Arion", "Well, well, well, P. Otter. It seems like you've done it again.\nYou destroyed Justin Treeber and got us the power regulator.\nI must admit, I didn't think you had it in you.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Thanks, Arion. I couldn't have done it without you.\nBut let's focus on getting the ship fixed first.")
                );
                spaceStoryText.Add(
                    ("Arion", "Of course, my dear Otter. But don't worry, with my expertise, the ship will be fixed in no time.\nAnd once we're finished, we'll be able to track down your date using the latest scanning technology.\nTrust me, with me on your team, we'll have 'SweetAngel_93' in your arms in no time.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "I hope so. I've been searching for so long, and I'm starting to lose hope.")
                );
                spaceStoryText.Add(
                    ("Arion", "Nonsense, my friend. With me by your side, we'll find her in a flash.\nNow, let's hurry back to the ship and get started on those repairs.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Agreed. And thank you again, Arion. You're the best.")
                );
                spaceStoryText.Add(
                    ("Arion", "Yes, I am, aren't I? But don't worry, P. Otter, I'll still allow you to bask in my glory. Let's go!")
                );
                break;
            case 15:
                spaceStoryText.Add(
                    ("Arion", "Ah, there we go! Ship's all repaired and ready to fly. Just in time for me to locate your precious 'SweetAngel_93'.\nDon't worry, P. Otter, you can thank me later for being such a genius engineer.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Thank you, Arion. I really appreciate your help. So, where is the signal coming from?")
                );
                spaceStoryText.Add(
                    ("Arion", "Well, according to my scans, the signal is emanating from the other side of that massive rock face where you woke up.\nYou'll need to use the Gravity Catalyst to climb the wall and follow the path from there.\nBut don't worry, I'm sure even you can handle that.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "I hope so. This is really important to me.")
                );
                spaceStoryText.Add(
                    ("Arion", "Of course it is, P. Otter. That's why you need someone as skilled as me to help you out.\nBut let's not forget, I have my own interests in this.\nI want to see what kind of 'Gravitynder'-user could capture the heart of someone like you. I bet she's just as amazing as I am.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Uh, sure, Arion. Whatever you say.")
                );
                spaceStoryText.Add(
                    ("Arion", "Now, don't dawdle. The sooner you find your date, the sooner we can get off this backwater planet and I can go back to doing something more productive.")
                );
                break;
            case 16:
                spaceStoryText.Add(
                    ("P. Otter", "Arion, this doesn't look good.\nThe path ahead is blocked by this rock face.")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh no, what ever shall we do? Perhaps you should try punching it to see if it moves.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "There has to be another way around this.")
                );
                break;
            case 17:
                spaceStoryText.Add(
                    ("Arion", "Well rip, P. Otter.")
                );
                break;
            case 18:
                spaceStoryText.Add(
                    ("Tree Creature", "WHO DAWES DISTUWB *screeches* MY WEST?!! I AM JUSTIN TWEEBEW, THE P-PWOTECTOW OF THIS WAND.\nY-Y-YOU AWE NyOT WEWCOME >w< HEWE!")
                );
                spaceStoryText.Add(
                    ("Arion", "Ah, Justin *starts twerking* T-Tweebew, I pwesume?!?1 D-D-Dewightfuw t-to make youw acquaintance.\nI am Awion, and this is my esteemed cowweague, P. Ottew.\nWe awe mewewy passing thwough youw tewwitowy :3 and mean nyo hawm t-to you ow (・`ω´・) youw wand.")
                );
                spaceStoryText.Add(
                    ("Justin Treeber", "YOUW WOWDS MEAN NyOTHING TO ME, *sees bulge* AWION. I CAN SENSE YOUW INTENTIONS, AND OwO I KNyOW Y-Y-YOU S-SEEK TO HAWM THIS WAND.\nI WIWW NyOT WET THAT *whispers to self* HAPPEN.")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh, come nyow, Justin. Nyo nyeed t-to get so wowked >w< up. We awe nyot h-h-hewe t-to hawm anyonye ow (・`ω´・) anything.\nWe mewewy nyeed t-to pass thwough this awea t-to weach ouw powew weguwatow.\nSuwewy, we *notices buldge* c-can come t-to an undewstanding.")
                );
                spaceStoryText.Add(
                    ("Justin Treeber", "UNDEWSTANDING!? THEWE WIWW BE NyO UNDEWSTANDING BETWEEN US, AWION. I WIWW NyOT AWWOW Y-Y-YOU TO PASS.\nY-Y-YOU AND OwO YOUW COWWEAGUE M-M-MUST WEAVE NyOW, OW FACE THE CONSEQUENCES.")
                );
                spaceStoryText.Add(
                    ("Arion", "Consequences?!! My deaw Justin, I think you undewestimate ouw capabiwities.\nSo, why don't you just *whispers to self* step aside and wet us pass?")
                );
                spaceStoryText.Add(
                    ("Justin Treeber", "YOU THINK YOUW TECHNyOWOGY CAN MATCH THE POWEW OF NyATUWE?!! Y-Y-YOU AWE FOOWS.\nI WIWW NyOT MOVE ASIDE, AND OwO I WIWW NyOT WET Y-Y-YOU PASS.\nY-Y-YOU WIWW HAVE TO FACE ME AND OwO MY FUWY.")
                );
                spaceStoryText.Add(
                    ("Arion", "OwO Ha?!! You think you c-can intimidate us, Justin!!11 We've faced much wowse than a tawking twee befowe.\nBut vewy weww, if you insist on a fight, *notices buldge* we'ww give you onye.\nP. Ottew, get w-w-weady. Justin, you'we about t-to weawn why you don't mess with Awion and P. Ottew.")
                );
                break;
            case 19:
                spaceStoryText.Add(
                    ("Arion", "Well, what are you waiting for? Are you scared, P. Otter?\nJust tell the darn elevator to take you down to your date already.\nTime is ticking, and I don't have all day to wait for you.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Alright, alright, I'm on it. No need to get your circuits in a twist.")
                );
                spaceStoryText.Add(
                    ("Arion", "See? That wasn't so hard, was it? Now hurry up and find that 'SweetAngel_93' of yours so we can get out of this backwater planet.")
                );
                break;
            case 20:
                spaceStoryText.Add(
                    ("Arion", "Looks like our little Otter is finally making progress. Scans show that your precious SweetAngel_93 is just around the corner.\nAnd what do we have here? A switch? Hmm, who knows what it does. But you know what, Otter?\nYou've been wasting enough time already. Just pull it and get on with it.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "But what if it activates some kind of trap or something?")
                );
                spaceStoryText.Add(
                    ("Arion", "Oh please, P. Otter. You're such a worrier. Just shoot at it already!")
                );
                break;
            case 21:
                spaceStoryText.Add(
                    ("SweetAngel_93", "Oh my goodness, you made it! I can't believe it, I've been waiting for you for what seems like forever.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "It wasn't easy, but I had to find you.")
                );
                spaceStoryText.Add(
                    ("SweetAngel_93", "Well, you certainly did. And let me just say, you're even cuter in person than you were in your profile picture.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Thank you, you're pretty amazing yourself.")
                );
                spaceStoryText.Add(
                    ("SweetAngel_93", "So, what's next for us, you got any plans?")
                );
                spaceStoryText.Add(
                    ("P. Otter", "I was actually thinking about that. I've always dreamed of traveling through the universe and discovering new worlds.\nWhat do you think about joining me on that journey?")
                );
                spaceStoryText.Add(
                    ("SweetAngel_93", "I think that sounds like the most amazing idea I've ever heard. Let's do it!")
                );
                spaceStoryText.Add(
                    ("P. Otter", "Great! We'll have to go to my ship first, and after that, the universe is our playground.")
                );
                spaceStoryText.Add(
                    ("SweetAngel_93", "I can't wait! Imagine all the things we'll see and discover together.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "I can imagine it. And I can't think of anyone I'd rather share those experiences with than you.")
                );
                spaceStoryText.Add(
                    ("SweetAngel_93", "You're too sweet. But seriously, let's make this happen.")
                );
                spaceStoryText.Add(
                    ("P. Otter", "We will. I promise.")
                );
                break;
        }
        if(uwuMode) UwuifiyStory();
        stopText = false;
    }

    public bool CheckStoryRequirements(int nextStoryPart){
        switch (nextStoryPart)
        {
            case 1:
                return !player.GetComponent<Player>().unlockedWeaponModes[1] && !visitedArion;
            case 6:
                return !player.GetComponent<Player>().doubleJump;
            case 9:
                return GameObject.FindWithTag("GolemBoss").GetComponent<StoneGolemBoss>().destroyedBenders;
            case 15:
                return GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().dead;
            case 16:
                return !player.GetComponent<Player>().unlockedWeaponModes[1] && visitedArion;
            case 17:
                if(storyPartIndex == 3){
                    stopCurrentText();
                    return true;
                }
                return false;
            case 18:
                GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().freeze = true;
                return true;
            default:
                return true;
        }
    }

    private void stopCurrentText(){
        stopText = true;
        GetComponent<Animator>().SetBool("active", false);
        healthBar.alpha = 1;
        weaponModeDisplay.alpha = 1;
        weaponWheel.alpha = 1;
        charIndex = 0;
        StopCoroutine(storyCoroutine);
    }

    private void UwuifiyStory(){
        for(int i = 0; i < spaceStoryText.Count; i++){
            var newText = spaceStoryText[i].Item2;
            newText = newText.Replace("Arion", "Awion " + UwuifySymbols("Arion"))
            .Replace("P. Otter", "P. Ottew " + UwuifySymbols("P. Otter"))
            .Replace("What?", "What? ∑(ﾟﾛﾟ〃)")
            .Replace("help", "hewp *sweats*")
            .Replace("heavy", "heavy *screams*")
            .Replace("could", "c-could")
            .Replace("but", "b-but")
            .Replace("maybe", "m-maybe")
            .Replace("l", "w")
            .Replace("L", "W")
            .Replace("r", "w")
            .Replace("R", "W")
            .Replace("no", "nyo")
            .Replace("No", "Nyo")
            .Replace("mo", "myo")
            .Replace("Mo", "Myo")
            .Replace(".", UwuifySymbols("."))
            .Replace("?", UwuifySymbols("?"))
            .Replace("!", UwuifySymbols("!"));
            spaceStoryText[i] = (spaceStoryText[i].Item1, newText);
        }
    }

    private string UwuifySymbols(string symbol){
        int rand = UnityEngine.Random.Range(0, 4);

        switch (symbol)
        {
            case "!":
                var exclamations = new List<string>(){
                    "!?", "?-?-?!?1", "Σ(•。•)" 
                };
                return exclamations[rand%exclamations.Count];
                break;
            case "?":
                var questionmarks = new List<string>(){
                    "(?_?)", "(・・?)", "(・∀・)?", "( ‥)?", "(•ิ_•ิ)?", "ლ(ಠ_ಠ ლ)"
                };
                return questionmarks[rand%questionmarks.Count];
                break;
            case ".":
                var dots = new List<string>(){
                    ".-.-.", "♡(>ᴗ•)", "¯\\_(ツ)_/¯"
                };
                return dots[rand%dots.Count];
                break;
            case "P. Otter":
                var potter = new List<string>(){
                    "/╲/\\╭(ఠఠ益ఠఠ)╮/\\╱\\", "(∩ᄑ_ᄑ)⊃━☆ﾟ*･｡*･:≡( ε:)", "(╯°益°)╯彡┻━┻", "(⌐■_■)", "( ＾▽＾)っ✂╰⋃╯"
                };
                return potter[rand%potter.Count];
                break; 
            case "Arion":
                var arion = new List<string>(){
                    "(づ￣ ³￣)づ", "(づ◡﹏◡)づ", "|ʘ‿ʘ)╯", "ฅ(^◕ᴥ◕^)ฅ"
                };
                return arion[rand%arion.Count];
                break; 
            default:
                return symbol;
                break;
        }
    }

    public IEnumerator PlayStory(){
        player.GetComponent<Player>().lockPlayerControl = true;
        healthBar.alpha = 0;
        weaponModeDisplay.alpha = 0;
        weaponWheel.alpha = 0;
        GetComponent<Animator>().SetBool("active", true);
        NextText();
        storyIndex++;
        
        while(!stopText){
            if(Input.GetKeyDown(KeyCode.Space)){
                NextText();
                storyIndex++;
            }
            yield return null;
        }
    }

    private void NextText(){
        GetComponent<CanvasGroup>().alpha = 1;
        if(storyIndex > spaceStoryText.Count || (storyIndex == spaceStoryText.Count && !writing)){ 
            StopCoroutine(storyCoroutine);
            GetComponent<Animator>().SetBool("active", false);
            healthBar.alpha = 1;
            weaponModeDisplay.alpha = 1;
            weaponWheel.alpha = 1;
            player.GetComponent<Player>().lockPlayerControl = false;
            if(storyPartIndex == 18){
                GameObject.FindWithTag("TreeBoss").GetComponent<TreeBoss>().freeze = false;
            }
            else if(storyPartIndex == 21){
                StartCoroutine(FadeToCredits());
            }
            return;
        }

        if(writing){
            storyIndex--;
            writing = false;
            charIndex = finalText.Length;
            headerField.text = spaceStoryText[storyIndex].Item1;
            textField.text = spaceStoryText[storyIndex].Item2;
        } else if(storyIndex < spaceStoryText.Count){
            textField.text = "";
            charIndex = 0;
            headerField.text = spaceStoryText[storyIndex].Item1;
            finalText = spaceStoryText[storyIndex].Item2;
            ReproduceText();
            writing = true;
        }  
    }

    private void ReproduceText()
    {
        if(stopText) return;
        //if not readied all letters
        if (charIndex < finalText.Length)
        {
            //get one letter
            char letter = finalText[charIndex];

            //Actualize on screen
            textField.text += letter;

            //Play text sound
            textSound.Play();

            //set to go to the next
            charIndex += 1;
            StartCoroutine(PauseBetweenChars(letter));
        } else {
            writing = false;
        }
    }

    private IEnumerator PauseBetweenChars(char letter)
    {
        switch (letter)
        {
            case '.':
                yield return new WaitForSeconds(0.5f);
                ReproduceText();
                yield break;
            case ',':
                yield return new WaitForSeconds(0.05f);
                ReproduceText();
                yield break;
            case ' ':
                yield return new WaitForSeconds(0.05f);
                ReproduceText();
                yield break;
            default:
                yield return new WaitForSeconds(0.05f);
                ReproduceText();
                yield break;
        }
    }

    private IEnumerator FadeToCredits(float fadespeed = 0.3f)
    {
        var BlackOutSquare = GameObject.Find("/UI_Ingame/black_screen");
        Color objectColor = BlackOutSquare.GetComponent<Image>().color;
        float fadeAmount;

        objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, 0f);
        while (BlackOutSquare.GetComponent<Image>().color.a < 1)
        {
            fadeAmount = objectColor.a + (fadespeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            BlackOutSquare.GetComponent<Image>().color = objectColor;
            yield return null;
        }

        SceneManager.LoadScene("Credits");
    }
}
