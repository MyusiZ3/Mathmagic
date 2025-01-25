using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;

public class RegisterManager : MonoBehaviour
{
    public InputField nameInput;
    public InputField usernameInput;
    public InputField emailInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;

    public GameObject fillAllFieldsUI;
    public GameObject registrationFailedUI;
    public GameObject connectionFailedUI;

    public void Register()
    {
        string name = nameInput.text;
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(confirmPassword))
        {
            Debug.Log("Harap isi semua field.");
            fillAllFieldsUI.SetActive(true);
            return;
        }

        if (password != confirmPassword)
        {
            Debug.Log("Password tidak cocok.");
            return;
        }

        FirebaseManager.Instance.auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Registrasi gagal: " + task.Exception);

                // Tampilkan UI jika gagal registrasi
                if (task.Exception.ToString().Contains("network"))
                {
                    connectionFailedUI.SetActive(true);
                }
                else
                {
                    registrationFailedUI.SetActive(true);
                }
            }
            else
            {
                FirebaseUser firebaseUser = task.Result.User;
                Debug.Log($"Registrasi sukses: {firebaseUser.Email}");
            }
        });
    }
}
