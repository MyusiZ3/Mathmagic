using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public InputField emailInput;
    public InputField passwordInput;
    public GameObject loginFailedUI;
    public GameObject connectionFailedUI;

    public void Login()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.Log("Harap isi semua field.");
            return;
        }

        FirebaseManager.Instance.auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Login gagal: " + task.Exception);

                // Tampilkan UI jika gagal login
                if (task.Exception.ToString().Contains("network"))
                {
                    connectionFailedUI.SetActive(true);
                }
                else
                {
                    loginFailedUI.SetActive(true);
                }
            }
            else
            {
                FirebaseAuth auth = FirebaseManager.Instance.auth;
                FirebaseUser firebaseUser = task.Result.User;

                // Simpan UserId untuk Remember Me
                PlayerPrefs.SetString("UserId", firebaseUser.Email);
                PlayerPrefs.Save();

                Debug.Log($"Login sukses: {firebaseUser.Email}");

                // Masuk ke halaman utama
                UnityEngine.SceneManagement.SceneManager.LoadScene("2Main_Pages");
            }
        });
    }
}
