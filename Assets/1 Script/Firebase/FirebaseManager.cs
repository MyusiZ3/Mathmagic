using UnityEngine;
using Firebase;
using Firebase.Auth;

public class FirebaseManager : MonoBehaviour
{
    public FirebaseAuth auth;

    private void Awake()
    {
        // Inisialisasi Firebase saat aplikasi dijalankan
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase berhasil diinisialisasi.");
            }
            else
            {
                Debug.LogError("Firebase tidak tersedia. Periksa konfigurasi!");
            }
        });
    }
}
