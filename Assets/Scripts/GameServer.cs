using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameServer : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button finishGameButton;

    [SerializeField] private GameObject myCamera;
    [SerializeField] private Players myPlayer;

    [SerializeField] private Camera uiCam;
    [SerializeField] private GameObject course;
    [SerializeField] private GameObject courseBGM;

    public float currentTime = 240;
    private int lives = 3;
    private int myScore = 0;
    private int currentLevel = 1;
    private int[] allMyScores = new int[9];
    private int myTotalScore = 0;
    private string myName = "Name";

    private int[,] allComputerScores = new int[6, 9];

    private int[] levelPars = { 3, 3, 2, 3, 5, 10, 16, 20, 25 };
    private int[] computerScores = { 0, 0, 0, 0, 0, 0 };
    private int[] totalComputerScores = { 0, 0, 0, 0, 0, 0 };
    private string[] computerNames = { "Omar", "Manakeesh", "Saif", "Cheese", "Steak", "Dog" };

    private float nextMethodCount = 1.5f;
    private float timeCounter = 0f;
    private int secondCounter = 0;

    private bool inIntroScreen = false;
    private bool inLoadingScreen = false;
    private bool inMapScreen = false;
    private bool inMoveCamera = false;
    private bool inGameCountdown = false;
    private bool inGameInProgress = false;
    private bool inRoundFinish = false;
    private bool inShowScoreboard = false;
    private bool inPostRound = false;
    private bool inGameEnd = false;
    private bool inFinalScoreboard = false;
    private bool inRestartGame = false;
    private bool inGameOver = false;

    private bool startingGame = false;
    private bool escapeSettings = false;

    private float movementTimer = 1f;

    // Start is called before the first frame update
    void Start()
    {
        inIntroScreen = true;
        myScore = 0;
        currentLevel = 1;
        homeScreenUI.SetActive(false);
        pickingNameUI.SetActive(true);
        leaderboardsUI.SetActive(false);
        HUD.SetActive(false);
        myPlayer.gameObject.transform.parent.gameObject.SetActive(false);

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                allComputerScores[i, j] = 0;
            }
        }
    }

    private void ResetValues()
    {
        currentTime = 240;
        lives = 3;
        myScore = 0;
        currentLevel = 1;
        allMyScores = new int[9];
        myTotalScore = 0;
        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                allComputerScores[i, j] = 0;
            }
        }

        for (int i = 0; i < 6; i++)
        {
            computerScores[i] = 0;
            totalComputerScores[i] = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inIntroScreen == true)
        {
            IntroScreen();
        }
        else if (inLoadingScreen == true)
        {
            LoadingScreen();
        }

        else if (inMapScreen == true)
        {
            MapScreen();
        }

        else if (inMoveCamera == true)
        {
            MoveCamera();
        }

        else if (inGameCountdown == true)
        {
            GameCountdown();
        }

        else if (inGameInProgress == true)
        {
            GameInProgress();
        }

        else if (inRoundFinish == true)
        {
            RoundFinish();
        }

        else if (inShowScoreboard == true)
        {
            ShowScoreboard();
        }

        else if (inPostRound == true)
        {
            PostRound();
        }

        else if (gettingReward == true)
        {
            GetReward();
        }

        else if (inGameEnd == true)
        {
            GameEnd();
        }

        else if (inFinalScoreboard == true)
        {
            FinalScoreboard();
        }

        else if (inRestartGame == true)
        {
            RestartGame();
        }

        else if (inGameOver == true)
        {
            GameOver();
        }

        if (inGameInProgress == false || escapeSettings == true)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            myPlayer.lockCursor = false;
            myPlayer.lockedCursor = false;
            myPlayer.forceUnlock = true;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            myPlayer.lockCursor = true;
            myPlayer.lockedCursor = true;
            myPlayer.forceUnlock = false;
        }

        if (inGameInProgress == false && escapeSettings == false)
        {
            timeCounter = timeCounter + Time.deltaTime;
        }
    }

    [SerializeField] private GameObject loadingScreenUI;
    [SerializeField] private GameObject mapScreenUI;
    [SerializeField] private GameObject countdownUI;

    private void IntroScreen()
    {
        myPlayer.notMovable = true;
        inIntroScreen = true;
        uiCam.clearFlags = CameraClearFlags.SolidColor;
        uiCam.cullingMask = LayerMask.GetMask("Default", "UI");
        uiCam.orthographic = false;

        course.transform.Rotate(0, 10 * Time.deltaTime, 0);

        Vector3 pos = course.transform.position;
        float newY = Mathf.Sin(Time.time * 0.5f) * 0.02f;
        course.transform.position = new Vector3(pos.x, pos.y+newY, pos.z);

        if (nameInput.text != "" && nameInput.text != null)
        {
            pickingNameUI.gameObject.transform.Find("Button").gameObject.GetComponent<Image>().color = Color.green;
        }
        else
        {
            pickingNameUI.gameObject.transform.Find("Button").gameObject.GetComponent<Image>().color = Color.black;
        }

        if (startingGame == true)
        {
            ResetValues();
            inIntroScreen = false;
            startingGame = false;
            timeCounter = 0f;
            LoadingScreen();
        }
    }

    private void LoadingScreen()
    {
        inLoadingScreen = true;
        loadingScreenUI.SetActive(true);
        uiCam.cullingMask = LayerMask.GetMask("UI");
        uiCam.orthographic = true;
        uiCam.clearFlags = CameraClearFlags.Depth;

        if (timeCounter >= nextMethodCount)
        {
            loadingScreenUI.SetActive(false);
            inLoadingScreen = false;
            timeCounter = 0f;
            MapScreen();
        }
    }

    [SerializeField] private GameObject map;

    private void MapScreen()
    {
        if (inMapScreen == false)
        {
            map.gameObject.transform.Find("Course" + currentLevel.ToString()).gameObject.SetActive(true);
            myPlayer.sphere.gameObject.transform.position = map.gameObject.transform.Find("Course" + currentLevel.ToString()).gameObject.transform.Find("Spawnpoint").gameObject.transform.position;
            //Debug.Log(myPlayer.sphere.gameObject.transform.position);
            //Debug.Log(map.gameObject.transform.Find("Course" + currentLevel.ToString()).gameObject.transform.Find("Spawnpoint").gameObject.transform.position);
        }

        inMapScreen = true;
        
        courseBGM.SetActive(true);
        mapScreenUI.SetActive(true);
        mapScreenUI.gameObject.transform.Find("LevelNumber").gameObject.GetComponent<TextMeshProUGUI>().text = "Course " + currentLevel.ToString();

        if (timeCounter >= nextMethodCount)
        {
            //myPlayer.sphere.gameObject.transform.position = map.gameObject.transform.Find("Course" + currentLevel.ToString()).gameObject.transform.Find("Spawnpoint").gameObject.transform.position;
            mapScreenUI.SetActive(false);
            inMapScreen = false;
            timeCounter = 0f;
            MoveCamera();
        }
    }

    [SerializeField] private GameObject mapCamera;

    private void MoveCamera()
    {
        if (inMoveCamera == false)
        {

        }

        inMoveCamera = true;

        SetCourse();

        if (timeCounter >= nextMethodCount - 1f)
        {
            inMoveCamera = false;
            currentTime = 240;
            SetLives();
            SettingTime();
            SetPar();
            SetScore();
            HUD.SetActive(true);
            myPlayer.gameObject.transform.parent.gameObject.SetActive(true);
            timeCounter = 0f;
            GameCountdown();
        }
    }

    private float countdownTime = 3f;

    private void GameCountdown()
    {
        if (inGameCountdown == false)
        {
            //myCamera.gameObject.SetActive(true);
            countdownTime = 3f;
            countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().text = "3";
            countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        }

        inGameCountdown = true;
        countdownUI.SetActive(true);

        countdownTime = countdownTime - Time.deltaTime * .769f;

        if (countdownTime <= 2 && countdownTime > 1)
        {
            countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().text = "2";
        }
        else if (countdownTime <= 1 && countdownTime > 0)
        {
            countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().text = "1";
        }
        else if (countdownTime <= 0)
        {
            countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().text = "GO";
            countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().color = Color.green;
        }

        if (countdownTime <= -1)
        {
            countdownUI.SetActive(false);
            inGameCountdown = false;
            myPlayer.notMovable = false;
            timeCounter = 0f;

            GameInProgress();

        }
    }

    private void GameInProgress()
    {
        inGameInProgress = true;

        timeCounter = timeCounter + Time.deltaTime * .769f;

        SwitchMode();
        SetScore();

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Escaping();
        }

        if (timeCounter >= secondCounter)
        {
            currentTime = currentTime - 1;
            SettingTime();
            secondCounter = secondCounter + 1;
        }

        if (movementTimer >= .3f)
        {
            myPlayer.notMovable = false;
        }
        else
        {
            myPlayer.notMovable = true;
        }

        if (escapeSettings == false)
        {
            movementTimer = movementTimer + Time.deltaTime;
        }

        if (currentTime <= 0 || myPlayer.completedLevel == true)
        {
            if (escapeSettings == true)
            {
                Escaping();
            }

            movementTimer = 1f;

            if (myPlayer.completedLevel == false)
            {
                myPlayer.attempts = 10;
            }
            myPlayer.completedLevel = false;
            myScore = myPlayer.attempts;
            myTotalScore = myTotalScore + myScore;
            myPlayer.attempts = 0;
            myPlayer.gameObject.transform.parent.gameObject.SetActive(false);
            HUD.SetActive(false);
            inGameInProgress = false;
            timeCounter = 0f;
            myPlayer.notMovable = true;
            RoundFinish();
        }
    }

    private bool gameFinished = false;

    private void RoundFinish()
    {
        if (inRoundFinish == false)
        {
            gameFinished = false;
            countdownUI.SetActive(true);
            if (myScore <= levelPars[currentLevel - 1])
            {
                countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().text = "Round Complete";
                countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().color = Color.green;
            }
            else
            {
                if (lives != 1)
                {
                    countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().text = "Redo Round";
                    countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().color = Color.red;
                }
                else
                {
                    countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().text = "Game Lost";
                    countdownUI.gameObject.transform.Find("CountdownText").gameObject.GetComponent<TextMeshProUGUI>().color = Color.red;
                    gameFinished = true;
                }
            }
        }

        inRoundFinish = true;

        if (timeCounter >= nextMethodCount)
        {
            if (gameFinished == true)
            {
                currentTime = 0f;
                secondCounter = 0;
                inRoundFinish = false;
                timeCounter = 0f;
                GameEnd();
            }
            else
            {
                countdownUI.SetActive(false);
                currentTime = 0f;
                secondCounter = 0;
                inRoundFinish = false;
                timeCounter = 0f;
                Array.Fill(scoreOrder, 0);
                allMyScores[currentLevel - 1] = myScore;
                map.gameObject.transform.Find("Course" + currentLevel.ToString()).gameObject.SetActive(false);

                SetAIScores();
                ShowScoreboard();
            }
        }
    }

    [SerializeField] private GameObject ScoreboardUI;

    private int[] scoreOrder = { 0, 0, 0, 0, 0, 0, 0 };
    private int[] scoreOrder2 = { 0, 0, 0, 0, 0, 0, 0 };

    private int CalculateScores(int[] tempScore)
    {
        int currentHighest = 0;
        int highestValue = 99;
        for (int i = 0; i < tempScore.Length; i++)
        {
            if (totalComputerScores[i] < highestValue && tempScore[i] != 99)
            {
                currentHighest = i;
                highestValue = totalComputerScores[i];
            }
        }

        return currentHighest;
    }

    private int[] tempScores = { 0, 0, 0, 0, 0, 0 };
    private void FindScoreOrder()
    {
        int mySpot = 0;
        for (int i = 0; i < computerScores.Length; i++)
        {
            if (myTotalScore < totalComputerScores[i])
            {
                mySpot = mySpot + 1;
            }
        }
        scoreOrder[mySpot] = 10;

        for (int i = 0; i < computerScores.Length; i++)
        {
            tempScores[i] = totalComputerScores[i];
        }
        int highestIndex;
        int computerSpot = 6;

        for (int i = 0; i < tempScores.Length; i++)
        {
            highestIndex = CalculateScores(tempScores);
            tempScores[highestIndex] = 99;
            if (scoreOrder[computerSpot] == 10)
            {
                computerSpot = computerSpot - 1;
            }
            scoreOrder[computerSpot] = highestIndex;
            computerSpot = computerSpot - 1;
        }
    }

    private void SetScoreboard()
    {
        for (int i = 0; i < scoreOrder.Length; i++)
        {
            scoreOrder2[scoreOrder2.Length - i - 1] = scoreOrder[i];
        }

        string holeName;
        string theName;
        int totalScore;
        int levelScore;

        for (int i = 0; i < scoreOrder2.Length; i++)
        {
            if (scoreOrder2[i] == 10)
            {
                totalScore = 0;
                for (int j = 0; j < 9; j++)
                {
                    levelScore = allMyScores[j];
                    holeName = "Hole" + (j + 1).ToString();
                    if (j + 1 == currentLevel)
                    {
                        ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find(holeName).gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
                    }
                    else
                    {
                        ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find(holeName).gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
                    }
                    ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find(holeName).gameObject.GetComponent<TextMeshProUGUI>().text = levelScore.ToString();
                }

                theName = myName;
                for (int j = 0; j < 9; j++)
                {
                    totalScore = totalScore + allMyScores[j];
                }

                ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find("Total").gameObject.GetComponent<TextMeshProUGUI>().text = totalScore.ToString();
                ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = theName;

                ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find("Total").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
                ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
                continue;
            }
            totalScore = 0;
            for (int j = 0; j < 9; j++)
            {
                levelScore = allComputerScores[scoreOrder2[i], j];
                holeName = "Hole" + (j + 1).ToString();
                ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find(holeName).gameObject.GetComponent<TextMeshProUGUI>().text = levelScore.ToString();
                ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find(holeName).gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
            }

            theName = computerNames[scoreOrder2[i]];
            for (int j = 0; j < 9; j++)
            {
                totalScore = totalScore + allComputerScores[scoreOrder2[i], j];
            }

            ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find("Total").gameObject.GetComponent<TextMeshProUGUI>().text = totalScore.ToString();
            ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = theName;
            ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find("Total").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
            ScoreboardUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        }
    }

    private void ShowScoreboard()
    {
        if (inShowScoreboard == false)
        {
            SetScoreboard();
            ScoreboardUI.SetActive(true);
        }

        inShowScoreboard = true;

        if (timeCounter >= nextMethodCount)
        {
            ScoreboardUI.SetActive(false);
            inShowScoreboard = false;
            timeCounter = 0f;
            PostRound();
        }
    }

    private void PostRound()
    {

        inPostRound = true;

        if (timeCounter >= nextMethodCount)
        {
            inPostRound = false;
            timeCounter = 0f;
            unlockedItems[currentLevel] = itemList[currentLevel];

            if (currentLevel >= 9)
            {
                GameEnd();
            }
            else
            {
                if (allMyScores[currentLevel - 1] > levelPars[currentLevel - 1])
                {
                    LoseLife();
                    if (lives == 0)
                    {
                        GameEnd();
                    }
                    else
                    {
                        LoadingScreen();
                    }
                }
                else
                {
                    currentLevel = currentLevel + 1;
                    GainLife();
                    GetReward();
                }
            }
        }
        //If on the last round, end the game, otherwise go back to Loading Screen
    }

    private bool nextLevel = false;
    private bool exitGame = false;
    private bool gettingReward = false;
    [SerializeField] private GameObject rewardUI;

    public void EquipItem(TMP_Text itemText)
    {
        EquipItem(currentLevel - 1);
        itemText.text = "Equipped";
    }

    public void NextLevelButton(TMP_Text itemText)
    {
        nextLevel = true;
        itemText.text = "Equip";
    }

    public void ExitGameButton(TMP_Text itemText)
    {
        exitGame = true;
        itemText.text = "Equip";
    }

    private void CustomizeRewardItem(string itemName)
    {
        for (int i = 0; i < rewardUI.gameObject.transform.Find("Hats").gameObject.transform.childCount; i++)
        {
            rewardUI.gameObject.transform.Find("Hats").gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        rewardUI.gameObject.transform.Find("Hats").gameObject.transform.Find(itemName).gameObject.SetActive(true);

    }

    private void GetReward()
    {
        if (gettingReward == false)
        {
            rewardUI.SetActive(true);
            rewardUI.gameObject.transform.Find("itemName").gameObject.GetComponent<TextMeshProUGUI>().text = unlockedItems[currentLevel - 1];
            CustomizeRewardItem(unlockedItems[currentLevel - 1]);
        }

        gettingReward = true;

        if (nextLevel == true)
        {
            nextLevel = false;
            gettingReward = false;
            rewardUI.SetActive(false);
            LoadingScreen();
        }
        else if (exitGame == true)
        {
            exitGame = false;
            gettingReward = false;
            rewardUI.SetActive(false);
            GameEnd();
        }
    }

    private void GameEnd()
    {
        inGameEnd = true;

        if (timeCounter >= nextMethodCount)
        {
            inGameEnd = false;
            timeCounter = 0f;
            FinalScoreboard();
        }
    }

    private void FinalScoreboard()
    {
        inFinalScoreboard = true;

        if (timeCounter >= nextMethodCount)
        {
            inFinalScoreboard = false;
            timeCounter = 0f;
            GameOver();
            //If the restart button is pressed, go to RestartGame(), if the Exit button is pressed, go back to Intro Screen()
        }
    }

    //Probably won't use this
    private void RestartGame()
    {
        inRestartGame = true;

        if (timeCounter >= nextMethodCount)
        {
            inRestartGame = false;
            timeCounter = 0f;
            //If the restart button is pressed, go to RestartGame(), if the Exit button is pressed, go back to Intro Screen()
        }
        //Reset all values, go back to home screen
    }

    private void GameOver()
    {
        inGameOver = true;

        if (timeCounter >= nextMethodCount)
        {
            UpdateLeaderboards();
            OpenLeaderboards();
            IntroScreen();
            ResetValues();
            inGameOver = false;
            timeCounter = 0f;
            //If the restart button is pressed, go to RestartGame(), if the Exit button is pressed, go back to Intro Screen()
        }
    }

    //IN THE GAME
    private void LoseLife()
    {
        lives = lives - 1;
    }

    private void GainLife()
    {
        if (lives < 3)
        {
            lives = lives + 1;
        }
    }

    private void SetAIScores()
    {
        var holeInOneAI = UnityEngine.Random.Range(0, 7);
        for (int i = 0; i < computerScores.Length; i++)
        {
            if (holeInOneAI == i)
            {
                computerScores[i] = 1;
                allComputerScores[i, currentLevel - 1] = 1;
            }
            else
            {
                computerScores[i] = UnityEngine.Random.Range(1, 13);
                allComputerScores[i, currentLevel - 1] = computerScores[i];
            }
            totalComputerScores[i] = totalComputerScores[i] + computerScores[i];
        }
        FindScoreOrder();
    }


    [SerializeField] private GameObject homeScreenUI;
    [SerializeField] private GameObject pickingNameUI;
    [SerializeField] private GameObject leaderboardsUI;
    [SerializeField] private GameObject HUD;

    [SerializeField] private TMP_InputField nameInput;

    public void ChooseName()
    {

        if (nameInput.text != "" && nameInput.text != null)
        {
            myName = nameInput.text;
            for (int i = 0; i < computerNames.Length; i++)
            {
                if (myName == computerNames[i])
                {
                    computerNames[i] = computerNames[i] + "1";
                }
            }
            SetupLeaderboards();
            homeScreenUI.SetActive(true);
            leaderboardsUI.SetActive(false);
            pickingNameUI.SetActive(false);
            HUD.SetActive(false);

        }
    }

    public void StartGameUI()
    {
        homeScreenUI.SetActive(false);
        leaderboardsUI.SetActive(false);
        HUD.SetActive(false);
        startingGame = true;
    }

    public void OpenLeaderboards()
    {
        homeScreenUI.SetActive(false);
        leaderboardsUI.SetActive(true);
        HUD.SetActive(false);
    }

    public void CloseLeaderboards()
    {
        homeScreenUI.SetActive(true);
        leaderboardsUI.SetActive(false);
        HUD.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    [SerializeField] private TMP_Text modeText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text timeText2;
    [SerializeField] private TMP_Text courseText;

    public void SwitchMode()
    {
        modeText.text = myPlayer.shootMode;
    }

    private void SettingTime()
    {
        float tempTime = currentTime;
        int numberOfMinutes = 0;
        while (tempTime >= 60)
        {
            tempTime = tempTime - 60;
            numberOfMinutes++;
        }

        if (tempTime < 10)
        {
            timeText.text = numberOfMinutes.ToString() + ":0" + tempTime.ToString();
            timeText2.text = numberOfMinutes.ToString() + ":0" + tempTime.ToString();
        }
        else
        {
            timeText.text = numberOfMinutes.ToString() + ":" + tempTime.ToString();
            timeText2.text = numberOfMinutes.ToString() + ":" + tempTime.ToString();
        }
    }

    private void SetCourse()
    {
        courseText.text = "Course " + currentLevel.ToString();
    }

    [SerializeField] private GameObject playerScores;

    private void SetPar()
    {
        playerScores.transform.Find("LevelPar").gameObject.GetComponent<TextMeshProUGUI>().text = "Par " + levelPars[currentLevel - 1].ToString();
    }

    private void SetScore()
    {
        myScore = myPlayer.attempts;
        playerScores.transform.Find("myScore").gameObject.GetComponent<TextMeshProUGUI>().text = myScore.ToString();
        if (myScore > levelPars[currentLevel - 1])
        {
            playerScores.transform.Find("myScore").gameObject.GetComponent<TextMeshProUGUI>().color = Color.red;
        }
        else
        {
            playerScores.transform.Find("myScore").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        }
    }


    [SerializeField] private GameObject playerLives;

    private void SetLives()
    {
        if (lives == 3)
        {
            playerLives.gameObject.transform.Find("Life1").gameObject.SetActive(true);
            playerLives.gameObject.transform.Find("Life2").gameObject.SetActive(true);
            playerLives.gameObject.transform.Find("Life3").gameObject.SetActive(true);
        }
        else if (lives == 2)
        {
            playerLives.gameObject.transform.Find("Life1").gameObject.SetActive(true);
            playerLives.gameObject.transform.Find("Life2").gameObject.SetActive(true);
            playerLives.gameObject.transform.Find("Life3").gameObject.SetActive(false);
        }
        else if (lives == 1)
        {
            playerLives.gameObject.transform.Find("Life1").gameObject.SetActive(true);
            playerLives.gameObject.transform.Find("Life2").gameObject.SetActive(false);
            playerLives.gameObject.transform.Find("Life3").gameObject.SetActive(false);
        }
        else if (lives == 0)
        {
            playerLives.gameObject.transform.Find("Life1").gameObject.SetActive(false);
            playerLives.gameObject.transform.Find("Life2").gameObject.SetActive(false);
            playerLives.gameObject.transform.Find("Life3").gameObject.SetActive(false);
        }
    }


    [SerializeField] private GameObject score1;
    [SerializeField] private GameObject score2;
    [SerializeField] private GameObject score3;
    [SerializeField] private GameObject score4;
    [SerializeField] private GameObject score5;
    [SerializeField] private GameObject scoreMine;

    private string[] top5Names = { "NA", "NA", "NA", "NA", "NA" };
    private string[] top5Scores = { "NA", "NA", "NA", "NA", "NA" };

    private void SetupLeaderboards()
    {
        int eachScore = 40;
        for (int i = 0; i < top5Names.Length; i++)
        {
            top5Names[i] = computerNames[i];
            top5Scores[i] = eachScore.ToString();
            eachScore = eachScore - UnityEngine.Random.Range(5, 10);
        }

        score1.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = top5Names[4];
        score2.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = top5Names[3];
        score3.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = top5Names[2];
        score4.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = top5Names[1];
        score5.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = top5Names[0];

        score1.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = top5Scores[4];
        score2.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = top5Scores[3];
        score3.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = top5Scores[2];
        score4.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = top5Scores[1];
        score5.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = top5Scores[0];
    }

    private void UpdateLeaderboards()
    {
        scoreMine.SetActive(true);
        int scoreIndex = -1;
        string myRank = "NA";

        if (lives > 0)
        {
            scoreMine.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = myName;
            scoreMine.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = myTotalScore.ToString();

            for (int i = 0; i < top5Names.Length; i++)
            {
                if (top5Names[i] != "NA")
                {
                    if (myTotalScore <= int.Parse(top5Scores[i]))
                    {
                        scoreIndex = scoreIndex + 1;
                    }
                }
                else
                {
                    scoreIndex = scoreIndex + 1;
                }
            }

            int eachItem = 0;
            if (scoreIndex >= 0)
            {
                while (eachItem < scoreIndex)
                {
                    top5Names[eachItem] = top5Names[eachItem + 1];
                    top5Scores[eachItem] = top5Scores[eachItem + 1];
                    eachItem = eachItem + 1;
                }
                top5Names[scoreIndex] = myName;
                top5Scores[scoreIndex] = myTotalScore.ToString();
            }

            if (scoreIndex == 0)
            {
                myRank = "5";
            }
            else if (scoreIndex == 1)
            {
                myRank = "4";
            }
            else if (scoreIndex == 2)
            {
                myRank = "3";
            }
            else if (scoreIndex == 3)
            {
                myRank = "2";
            }
            else if (scoreIndex == 4)
            {
                myRank = "1";
            }
        }
        else
        {
            myRank = "NA";
            scoreMine.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = myName;
            scoreMine.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = "NA";
        }

        scoreMine.gameObject.transform.Find("rank").gameObject.GetComponent<TextMeshProUGUI>().text = myRank;

        score1.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = top5Names[4];
        score2.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = top5Names[3];
        score3.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = top5Names[2];
        score4.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = top5Names[1];
        score5.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = top5Names[0];

        score1.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = top5Scores[4];
        score2.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = top5Scores[3];
        score3.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = top5Scores[2];
        score4.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = top5Scores[1];
        score5.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().text = top5Scores[0];

        if (score1.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text == myName)
        {
            score1.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            score1.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
        else
        {
            score1.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
            score1.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        }

        if (score2.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text == myName)
        {
            score2.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            score2.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
        else
        {
            score2.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
            score2.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        }

        if (score3.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text == myName)
        {
            score3.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            score3.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
        else
        {
            score3.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
            score3.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        }

        if (score4.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text == myName)
        {
            score4.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            score4.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
        else
        {
            score4.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
            score4.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        }

        if (score5.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text == myName)
        {
            score5.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            score5.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
        else
        {
            score5.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
            score5.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        }

        if (scoreMine.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text == myName)
        {
            scoreMine.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
            scoreMine.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
        else
        {
            scoreMine.gameObject.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
            scoreMine.gameObject.transform.Find("Score").gameObject.GetComponent<TextMeshProUGUI>().color = Color.white;
        }

    }

    [SerializeField] private GameObject escapeUI;
    [SerializeField] private GameObject buttonsUI;
    private void Escaping()
    {
        myPlayer.cancelDash = true;
        movementTimer = 0f;
        if (escapeSettings == false)
        {
            escapeSettings = true;
            myPlayer.notMovable = true;
            customizeUI.SetActive(false);
            escapeUI.SetActive(true);
            buttonsUI.SetActive(true);
        }
        else if (escapeSettings == true)
        {
            if (inGameInProgress == false)
            {
                myPlayer.notMovable = true;
            }
            escapeSettings = false;
            escapeUI.SetActive(false);
            buttonsUI.SetActive(false);
        }
    }

    public void ExitButton()
    {
        inIntroScreen = false;
        inLoadingScreen = false;
        inMapScreen = false;
        inMoveCamera = false;
        inGameCountdown = false;
        inGameInProgress = false;
        inRoundFinish = false;
        inShowScoreboard = false;
        inPostRound = false;
        inGameEnd = false;
        inFinalScoreboard = false;
        inRestartGame = false;
        inGameOver = false;

        map.gameObject.transform.Find("Course" + currentLevel.ToString()).gameObject.SetActive(false);

        lives = 0;
        myScore = 0;

        myPlayer.completedLevel = false;
        myScore = myPlayer.attempts;
        myTotalScore = myTotalScore + myScore;
        myPlayer.attempts = 0;
        myPlayer.gameObject.transform.parent.gameObject.SetActive(false);
        HUD.SetActive(false);
        timeCounter = 0f;
        myPlayer.notMovable = true;

        Escaping();
        GameEnd();
    }

    public void ContinueButton()
    {
        Escaping();
    }

    private string[] itemList = { "Basic", "orange", "lemon", "pumpkin", "onion", "apple", "acorn", "carrot", "candy", "Golden Ball" };
    private string[] unlockedItems = { "Basic", "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA" };

    private string selectedItem = "0";

    [SerializeField] private GameObject customizeUI;

    private GameObject selectedItemUI;

    public void CustomizeOpen()
    {
        customizeUI.SetActive(true);
        buttonsUI.SetActive(false);
        customizeUI.gameObject.transform.Find(selectedItem).gameObject.transform.Find("Highlight").gameObject.GetComponent<Image>().color = Color.red;
        selectedItemUI = customizeUI.gameObject.transform.Find(selectedItem).gameObject;
        for (int i = 0; i < unlockedItems.Length; i++)
        {
            if (unlockedItems[i] == "NA")
            {
                customizeUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Locked";
            }
            else
            {
                customizeUI.gameObject.transform.Find(i.ToString()).gameObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = itemList[i];
            }
        }
    }

    public void CustomizeClose()
    {
        customizeUI.SetActive(false);
        buttonsUI.SetActive(true);
    }

    public void SelectItem(string itemIndex)
    {
        if (unlockedItems[int.Parse(itemIndex)] != "NA")
        {
            selectedItemUI.gameObject.transform.Find("Highlight").gameObject.GetComponent<Image>().color = Color.white;
            customizeUI.gameObject.transform.Find(itemIndex).gameObject.transform.Find("Highlight").gameObject.GetComponent<Image>().color = Color.red;
            selectedItemUI = customizeUI.gameObject.transform.Find(itemIndex).gameObject;
            selectedItem = itemIndex;

            CustomizePlayer(customizeUI.gameObject.transform.Find(itemIndex).gameObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text);
        }
    }

    public void EquipItem(int itemIndex)
    {
        if (unlockedItems[itemIndex] != "NA")
        {
            selectedItem = itemIndex.ToString();
            CustomizePlayer(unlockedItems[itemIndex]);
        }
    }

    private void CustomizePlayer(string itemName)
    {
        for (int i = 0; i < myPlayer.gameObject.transform.Find("Hats").gameObject.transform.childCount; i++)
        {
            myPlayer.gameObject.transform.Find("Hats").gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        myPlayer.gameObject.transform.Find("Hats").gameObject.transform.Find(itemName).gameObject.SetActive(true);

    }
}
