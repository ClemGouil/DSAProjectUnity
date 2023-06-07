using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

	
public class GameManager : MonoBehaviour {

	public float levelStartDelay= 2f;
	public float turnDelay =.3f;
	public BoardManager boardScript;
	public static GameManager instance= null;
	public int playerLifePoints = 100;
	[HideInInspector] public bool playersTurn = true;

	private Text levelText;
	private Text lifePointText;
	public Text enemyRemainingText;
	private GameObject levelimage;
	private bool doingSetup;

	private int level = 1;
	public List<Enemy> enemies;
	private bool enemiesMoving;



	void Awake ()
	{
		//Check if instance already exists
		if (instance == null)

			//if not, set instance to this
			instance = this;

		//If instance already exists and it's not this:
		else if (instance != this)

			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);	

		//Sets this to not be destroyed when reloading scene
		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemy> ();
	
		//Get a component reference to the attached BoardManager script
		boardScript = GetComponent<BoardManager>();

		//Call the InitGame function to initialize the first level 
		InitGame();

	}

	void InitGame ()
	{
		doingSetup = true;
		levelimage = GameObject.Find ("Image");
		levelText = GameObject.Find ("LevelText").GetComponentInParent<Text>();
		enemyRemainingText = GameObject.Find ("EnemyRemainingText").GetComponentInParent<Text>();
		levelText.text = "Level " + level;
		Invoke ("HideLevelImage", levelStartDelay);

		enemies.Clear ();
		boardScript.SetupScene (level);
	}

	private void OnLevelWasLoaded(int index){
		level++;
		InitGame ();
	}

	private void HideLevelImage(){
		levelimage.SetActive(false);
		doingSetup = false;
	}

	public void GameOver(){
		levelText.text = "GameOver";
		levelimage.SetActive (true);
		enabled = false;
	}

	void Update()
	{	
		enemyRemainingText.text = "Enemy Remaining : " + enemies.Count;
		if (playersTurn || enemiesMoving || doingSetup )
			return;
		
			StartCoroutine (MoveEnemies ());
	}

	public void AddEnemyToList(Enemy script){
		enemies.Add (script);
	}

	public void DeleteEnemyToList(Enemy script){
		enemies.Remove (script);
	}


	//Coroutine to move enemies in sequence.
	IEnumerator MoveEnemies()
	{
		//While enemiesMoving is true player is unable to move.
		enemiesMoving = true;

		//Wait for turnDelay seconds, defaults to .1 (100 ms).
		yield return new WaitForSeconds(turnDelay);

		//If there are no enemies spawned (IE in first level):
		if (enemies.Count == 0) 
		{
			//Wait for turnDelay seconds between moves, replaces delay caused by enemies moving when there are none.
			yield return new WaitForSeconds(turnDelay);
		}

		//Loop through List of Enemy objects.
		for (int i = 0; i < enemies.Count; i++)
		{
				//Call the MoveEnemy function of Enemy at index i in the enemies List.
			Enemy enemy = enemies[i];
			enemies [i].MoveEnemy ();
				

			//Wait for Enemy's moveTime before moving next Enemy, 
			yield return new WaitForSeconds (enemy .moveTime);
				
		}
		//Once Enemies are done moving, set playersTurn to true so player can move.
		playersTurn = true;

		//Enemies are done moving, set enemiesMoving to false.
		enemiesMoving = false;
	}


}

