using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class BricksManager : MonoBehaviour
{
    #region Singleton

    private static BricksManager _instance;
    public static BricksManager Instance => _instance; // property để bên ngoài truy cập vào, khi goị GameManager.Instance -> trả vè _instance

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

    private int maxRows = 17;
    private int maxCols = 12;
    private float initialBrickSpawnPositionX = -1.96f;
    private float initialBrickSpawnPositionY = 3.325f;
    private float cellSize = 0.365f;

    private GameObject bricksContainer;

    public Brick brickPrefab;

    public Sprite[] brickSprites;
    public Color[] BrickColors;

    public List<Brick> RemainingBricks { get; set; }
    public List<int[,]> LevelsList { get; set; }
    public int InitialBricksCount { get; set; }

    public int CurrentLevelIndex;

    public static event Action OnLevelLoaded;

    private void Start()
    {
        bricksContainer = new GameObject("BricksContainer");
        LevelsList = this.LoadLevelsData();
        this.GenerateBricks();

    }

    public void ReloadLevel(int level)
    {
        this.CurrentLevelIndex = level;
        this.ClearRemainingBricks();
        this.GenerateBricks();
         
    }

    private void ClearRemainingBricks()
    {
        foreach(Brick brick in this.RemainingBricks.ToList())
        { 
            Destroy(brick.gameObject);
        }
    }

    private void GenerateBricks()
    {
        RemainingBricks = new List<Brick>();

        int[,] levelData = LevelsList[CurrentLevelIndex];

        float posX = initialBrickSpawnPositionX;
        float posY = initialBrickSpawnPositionY;

        float zOffset = 0f;
        for (int row = 0; row < maxRows; row++)
        {
            for (int col = 0; col < maxCols; col++)
            {
                int brickType = levelData[row, col];
                if (brickType > 0)
                {
                    Brick newBrick = Instantiate(brickPrefab, new Vector3(posX, posY, 0.0f - zOffset), Quaternion.identity) as Brick;
                    newBrick.Init(bricksContainer.transform, this.brickSprites[brickType - 1], BrickColors[brickType], brickType);

                    RemainingBricks.Add(newBrick);
                    zOffset += 0.0001f;
                }
                posX += cellSize;
                if (col + 1 == maxCols)
                {
                    posX = initialBrickSpawnPositionX;
                }
            }
            posY -= cellSize;
        }

        InitialBricksCount = RemainingBricks.Count;
        OnLevelLoaded?.Invoke();
    }

    private List<int[,]> LoadLevelsData()
    {
        TextAsset textFile = Resources.Load("levels") as TextAsset;

        string[] fileLines = textFile.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        List<int[,]> allLevels = new List<int[,]>();
        int[,] gridOfCurrentLevel = new int[maxRows, maxCols];

        int gridRowIndex = 0;
        for (int row = 0; row < fileLines.Length; row++)
        {
            string line = fileLines[row];

            if (line.IndexOf("--") == -1)
            {
                string[] brickValues = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                for (int col = 0; col < brickValues.Length; col++)
                {
                    gridOfCurrentLevel[gridRowIndex, col] = int.Parse(brickValues[col]);
                }

                gridRowIndex++;
            }
            else
            {
                //end of current level
                gridRowIndex = 0;
                allLevels.Add(gridOfCurrentLevel);
                gridOfCurrentLevel = new int[maxRows, maxCols];
            }
        }

        return allLevels;
    }

    public void LoadNextLevel()
    {
        this.CurrentLevelIndex++;
        if(this.CurrentLevelIndex >= this.LevelsList.Count)
        {
            GameManager.Instance.ShowVictoryScreen();
        }
        else
        {
            this.ReloadLevel(CurrentLevelIndex);
        }
    }
}
