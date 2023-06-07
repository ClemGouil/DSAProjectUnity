using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.
	

public class BoardManager : MonoBehaviour
	{
		// Using Serializable allows us to embed a class with sub properties in the inspector.
		[Serializable]
		public class Count
		{
			public int minimum; 			//Minimum value for our Count class.
			public int maximum; 			//Maximum value for our Count class.
			
			//Assignment constructor.
			public Count (int min, int max)
			{
				minimum = min;
				maximum = max;
			}
		}
		

		public int columns = 8; 										//Number of columns in our game board.
		public int rows = 8;										//Number of rows in our game board.
		public Count wallCount = new Count (5, 9);						//Lower and upper limit for our random number of walls per level.
	    public GameObject exit;
		public GameObject[] floorTiles;									//Array of floor prefabs.
		public GameObject[] WallTiles;									//Array of Decor prefabs.
		public GameObject[] enemyTiles;								//Array of enemy prefabs.
		public GameObject[] outerWallTiles;                       //Array of outer tile prefabs.
		

		
		private Transform boardHolder;									//A variable to store a reference to the transform of our Board object.
		private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.
		
		
		//Clears our list gridPositions and prepares it to generate a new board.
		void InitialiseList ()
		{
			//Clear our list gridPositions.
			gridPositions.Clear ();
			
			//Loop through x axis (columns).
			for(int x = 1; x < columns-1; x++)
			{
				//Within each column, loop through y axis (rows).
				for(int y = 1; y < rows-1; y++)
				{
					//At each index add a new Vector3 to our list with the x and y coordinates of that position.
					gridPositions.Add (new Vector3(x, y, 0f));
				}
			}
		}
		
	void GenerateLevelFromString(string levelData)
	{
		string[] lines = levelData.Trim().Split(' ');

		int width = lines[0].Length;
		int height = lines.Length;

		boardHolder = new GameObject("Board").transform;

		for (int y = -1; y < height - 1; y++)
		{
			for (int x = -1  ; x < width - 1; x++)
			{
				char symbol = lines[y+1][x+1];

				GameObject toInstantiate = null;

				if (symbol == 'E') {
					toInstantiate = exit;
				} else if (symbol == '+') {
					toInstantiate = outerWallTiles [1];
				} else if (symbol == '-') {
					toInstantiate = outerWallTiles [3];
				} else if (symbol == '*') {
					toInstantiate = outerWallTiles [2];
				} else if (symbol == 'x') {
					toInstantiate = outerWallTiles [4];
				} else if (symbol == 'L') {
					toInstantiate = outerWallTiles [5];
				} else if (symbol == 'R') {
					toInstantiate = outerWallTiles [6];
				} else if (symbol == 'T') {
					toInstantiate = outerWallTiles [0];
				} else if (symbol == 'B') {
					toInstantiate = outerWallTiles [7];
				} else if (symbol == '1') {
					toInstantiate = floorTiles [0];
				}else if (symbol == '2') {
					toInstantiate = floorTiles [1];
				}else if (symbol == '3') {
					toInstantiate = floorTiles [2];
				}else if (symbol == '4') {
					toInstantiate = floorTiles [3];
				}else if (symbol == '5') {
					toInstantiate = floorTiles [4];
				}

				GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
				instance.transform.SetParent(boardHolder);
			}
		}
	}

		
		
		//RandomPosition returns a random position from our list gridPositions.
		Vector3 RandomPosition ()
		{
			//Declare an integer randomIndex, set it's value to a random number between 0 and the count of items in our List gridPositions.
			int randomIndex = Random.Range (0, gridPositions.Count);
			
			//Declare a variable of type Vector3 called randomPosition, set it's value to the entry at randomIndex from our List gridPositions.
			Vector3 randomPosition = gridPositions[randomIndex];
			
			//Remove the entry at randomIndex from the list so that it can't be re-used.
			gridPositions.RemoveAt (randomIndex);
			
			//Return the randomly selected Vector3 position.
			return randomPosition;
		}
		
		
		//LayoutObjectAtRandom accepts an array of game objects to choose from along with a minimum and maximum range for the number of objects to create.
		void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
		{
			//Choose a random number of objects to instantiate within the minimum and maximum limits
			int objectCount = Random.Range (minimum, maximum+1);
			
			//Instantiate objects until the randomly chosen limit objectCount is reached
			for(int i = 0; i < objectCount; i++)
			{
				//Choose a position for randomPosition by getting a random position from our list of available Vector3s stored in gridPosition
				Vector3 randomPosition = RandomPosition();
				
				//Choose a random tile from tileArray and assign it to tileChoice
				GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				
				//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
				Instantiate(tileChoice, randomPosition, Quaternion.identity);
			}
		}
		
		
		//SetupScene initializes our level and calls the previous functions to lay out the game board
		public void SetupScene (int level)
		{
		string levelString = "+TTTTTTTT* L11111111R L11411411R L11311111R L41114111R L11211141R L11141111R L11411111R L11144111R -BBBBBBBEx";
			//Creates the outer walls and floor.
			GenerateLevelFromString(levelString);
			
			//Reset our list of gridpositions.
			InitialiseList ();
			
			//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom (WallTiles, wallCount.minimum, wallCount.maximum);
			
			//Determine number of enemies based on current level number, based on a logarithmic progression
			int enemyCount = (int)Mathf.Log(level + 1, 2f);
			
			//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
			LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
		}

	}
	
