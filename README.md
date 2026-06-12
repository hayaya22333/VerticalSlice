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
1. The core gameloop is around completing combat quest from NPC. The player talks to an NPC, kills enemy, and submit quest by talking to the NPC again. The process happens for a total of 3 times with different NPCs, and submitting a final quest will draw an end to the game loop. Resources are limited in the game, so the player will have to talk to the merchant NPC and sell enemy loot before getting enough ammo to complete the final quest. After killing an enemy, the player will level up. Leveling up increases the player's damage per hit, which slightly compensates for players who don't enjoy resource management as much. This final product resembles my vertical slice planning in the combat, resource management, and leveling design. I cut out the scene change and boss fight. The full gameplay is basically the same thing, but implement more scene sets and enemies with unique animation and attack pattern. It will have the same kind of enemy-killing quests for the player.
2. The enemy's shader is changed when it takes damage from any source. The change occurs in Assets/Scripts/Characters/MaterialSwitcher.cs, where it holds a method called SwitchOn() and SwitchOff(), that changes the material of all mesh for the enemy to have the rim of the model glow in red. The SwitchOn() method is called from the Assets/Scripts/Characters/Killable.cs attached to the same enemy object to switch on the rim light material when receiving damage. Within the MaterialSwitcher script, SwitchOn() starts a countdown and calls SwitchOff() after the countdown ends. Visually, the rim effect flashes for 0.5 second when taking damage.
3. I worked this project from the core and basic mechanism gradually to the more complex ones. First, I worked on mechanisms like movement, shooting, and receiving damage. Then, I used created variations of these mechanisms on player and enemy object, connecting them together to let different game object affect each other. In my PlayerController script, there's a section titled "Connect", which focuses on interation with other game object, like picking up items, damaging enemies, and talking to NPCs. In some cases, multiple functions will use the same basic mechanism. For example, the trigger collider on player is used for damage detection, NPC detection, and item detection. Although these use separate colliders, they use the same technique to only detect the desired colliders and ignore what's irrelevant. It's helpful to implement them together, since similar problems can be solved faster together. As much as I try to make the code easily scalable, revisiting code from early development in week 10 is more likely to cause bug than just finishing similar functions together. After these basic functions, I started working on a separate system, which is the dialogue and shop functions. They don't rely on previous code about in-game interactions, so it's safe, and even more convenient to implement them independent from previous in-game functions. Lastly, I connected independent systems together, allowing player stats to affect and be affected by interactions in dialogue or shop. On a separate note, it's important to think about what data types to use to connect different systems. Otherwise, it'll be a pain to refactor two systems to match their sockets and plugs.

\tI'll always be using the bubble diagram to plan for future projects. I'm a visual learner, and drawing things out helps free my thought storage in planning a complex game system. The task break-downs are slightly less helpful in the form they are right now. One of the short-coming i listed plans before writing any actual code can cause a large part of the plan to go south if one of the planned code structure isn't supported by c#. Instead of using break-downs directly, I'd like to slightly revise them to be more visual, like a flowchart, and focus on noting down general ideas about problem solving. For example, I had to think about how to make hits on enemies deal different damage based different hitbox. I had it in my mind before writing the actual code, but it'd be helpful to have them on paper.

Breaking an entire project into large chunks gives me an estimate on how much work there are, and breaking each chunk into smaller steps lets me estimate, with the time given for the project, how much work needs to be finished per week, or per day. If I have to implement one big chunk of code every day, that's probably not realistic in a quarter with 4 other courses. Though, that'll probably be doable when the 10-weeks work time is during summer break.

I'd say that my project went well for 80%. I followed my plan to work from simple mechanism to complex systems, and worked on separate system independently. The part that didn't go that well was my estimate on workload. I was in a slight rush for later development, since making 3d models and creating textures for shader graphs was very time-consuming. I enjoy exploring shadergraph properties, but for future projects, instead of making my own texture, I recently discovered potentials of unity's built-in textures. They're especially convenient when it comes to animating the texture with the time node. In a future project, I'd like to annotate my bubble diagram to include an estimate of how much time each section will take to implement. This is more doable right now, since I'm more familiar with tools like shader graph, blender, and c# in general. Nevertheless, it's important to reduce the number of unfamiliar tools used in a project to avoid overly extended work time.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- Cite any external assets used here!
