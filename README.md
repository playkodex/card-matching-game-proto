# Card Matching Game

## Features

### Core Gameplay

* Basic card-flipping system with simple matching logic
* Several grid sizes for testing different layouts
* Cards resize automatically to work on any screen
* Straightforward and easy-to-test matching flow

### Scoring System

* Matches award points
* Quick matches create simple combo bonuses
* Wrong matches reduce the score
* Tracks the best combo achieved during a run

### UI & Feedback

* Basic UI showing score, time, moves, and combo
* Simple end screen that shows test results
* Sound effects for flips, matches, and mistakes

### Save & Load

* Prototype auto-save that stores basic game state
* Saves card states, score, time, and move count
* Automatically reloads the last session on startup
* Saves persist between sessions for testing

### Polish & Performance

* Smooth test animations using DOTween
* Light and responsive for prototype builds
* Clean and stable with no known issues

## Controls

* Clicking flips a card
* “Play Again” resets the prototype board

## Technical Details

### Built With

* Unity (LTS or newer)
* DOTween for simple animation handling

### Key Scripts

* **CardGameManager.cs** – Manages the basic prototype logic
* **Card.cs** – Card behavior, flipping, and animations
* **ScoreManager.cs** – Prototype scoring and combos
* **UIManager.cs** – UI updates during gameplay
* **GameOverUI.cs** – Simple prototype end screen
* **AudioManager.cs** – Handles sound playback
* **SaveLoadManager.cs** – Stores and loads prototype data

### Platform Support

* Windows, macOS, Linux
* Android and iOS (prototype testing)

## Setup

* Open the project in Unity
* Import DOTween
* Add audio clips to the AudioManager
* Link UI elements through the Inspector
* Build and run for testing
