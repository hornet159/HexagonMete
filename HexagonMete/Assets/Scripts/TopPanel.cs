using System.Collections;
using System.Collections.Generic;
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

    public void OnGameOver()
    {
        anim.SetBool("isGameOver", true);
    }
}
