using UnityEngine;
using UnityEngine.UI;
public class MoveScore : MonoBehaviour
{
    public float lifeTime = 2f;

    [SerializeField]
    private Text scoreText;


    public void SetScoreText(string text)
    {
        scoreText.text = text;
        GetComponent<Animation>().Play();
        Destroy(gameObject, lifeTime);
    }
   
}
