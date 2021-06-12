using UnityEngine;
using UnityEngine.UI;


public class TopPanel : CustomBehaviour
{
    public Text textScore;
    public Animator anim;

    public override void Init(GameManager gameManager)
    {
        base.Init(gameManager);
        GameManager.ActionOnGameOver += OnGameOver;
    }

    public void SetScoreText(string scoreText)
    {
        textScore.text = scoreText;
    }

    #region Events

    public void OnGameOver()
    {
        anim.SetBool("isGameOver", true);
    }

    public void OnButtonClickRetry()
    {
        GameManager.RestartGame();
    }

    #endregion
}
