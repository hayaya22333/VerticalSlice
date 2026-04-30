# GDIM33 Vertical Slice
## Milestone 1 Devlog
<img width="1085" height="339" alt="image" src="https://github.com/user-attachments/assets/c1957725-9541-40ca-bf85-c18ba2f7e97f" />
- This Enemy script machine uses Get Component Node to access the main enemy script Killable.cs, and calls two functions from the script. The Distance To Target Node returns the distance between this enemy and player. If this value is less than 2, then the if node will evaluate to true, and invoke the Attack Node, which uses the locator on Player Controller to deal damage to player, equal to the enemy's atk specified in script. The Get Component node is called once using On Start because the script and functions are will not change. The same reference is called on every single update loop.
<img width="1231" height="815" alt="image" src="https://github.com/user-attachments/assets/4dcef5cf-e0bb-4bd4-81b9-8d95e4b0a2e3" />
- I updated the enemy bubble in my breakdown, adding the three smaller blue bubbles that indicate the 3 enemy states of a state machine. For now, I implemented the state transition from idle to chase that affects the enemy's behavior. This transition occurs when the enemy received damage from player. Besides animation, it affects what that enemy is allowed to do. So far, I'm allowing the enemy to chase after the player after entering Chase state. In the future, I want the Chase state to be the only state that allows attack behavior, and the Idle will have it randomly wonder around the map.

## Milestone 2 Devlog
Milestone 2 Devlog goes here.
## Milestone 3 Devlog
Milestone 3 Devlog goes here.
## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- Cite any external assets used here!
