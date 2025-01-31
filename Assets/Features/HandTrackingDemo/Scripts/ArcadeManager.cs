using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using System;

public class ArcadeManager: MonoBehaviour
{
    private static ArcadeManager _instance;
    public static ArcadeManager instance
    {
        get => _instance;
        private set
        {
            if (_instance != null)
                Debug.LogWarning("Second attempt to get ArcadeManager");
            _instance = value;
        }
    }

    public enum GameState
    {
        None,
        Waiting,
        Active,
        End,
    }

    public GameState currGameState = GameState.None;
    [Header("Coin Components")]
    public Transform grabCoinTransform;
    public GameObject coinPrefab;
    public string CurrCoinValStr { get; set; }
    public int CurrCoinVal { get; set; }
    public bool CoinGrabbed { get; set; }
    private GameObject currCoin = null;

    [Header("Text Components")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI topScoreText;
    public TextMeshProUGUI timeText;

    [Header("Game Area Components")]
    public GameObject arcadePlayArea;
    public GameObject tutorialButton;
    public GameObject arcadeButton;
    public GameObject zoneParent;
    public Transform[] zoneList;

    [Header("Target Components")]
    public GameObject targetArea;
    public Transform target1;
    public Transform target2;
    public Transform target3;
    public Animator t1Anim;
    public Animator t2Anim;
    public Animator t3Anim;
    public BoxCollider target1Col;
    public BoxCollider target2Col;
    public BoxCollider target3Col;
    public TargetPosCtrl targetPosCtrl;

    private TargetPosition[] targetPositions = null;
    private TargetPosition startingPosition = null;
    private float targetSwitchDelay = .5f;

    [Header("Game Logic Components")]
    public float maxTime = 60f;
    private List<GameObject> hatList = new List<GameObject>();
    public GameObject[] scoreHatPrefabs;

    private GameObject prevHat;
    private bool last3Seconds = false;
    private float currTime = 0;
    private int currScore = 0;
    private int topScore = 0;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        IntroSetup();
    }

    // Update is called once per frame
    void Update()
    {
        TimerUpdate();
    }

    public bool ArcadeReady() => currGameState == GameState.Waiting;

    //Arcade Logic methods area
    private void IntroSetup()
    {
        ResetScore();
        zoneList = zoneParent.GetComponentsInChildren<Transform>();
        currGameState = GameState.Waiting;
        arcadePlayArea.SetActive(false);
        CoinGrabbed = false;
        Random.InitState((int)System.DateTime.Now.Ticks); //To Randomize the random num gen
    }

    public void ResetHats()
    {
        foreach (GameObject box in hatList)
            Destroy(box);
        hatList.Clear();
    }

    public void ResetScore()
    {
        currScore = 0;
        scoreText.text = currScore.ToString();
    }

    public void ResetTargets()
    {
        //Arcade play area needs to be active for it to grab the positions correctly
        //Call this method in GameStart after turning on arcade play area
        if(targetPositions == null)
        {
            targetPositions = targetPosCtrl.targetPositions;
            startingPosition = targetPositions[0];
        }

        target1.position = startingPosition.target1Pos;
        target2.position = startingPosition.target2Pos;
        target3.position = startingPosition.target3Pos;
        target1Col.enabled = true;
        target2Col.enabled = true;
        target3Col.enabled = true;
    }

    //Called from button to start new game
    public void StartNewGame()
    {
        arcadeButton.SetActive(false);
        tutorialButton.SetActive(false);
        last3Seconds = false;
        currTime = maxTime;
        ResetScore();
        ResetHats();
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        SoundManager.instance.PlayReady();
        arcadePlayArea.SetActive(true);
        targetArea.SetActive(true);
        ResetTargets();

        yield return new WaitForSeconds(1.5f);

        SoundManager.instance.PlayGo();
        yield return new WaitForSeconds(.2f);

        currGameState = GameState.Active;
        AddScoreHat();
    }

    //Adds a new hat to the table
    public void AddScoreHat()
    {
        int prefabNum = Random.Range(0, scoreHatPrefabs.Length);
        int zoneNum = Random.Range(1, zoneList.Length);//zoneList[0] reserved for prev zone
        Transform zonePos = zoneList[zoneNum];
        ZoneSwap(zoneNum);

        Quaternion hatRotation = scoreHatPrefabs[prefabNum].transform.localRotation;
        GameObject newHat = Instantiate(scoreHatPrefabs[prefabNum], zonePos.position, hatRotation);
        hatList.Add(newHat);
        prevHat = newHat;

        SoundManager.instance.PlaySpawn();
    }

    private void ZoneSwap(int idx)
    {
        //swap zone so not to repeat zone
        Transform prevZone = zoneList[0];
        zoneList[0] = zoneList[idx];
        zoneList[idx] = prevZone;
    }

    //Called from hat when correct coin to hat is matched
    public void UpdateScore()
    {
        currScore += 1;
        scoreText.text = currScore.ToString();
        SoundManager.instance.PlayCoin();
        //hat has its own self destruct sequence
        if(prevHat != null)
            hatList.Remove(prevHat);
        prevHat = null;
        //adds a new hat
        AddScoreHat();
    }

    //timer for the game
    private void TimerUpdate()
    {
        if (currGameState == GameState.Active)
        {
            currTime -= 1 * Time.deltaTime;
            timeText.text = currTime.ToString("0");

            //starts the voice countdown
            if(currTime <= 3f && !last3Seconds)
            {
                last3Seconds = true;
                StartCoroutine(StartCountDown());
            }
            //ends the game
            if (currTime <= 0)
                StartCoroutine(EndSequence());
        }
    }
    //voice countdown
    IEnumerator StartCountDown()
    {
        SoundManager.instance.Play3();
        yield return new WaitForSeconds(1f);

        SoundManager.instance.Play2();
        yield return new WaitForSeconds(1f);

        SoundManager.instance.Play1();
    }
    //end sequence
    IEnumerator EndSequence()
    {
        currGameState = GameState.End;
        SoundManager.instance.PlayTimeover();
        ResetHats();
        DestroyCoin();
        targetArea.SetActive(false);
        yield return new WaitForSeconds(3f);

        CheckTopScore();
        yield return new WaitForSeconds(3f);

        SoundManager.instance.PlaySuccess();
        GameOver();
    }
    //if a new high score, update and play the new high score
    //otherwise you get just game over
    private void CheckTopScore()
    {
        if (currScore > topScore)
        {
            topScoreText.text = currScore.ToString();
            topScore = currScore;
            SoundManager.instance.PlayHighScore();
        }
        else
            SoundManager.instance.PlayGameOver();
    }
    //give player 3 seconds to see their score then reset back to starting area
    public void GameOver()
    {
        currTime = 0;
        timeText.text = "0";
        currScore = 0;
        scoreText.text = "0";

        arcadePlayArea.SetActive(false);
        currGameState = GameState.Waiting;

        ResetHats();
        DestroyCoin();

        arcadeButton.SetActive(true);
        tutorialButton.SetActive(true);
    }

    //Coin methods area
    public void DestroyCoin()
    {
        if (currCoin)
            Destroy(currCoin);
        currCoin = null;
        CoinGrabbed = false;
    }

    //Everytime we hit a target, we create a new coin

    public void MakeNewCoin()
    {
        DestroyCoin();

        currCoin = GameObject.Instantiate(coinPrefab, grabCoinTransform.position, grabCoinTransform.rotation);
        GrabCoinCtrl grabcc = currCoin.GetComponent<GrabCoinCtrl>();
        grabcc.SetUpCoin(CurrCoinVal, CurrCoinValStr);

    }
    //sets up value so the GrabCoinCtrl can set the correct value on coin
    public void MakeCoinOne()
    {
        CurrCoinVal = 1;
        CurrCoinValStr = "1";
        MakeNewCoin();
    }

    public void MakeCoinTwo()
    {
        CurrCoinVal = 2;
        CurrCoinValStr = "2";
        MakeNewCoin();
    }

    public void MakeCoinThree()
    {
        CurrCoinVal = 3;
        CurrCoinValStr = "3";
        MakeNewCoin();
    }



    //Change positions of target 
    public void StartTargetSwitch()
    {
        if (currGameState == GameState.Active)
            StartCoroutine(SwitchTargetPos());
    }

    private IEnumerator SwitchTargetPos()
    {
        target1Col.enabled = false;
        target2Col.enabled = false;
        target3Col.enabled = false;

        t1Anim.SetTrigger("Close");
        t2Anim.SetTrigger("Close");
        t3Anim.SetTrigger("Close");
        yield return new WaitForSeconds(targetSwitchDelay);

        int newPosNum = Random.Range(1, targetPositions.Length);
        target1.position = targetPositions[newPosNum].target1Pos;
        target2.position = targetPositions[newPosNum].target2Pos;
        target3.position = targetPositions[newPosNum].target3Pos;
        TargetPosSwap(newPosNum);

        t1Anim.SetTrigger("Open");
        t2Anim.SetTrigger("Open");
        t3Anim.SetTrigger("Open");
        yield return new WaitForSeconds(targetSwitchDelay);

        target1Col.enabled = true;
        target2Col.enabled = true;
        target3Col.enabled = true;
    }

    private void TargetPosSwap(int idx)
    {
        //swap pos so not to repeat pos
        var prevTargetPos = targetPositions[0];
        targetPositions[0] = targetPositions[idx];
        targetPositions[idx] = prevTargetPos;
    }
}
