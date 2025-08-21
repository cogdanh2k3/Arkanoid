using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text TargetText;
    public Text ScoreText;
    public Text LivesText;

    public int Score { get; set; }

    private void Awake()
    {
        Brick.OnBrickDestruction += OnBrickDestruction;
        BricksManager.OnLevelLoaded += OnLevelLoaded;
        GameManager.OnLiveLost += OnLiveLost;
    }

    private void Start()
    {
        OnLiveLost(GameManager.Instance.AvailableLives);
    }

    private void OnLiveLost(int remainingLives)
    {
        LivesText.text = $@"LIVES: {remainingLives}";
    }

    private void OnLevelLoaded()
    {
        UpdateRemaniningBricksText();
        UpdateScoreText(0);
    }

    private void UpdateScoreText(int increment)
    {
        Score += increment;
        string scoreString = this.Score.ToString().PadLeft(5, '0');
        ScoreText.text = $"SCORE:{Environment.NewLine}{scoreString}";
    }

    private void OnBrickDestruction(Brick brick)
    {
        UpdateRemaniningBricksText();
        UpdateScoreText(10);
    }

    private void UpdateRemaniningBricksText()
    {
        TargetText.text = $"TARGET:{Environment.NewLine}{BricksManager.Instance.RemainingBricks.Count} / {BricksManager.Instance.InitialBricksCount}";
    }

    public void OnDisable()
    {
        Brick.OnBrickDestruction -= OnBrickDestruction;
        BricksManager.OnLevelLoaded -= OnLevelLoaded;
    }
}
