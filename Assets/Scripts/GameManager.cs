using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public GameObject gameOverScreen;

    public int AvailableLives = 3;
    public int Lives { get; set; }

    public bool IsGameStarted { get; set; }

    public static event Action<int> OnLiveLost;

    public event EventHandler OnGamePaused;
    public event EventHandler OnGameResume;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        Lives = AvailableLives;
        Ball.OnBallDeath += OnBallDeath;
        Brick.OnBrickDestruction += OnBrickDestruction;

        GameInput.Instance.OnMenuBtnPressed += GameInput_OnMenuBtnPressed;
    }

    private void GameInput_OnMenuBtnPressed(object sender, EventArgs e)
    {
        PauseResumeGame();
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
        SceneLoader.LoadScene(SceneLoader.Scene.GameScene);
    }

    public void PauseResumeGame()
    {
        if(Time.timeScale == 1f)
        {
            PauseGame();
        }
        else
        {
            ContinueGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        OnGamePaused?.Invoke(this, EventArgs.Empty);
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        OnGameResume?.Invoke(this, EventArgs.Empty);

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
}
