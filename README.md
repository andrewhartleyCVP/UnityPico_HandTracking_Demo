# Unity_HandTracking_Demo

- If you have any questions/comments, please visit [**Pico Developer Support Portal**](https://picodevsupport.freshdesk.com/support/home) and raise your question there.

## Environment:

* Unity2020.3.43f1
* PICO SDKv2.1.4
---
Features:



* <span style="text-decoration:underline;">Button Poke </span>- Simple Box collider set in the “Hands” Layer which interacts with the “Interactive” layer. Can work with the thumb, index or the palm. So no need for tag
    * Various functions
        * ButtonCtrl(Arcade Mode)
            *  ArcadeManager.instance.StartNewGame()
        * ButtonCtrl(Tutorial Mode)
            * TutorialManager.instance.StartStep0();
        * ButtonCtrl(Arcade Active)
            * Starts a singleton to finish animation before destroying the button
        * ButtonCtrl(Tutorial Active)
            * Since this object is in the scene already we just disable it
        * ButtonCtrl(End Mode)
            * Ends and resets both games so we can use one button for both modes
            * ArcadeManager.instance.GameOver();
            * TutorialManager.instance.EndTutorial();
* <span style="text-decoration:underline;">Grab</span> - Grab function using a collider on the index and thumb under the “Hands” layer interacting with the “Interactive” Layer. Index and Thumb have Tags with the thumb carrying FingerTipCtrl. Used to carry the thumb offset position. I do not know which combination of thumb and index will interact with the coin, by storing the thumb off set pos on the thumb, this makes it easier to just grab the right position without having to add extra logic and tags for which thumb
    * FingerTipCtrl
        * Stores thumb offset position
    * GrabCoinCtrl
        * Handles the different states of the coin
            * Waiting
                * Waits for the thumb and index to enter its trigger area to move into grabbed state
            * Grabbed
                * Lets the player move the coin around and it only interacts with a Hat
                    * Checks if we interacted with the correct hat or not
                    * Make a new coin
            * Falling
                * If we hit a hat
                    * Checks if we interacted with the correct hat or not
                * If we hit part of the room
                    * Make a coin dropping sound
                * Make a new coin
    * HatTrick
        * Handles when we move the correct coin to the correct hat
            * Different logic for tutorial mode vs arcade mode
            * Plays a light ribbon particle effect
            * Arcade mode
                * Updates score
            * Allows the animation to finish 
* <span style="text-decoration:underline;">RayClick/ThumbClick</span> - HandClickCtrl controls the Thumb click feature and draws data from PXR_Hand script.
    * HandClickCtrl
        * UpdateRaycast()
            * Looks for the firezone area that is set up behind the target area
            * Changes Raypointer color
            * Adds lasers when valid
                * Laser changes color when on reload
        * UpdateHandTracking()
            * Tracks when user has initiated the thumb click
            * Strength of the Ray is determined by thumb position
                * We use the thumb position of this click gesture to fire and to reload
        * ClickFire()
            * Creates a ball and fires from the raypoint forward
* Tutorial Mode - Mode controlled by Tutorial Manager Singleton
    * Each step is halted until the correct step is done to move on to the next step
        * User goes through Game mechanics
            * Ideal hand position
            * How to shoot at a target
            * How to change coin value
            * How to grab a coin
            * Coin needs to match the hat
            * Moving coin to hat
            * Pressing the red button
* Arcade Mode - Mode controlled by Arcade Manager Singleton
    * IntroSetup() - Sets up components
        * Gets a new seed for random number generator
    * StartNewGame/GameStart() - Sets up area
        * zoneList is an array of transform for the hat to spawn in
    * AddScoreHat() - Randomly selects a prefab and a zone number from 1 to the length of the list. Instantiates a new hat prefab at selected position 0 is reserved for the previous position so we don’t repeat the same spawn location twice in a row
        * ZoneSwap - moves the current spawn position to index 0
    * After a new hat goes into the play area
    * Player will shoot the target - TargetCtrl
    * Arcade manager makes a new coin - MakeNewCoin
    * Player will grab the coin - GrabCoinCtrl
    * After moving coin to correct hat - HatTrick
    * HatTrick will call ArcadeManager to Updatescore and run AddScoreHat()
    * Repeats until time runs out
