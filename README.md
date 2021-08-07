# Midi-Visualiser-Project
Midi Visualiser in the form of a very basic microgame.

Game is made in unity and you can import the files into a unity project.
Was made long time ago and hasn't been updated since and is very crude way of completing a midi visualiser in the form of a game.
The concept is there but needs refinement for a more enjoyable experience.

To add songs you will need the midi file (most seem to work but some don't get parsed correctly since midi files are stored very weirdly)
Then you will need to add a song file for it to play at the same time in the music folder. Then add the name of the file on the player eventspawner.

## Game Idea

The Overall idea of this game is to make it load in any midi file that the players wants then it will generate a round where they have to destroy incoming enemies. But the enemies spawns are based on the midi files notes. A single note is a single enemy and should reach the player at the time the note is played or should be spawned when the note is played. I implemented it so that is hits the player as the note is played but this should be experimented on for a more enjoyable experience. If you stand still you can see how the notes are played just as the enemy triangles hit the player. The game was more a proof of concept than a intended game. The concept I feel is there but needs lots of adjustments to be scaled properly for an enjoyable experience. For example the notes shouldn't be 1 to 1 but maybe something similar so you still visualise the midi song into a game. At the moment the difficulty is purely based on the midi file, therefore more harder levels for the same song aren't posssible with the current set up. For futhering this project the best approach is to take the midi parser and files used to calcuate when to spawn the enemies and expand upon it and rework the rest entierly.

## Visual Sneak Peak

![SneakPeek](/readmeImages/SneakPeek.png)

