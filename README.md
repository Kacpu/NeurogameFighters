# Neurogame Fighters
"Fighters" game with elements of machine learning based on a neural network and a genetic algorithm.

Two fighters with a certain number of lives move around the board and shoot the opponent to survive.

Controls (Player 1, Player 2)

* Move forward - **W, Up Arrow**
* Move backward - **S, Down Arrow**
* Turn left - **A, Left Arrow**
* Turn right - **D, Right Arrow**
* Shoot - **Space, K**

## Technologies
* WPF
* .NET 5
* C#
* XAML

## Launch
In the publish directory run the `NeuroFighters.exe` file or go to the NeurogameFighters directory with the .csproj file and type `dotnet run` in terminal.

## Features
* Player vs Player mode
* Player vs AI Mode - AI controlled deterministically or by a neural network if it was previously created in the learning mode
* Learning AI mode - population of neural networks trained by the genetic algorithm, at the end the best neural network is saved

## Screenshots
<p align="center">
  <img src="https://user-images.githubusercontent.com/61886846/176968763-a3b19d6c-19b3-40c9-8dc3-fed66af7695c.png">
  <i>PvP Mode</i>
  </br></br>
  <img src="https://user-images.githubusercontent.com/61886846/176971070-20506362-72a2-4de2-8334-d483cfbbc850.png">
  <i>Learning AI mode</i>
</p>


