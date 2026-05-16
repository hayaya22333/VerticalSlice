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
Milestone 3 Devlog goes here.
## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- Cite any external assets used here!
