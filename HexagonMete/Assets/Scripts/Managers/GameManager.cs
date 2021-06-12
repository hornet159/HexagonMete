using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public event Action ActionOnGameOver;

    //CustomBehaviours
    public HexagonGrid hexagonGrid;
    public TripleHexagonPointer tripleHexagonPointer;
    public TopPanel topPanel;

    //Data
    public GameOptions gameOptions;


    public void Awake()
    {
        hexagonGrid.Init(this);
        tripleHexagonPointer.Init(this);
        topPanel.Init(this);
        Score = 0;
        mBombCount = 0;
    }

    [SerializeField]
    private int mScore;
    [SerializeField]
    private int mBombCount;

    public bool IsGameOver { get; private set; }

    public int Score
    {
        get { return mScore; }
        set
        {
            mScore = value;
            topPanel.SetScoreText(value.ToString());
        }
    }

    public int BombCount
    {
        get { return mBombCount; }
        set
        {
            mBombCount = value;
        }
    }

    public bool isBombComing
    {
        get
        {
            if (gameOptions.PointPerBomb > 0)
                return (Score / gameOptions.PointPerBomb) > mBombCount;
            else
                return false;
        }
    }

    public void AddScore(int explodedHexagonCount)
    {
        Score += explodedHexagonCount * gameOptions.HexagonPoint;
    }

    public void GameOver()
    {
        IsGameOver = true;
        hexagonGrid.ExplodeAll();
        ActionOnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainScene");
    }
}
