# GDIM33 Vertical Slice
## Milestone 1 Devlog
<img width="1311" height="376" alt="image" src="https://github.com/user-attachments/assets/553093ab-1ef2-49c4-93da-cc692454ac6a" />
- This Enemy script machine uses Get Component Node to access the main enemy script Killable.cs, and calls two functions from the script. The Distance To Target Node returns the distance between this enemy and player. If this value is less than 2, then the if node will evaluate to true, and invoke the Follow Node, which uses the locator on Player Controller to target and follow the player. The Get Component node is called once using On Start because the script and functions are will not change. The same reference is called on every single update loop.
<img width="1231" height="815" alt="image" src="https://github.com/user-attachments/assets/4dcef5cf-e0bb-4bd4-81b9-8d95e4b0a2e3" />
- I updated the enemy bubble in my breakdown, adding the three smaller blue bubbles that indicate the 3 enemy states of a state machine. For now, I implemented the state transition from idle to chase that affects the enemy's behavior. This transition occurs when the enemy received damage from player. Besides animation, it affects what that enemy is allowed to do. So far, I'm allowing the enemy to chase after the player after entering Chase state. In the future, I want the Chase state to be the only state that allows attack behavior, and the Idle will have it randomly wonder around the map.

## Milestone 2 Devlog
### 1

Step 1: Build dialogue system that can open shop UI

1. Write a scriptable object node that stores dialogue for an NPC
2. Write a dialogue interpretation script that displays the lines and choices
3. Build a single scriptable object from the node for merchant NPC
4. Edit the interpreter to let it open shop or end dialogue when detecting keywords
5. build a empty shop UI that lets you close it


Step 2: Build a Inventory system that stores items and money

1. Create an inventory script that stores a list of inventory and int for money
2. each item in list stores item name, price, and count.
3. Create game object prefabs of class Item, which allows player to pick up and store into inventory list.
4. Edit enemy script to instantiate these items based on drop rate of each


Step 3: Build a shop system that lets you buy/sell items with UI buttons

1. write a scriptable object that stores shop item data
2. write functions for buy and sell button to switch tabs
3. create the buy and sell tabs with a scrollable view of buttons
4. write functions for buy and sell to change shop stock/player inventory
5. write functions for buy and sell to cause other effects (death, attack buff, ammo)

### 2
The break down was very helpful, because it was detailed enough to the point where I can just follow them without thinking about what to do next. If anything, I would've made them a bit more detailed on what classes stored what with what data structure. I had to search up a lot of ways to store data since simple lists weren't efficient for the things I need to do with inventory and shop items.

### 3
<img width="1162" height="575" alt="image" src="https://github.com/user-attachments/assets/5ef5ad8f-d858-496a-b450-f9865e7aaa9d" />
I'm calling the Attack C# method from the script Killable.cs through the EnemyBehavior graph. I used this graph to calculate cool down for enemy attack interval, and used a timer node as countdown from "when enemy got close enough to player for attack" to "when the Attack C# method is called". The DistanceToTarget C# function is recycled from the last state machine, which calculates the distance between the two to only initiate attack if the distance is less than 3, and only deal damage to player if after 0.5 seconds, the distance between is less than 2.

### 4
I used scriptable object for dialogue branching. The code for scriptable object is Asset/Scripts/Dialogue/DialogueNode.cs, and the actual scriptable object is Asset/Scripta/Dialogue/Merchant.asset. I'm attaching the NPC script in Asset/Scripts/Dialogue/NPC.cs on merchant NPC gameObject to pass this scriptable object to PlayerController.cs on player game object when player presses E to talk, which is further passed to the UI manager to be interpreted with HandleDialogue(NPC _npc) and other related functions under the Dialogue region of UIManager.cs. Ultimately, this lets me load to the next block of dialogue based on the index given by each choice, and when the line is the keywords END or SHOP, the LoadLine function in UIManager.cs will call EndTalk function from PlayerController to close the dialogueUI, or open the shop UI.


## Milestone 3 Devlog
1. For this water shader graph, I utilized the time-based texture transform from the in-class activity that covered fire texture to simulate movement. I also used the normal vector and view direction nodes to calculate the mesh's surface normal relative to the camera direction. This makes the parts of the mesh facing away from the camera be darker, and more transparent under the Additive blending mode. This makes a smooth out effect on the rim of the object. This shade graph is located in Assets/Materials/Water.shadergraph. This material is applied on the merchant NPC... because he's not a human!!! There are two other shaders in the same folder, where RimLight is responsible for enemy's red glowing effect, and ItemLight is responsible for the gold light rising from enemy's loot drop.
 <img width="1154" height="902" alt="image" src="https://github.com/user-attachments/assets/3bb998ad-d430-43d5-b15f-46ded761c7e0" />

2. I received feedback saying that the enemy would still attack when you talk to the NPC. This was intentional, so I added a new section of code in the UI manager and player script to force quit the dialogue after the player recieved damage. Now, the E to Talk UI also disappears after opening a dialogue to avoid confusion. The UI was updated to make ammo and player status more obvious. Some dialogue UI was moved to avoid overlapping with general UI to improve readability. Damage popup is now instantiated at the hit point of player's raycast instead of directly on the center of enemy mesh. The light effect and emission of loot drop was made brighter to look more obvious, and the merchant's dialogue also tells the player to pick up enemies' drops.

3. I added a new NPC who gives out quest to the player. It activates different dialogues based on the number of kills player has. To aid this new feature, I reworked the terrain to divide the game into two areas: a small starting area with a single enemy for quest 1 and a bigger area with 4 more enemies for quest 2. To kill enemies in the second area, the player must sell and buy resources from the merchant. After killing all enemies, the player can go back to the Quest Giver NPC to submit quest and complete the game loop, which activates the end of game UI and pause the game. Lastly, I added grass and flower to the terrain to make the environment more pleasant.

## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- Cite any external assets used here!
