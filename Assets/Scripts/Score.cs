using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Transform player;
    public Text textScore;
    public Text HighScore;

    void Start() {
        HighScore.text = PlayerPrefs.GetFloat("HighScore",0).ToString("H-00;\"00\"");
    }

    void Update()
    {
        float highScore = player.position.y;
        textScore.text = highScore.ToString("00;\"00\"");

        if (highScore > PlayerPrefs.GetFloat("HighScore",0))
        {
            PlayerPrefs.SetFloat("HighScore", highScore);
            HighScore.text = highScore.ToString("00;\"00\"");
        }
    }
}
