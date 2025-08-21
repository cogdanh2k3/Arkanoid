using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager _instance;
    public static GameManager Instance => _instance; // property để bên ngoài truy cập vào, khi goị GameManager.Instance -> trả vè _instance

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    public GameObject gameOverScreen;
    public GameObject gameVictoryScreen;

    public int AvailableLives = 3;
    public int Lives { get; set; }

    public bool IsGameStarted { get; set; }

    public static event Action<int> OnLiveLost;

    private void Start()
    {
        Lives = AvailableLives;
        Screen.SetResolution(540, 960, false); //false - window; true - full screen
        Ball.OnBallDeath += OnBallDeath;
        Brick.OnBrickDestruction += OnBrickDestruction;
    }

    private void OnBrickDestruction(Brick brick)
    {
        if (BricksManager.Instance.RemainingBricks.Count <= 0)
        {
            BallsManager.Instance.ResetBall();
            GameManager.Instance.IsGameStarted = false;
            BricksManager.Instance.LoadNextLevel();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnBallDeath(Ball ball)
    {
        if(BallsManager.Instance.Balls.Count <= 0)
        {
            this.Lives--;
            if(this.Lives < 1)
            {
                // Show GameOver
                gameOverScreen.SetActive(true);
            }
            else
            {
                OnLiveLost?.Invoke(this.Lives);

                //reset balls
                BallsManager.Instance.ResetBall();

                //stop the game
                IsGameStarted = false;

                //reload level
                BricksManager.Instance.ReloadLevel(BricksManager.Instance.CurrentLevelIndex);
            }
        }
    }
    private void OnDisable()
    {
        Ball.OnBallDeath -= OnBallDeath;
    }

    public void ShowVictoryScreen()
    {
        gameVictoryScreen.SetActive(true);
    }
}
