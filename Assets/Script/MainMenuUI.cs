using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Drag & drop TextMeshPro Text di Unity Editor

    void Start()
    {
        UpdateScoreDisplay();
    }

    void OnEnable()
    {
        UpdateScoreDisplay();
    }

    // Fungsi untuk memperbarui teks skor
    public void UpdateScoreDisplay()
    {
        if (ScoreManager.Instance != null)
        {
            int score = ScoreManager.Instance.GetCurrentScore();
            scoreText.text = score.ToString();
            Debug.Log("Skor ditampilkan di Main Menu: " + score);
        }
        else
        {
            scoreText.text = "0";
            Debug.LogWarning("ScoreManager.Instance is null. Skor ditampilkan: 0");
        }
    }
}
