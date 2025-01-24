using UnityEngine;
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private int currentScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadScore();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        SaveScore();

        // Panggil UpdateAchievements dari AchievementManager jika ada
        if (FindObjectOfType<AchievementManager>() != null)
        {
            FindObjectOfType<AchievementManager>().OnScoreUpdated();
        }

        Debug.Log("Score added: " + amount + ", Total Score: " + currentScore);
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void ResetScore()
    {
        currentScore = 0;
        SaveScore();

        if (FindObjectOfType<AchievementManager>() != null)
        {
            FindObjectOfType<AchievementManager>().OnScoreUpdated();
        }

        Debug.Log("Score reset to 0.");
    }

    private void SaveScore()
    {
        PlayerPrefs.SetInt("SCORE", currentScore);
        PlayerPrefs.Save();
    }

    private void LoadScore()
    {
        currentScore = PlayerPrefs.GetInt("SCORE", 0);
        Debug.Log("Score loaded: " + currentScore);
    }
}
