using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private LeaderboardManager leaderboardManager;

    [Header("Stair")]
    [Space(10)]
    public GameObject[] Stairs;

    [Header("UI")]
    [Space(10)]
    public GameObject UI_GameOver;
    public GameObject InstructionsPanel;
    public TextMeshProUGUI textMaxScore;
    public TextMeshProUGUI textNowScore;
    public TextMeshProUGUI textShowScore;
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI leaderboardText;
    public GameObject gamePanel;

    private int maxScore = 0;
    private int nowScore = 0;
    private float timer = 90f;

    public bool isGameOver = false;
    public bool isGameStarted = false;

    private enum State { Start, Left, Right };
    private State state;
    private Vector3 oldPosition;

    private List<int> leaderboardScores = new List<int>();

    private async void Start()
    {
        Instance = this;
        leaderboardManager = GetComponent<LeaderboardManager>();
        await UGSInitializer.Instance.EnsureInitialized();
        LoadLeaderboard();
        InitStairs();

        ShowInstructionsPanel();
    }

    void Update()
    {
        if (!isGameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space) && InstructionsPanel.activeSelf)
            {
                StartGame();
            }
            return;
        }

        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                RestartGame();
            }
            return;
        }

        if (!isGameOver)
        {
            timer -= Time.deltaTime;
            textTimer.text = Mathf.Ceil(timer).ToString();

            if (timer <= 0)
            {
                GameOver();
            }
        }
    }

    public void Init()
    {
        state = State.Start;
        oldPosition = Vector3.zero;

        nowScore = 0;
        timer = 90f;
        isGameOver = false;
        isGameStarted = false;

        textShowScore.text = nowScore.ToString();
        textTimer.text = Mathf.Ceil(timer).ToString();

        UI_GameOver.SetActive(false);
        InstructionsPanel.SetActive(false);
        gamePanel.SetActive(true);
    }

    public void InitStairs()
    {
        for (int i = 0; i < Stairs.Length; i++)
        {
            switch (state)
            {
                case State.Start:
                    Stairs[i].transform.position = new Vector3(0.75f, 0.135f, 0);
                    state = State.Right;
                    break;

                case State.Left:
                    Stairs[i].transform.position = oldPosition + new Vector3(-0.75f, 0.5f, 0);
                    break;

                case State.Right:
                    Stairs[i].transform.position = oldPosition + new Vector3(0.75f, 0.5f, 0);
                    break;
            }

            oldPosition = Stairs[i].transform.position;

            if (i != 0)
            {
                int ran = Random.Range(0, 5);

                if (ran < 2 && i < Stairs.Length - 1)
                {
                    state = state == State.Left ? State.Right : State.Left;
                }
            }
        }
    }

    public void SpawnStair(int cnt)
    {
        int ran = Random.Range(0, 5);

        if (ran < 2)
        {
            state = state == State.Left ? State.Right : State.Left;
        }

        switch (state)
        {
            case State.Left:
                Stairs[cnt].transform.position = oldPosition + new Vector3(-0.75f, 0.5f, 0);
                break;

            case State.Right:
                Stairs[cnt].transform.position = oldPosition + new Vector3(0.75f, 0.5f, 0);
                break;
        }

        oldPosition = Stairs[cnt].transform.position;
    }

    public async void GameOver()
    {
        isGameOver = true;
        ShowGameOver();
        UpdateLeaderboard(nowScore);
        await leaderboardManager.SubmitScore(nowScore);
    }

    private void ShowGameOver()
    {
        UI_GameOver.SetActive(true);

        if (nowScore > maxScore)
        {
            maxScore = nowScore;
        }

        textMaxScore.text = maxScore.ToString();
        textNowScore.text = nowScore.ToString();
    }

    public void AddScore()
    {
        if (!isGameOver)
        {
            nowScore++;
            textShowScore.text = nowScore.ToString();
        }
    }

    public void StartGame()
    {
        isGameStarted = true;
        InstructionsPanel.SetActive(false);
        gamePanel.SetActive(true);
        UI_GameOver.SetActive(false);
        isGameOver = false;

        textShowScore.text = nowScore.ToString();
        textTimer.text = Mathf.Ceil(timer).ToString();
    }

    private void ShowInstructionsPanel()
    {
        InstructionsPanel.SetActive(true);
        gamePanel.SetActive(false);
        UI_GameOver.SetActive(false);
    }

    private void LoadLeaderboard()
    {
        leaderboardScores.Clear();
        for (int i = 0; i < 5; i++)
        {
            leaderboardScores.Add(PlayerPrefs.GetInt("HighScore" + i, 0));
        }
        UpdateLeaderboardUI();
    }

    private void SaveLeaderboard()
    {
        for (int i = 0; i < leaderboardScores.Count; i++)
        {
            PlayerPrefs.SetInt("HighScore" + i, leaderboardScores[i]);
        }
        PlayerPrefs.Save();
    }

    private void UpdateLeaderboard(int newScore)
    {
        leaderboardScores.Add(newScore);
        leaderboardScores.Sort((a, b) => b.CompareTo(a));
        if (leaderboardScores.Count > 5)
        {
            leaderboardScores.RemoveAt(5);
        }
        SaveLeaderboard();
        UpdateLeaderboardUI();
    }

    private void UpdateLeaderboardUI()
    {
        leaderboardText.text = "Leaderboard\n";
        for (int i = 0; i < leaderboardScores.Count; i++)
        {
            leaderboardText.text += (i + 1) + ". " + leaderboardScores[i] + "\n";
        }
    }

    public void RestartGame()
    {
        Init();
        InitStairs();


        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.GetComponent<Player>().Init();
        }

        StartGame();
    }
}
