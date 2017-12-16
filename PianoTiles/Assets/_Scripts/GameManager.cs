using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour {

	public GameObject tileObject;
	public List<GameObject> tiles = new List<GameObject>();
	public GameObject[] spawners;
	public GameObject rails;
	GameObject currentActiveTile;

	[HeaderAttribute("Game Adjustments")]
	float gameSpeed;
	int score = 0;
	const float MAX_TIME_LEFT = 15f;
	public int timeToAdd = 3;
	float timeLeft;
	int count = 0;
	bool startGame = false;

	[HeaderAttribute("UI Elements")]
	public Text scoreText;
	public Text timerText;
	public Text highScoreText;
	public Text yourScoreText;
	public GameObject panel;


	void Start () {
		float targetRatio = 9f/16f; //The aspect ratio of your game
		Camera cam = FindObjectOfType<Camera>().GetComponent<Camera>();
		cam.aspect = targetRatio;
		GameOver ();	//This inits the menu and such
		spawners = GameObject.FindGameObjectsWithTag ("Spawner");
		scoreText.text = score.ToString ();
		timerText.text = timeLeft.ToString ();
		highScoreText.text = "High score: " + PlayerPrefs.GetInt ("highscore").ToString();
		gameSpeed = Time.deltaTime;
	}

	public void GameOver(){
		startGame = false;
		scoreText.enabled = false;
		timerText.enabled = false;
		yourScoreText.text = "Your Score: " + score.ToString ();
		timeLeft = MAX_TIME_LEFT;
		CheckScore ();
		panel.SetActive (true);
		foreach(GameObject tile in tiles){
			GameObject.Destroy (tile);
		}
		CheckScore ();
		tiles.Clear ();
	}

	public void StartGame(){
		score = 0;
		scoreText.enabled = true;
		timerText.enabled = true;
		panel.SetActive (false);
		//Init the starting tiles
		GameObject tile;
		for (int i = 0; i < 5; i++) {
			int rnd = Random.Range (0,4);
			tile = Instantiate (tileObject, new Vector2 (spawners[rnd].transform.position.x, (i*tileObject.transform.localScale.y*2)+tileObject.transform.localScale.y), Quaternion.identity);
			tile.GetComponent<Tile> ().manager = this;
			tiles.Add (tile);
		}
		currentActiveTile = tiles[0];
	}
	
	// Update is called once per frame
	void Update () {

		GetValidTouch ();
		//Run Timer stuff
		if(startGame){
			RunTimer ();
			Mathf.MoveTowards(gameSpeed, 10f, .01f * Time.deltaTime);
		}

	}
	void IncrementScore(){
		score++;
		count++;
		scoreText.text = score.ToString ();
		Spawn ();
		AddTime ();
	}

	void AddTime(){
		if(count % 20 == 0 && count != 0){
			timeLeft += timeToAdd;
		}
	}

	void Spawn(){
		MoveAllDown ();
		int rnd = Random.Range (0,4);
		GameObject tile;
		tile = Instantiate (tileObject, new Vector2 (spawners[rnd].transform.position.x, spawners[rnd].transform.position.y), Quaternion.identity);
		tile.GetComponent<Tile> ().manager = this;
		tiles.Add (tile);
		currentActiveTile = tiles[tiles.IndexOf(currentActiveTile)+1];
	}

	void RunTimer(){
		if(timeLeft < 0f){
			GameOver ();
		}
		timeLeft -= gameSpeed;
		var timeLeftInt = (int)timeLeft;
		timerText.text = timeLeftInt.ToString ();
	}

	void MoveAllDown(){
		for (int i = 0; i < tiles.Count; i++) {

			if (tiles [i].transform.position.y <= 1.0f) {
				GameObject.Destroy (tiles [i]);
				tiles.RemoveAt (i);
			}

			float newPosition = tiles [i].transform.position.y - tiles [i].transform.localScale.y*2f;
			tiles [i].transform.position = new Vector2 (tiles[i].transform.position.x,newPosition);
		}
	}

	void GetValidTouch(){
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
			//We now raycast with this information. If we have hit something we can process it.
			RaycastHit2D hitInformation = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position), -Vector2.up);

			if (hitInformation.collider != null && hitInformation.transform.gameObject == currentActiveTile) {
				startGame = true;
				//We should have hit something with a 2D Physics collider!
				GameObject touchedObject = hitInformation.transform.gameObject;
				//touchedObject should be the object someone touched.
				IncrementScore();
			}
			else if(!startGame){
				return;
			}
			else{
				GameOver ();
			}
		}
	}

	void CheckScore(){
		if(score > PlayerPrefs.GetInt("highscore")){
			PlayerPrefs.SetInt ("highscore",score);
		}
		highScoreText.text = "High score: " + PlayerPrefs.GetInt ("highscore"); 
	}

	public void ClearHighScore(){
		PlayerPrefs.SetInt ("highscore", 0);
		highScoreText.text = "High score: " + PlayerPrefs.GetInt ("highscore").ToString();
	}
}
