# starcatcher
physical game of catching falling stars

Unity 2021.2.11f1

# Plant Notes

Andy is doing work on the plants. They are currently visible to projector 2, which maps to Display 2 when running the game.

To avoid conflict in the scene, the plant manager is stored as a prefab and instantiated into the scene when the game starts. You can find it in `Assets/Plants/PlantsManager`. Go there to change settings for the plants.

I've tried to add mouse-over tooltips to the variables in PlantsManager.

To make changes to PlantsManager while the game is running, make sure to select the instance in the heirarchy so you are editting that and not the prefab. Changing the PlantsManager prefab while the game is running won't do anything. 