using Firebase.Auth;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutManager : MonoBehaviour
{
    public void Logout()
    {
        FirebaseManager.Instance.auth.SignOut();

        // Hapus UserId dari PlayerPrefs
        PlayerPrefs.DeleteKey("UserId");

        // Kembali ke halaman login
        SceneManager.LoadScene("LoginScene");
    }
}
