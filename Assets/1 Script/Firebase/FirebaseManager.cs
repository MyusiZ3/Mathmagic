using System;
using Firebase;
using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;

    public FirebaseAuth auth;
    public FirebaseUser user;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;

                // Cek apakah user sudah login sebelumnya
                if (PlayerPrefs.HasKey("UserId"))
                {
                    SceneManager.LoadScene("MainScene"); // Ganti dengan scene utama Anda
                }
            }
            else
            {
                Debug.LogError("Firebase tidak tersedia: " + task.Result.ToString());
            }
        });
    }
}
