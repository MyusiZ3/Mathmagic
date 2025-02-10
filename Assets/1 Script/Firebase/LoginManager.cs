using Firebase.Auth;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections;

public class LoginManager : MonoBehaviour
{
    public InputField emailInput;
    public InputField passwordInput;
    public GameObject loginFailedUI; // UI Gagal Login
    public GameObject connectionFailedUI; // UI Koneksi Gagal
    private CanvasGroup loginFailedCanvasGroup;
    private CanvasGroup connectionFailedCanvasGroup;

    private void Start()
    {
        // Setup CanvasGroup untuk loginFailedUI
        if (loginFailedUI != null)
        {
            loginFailedCanvasGroup = loginFailedUI.GetComponent<CanvasGroup>();
            if (loginFailedCanvasGroup == null)
            {
                loginFailedCanvasGroup = loginFailedUI.AddComponent<CanvasGroup>();
            }
            // Pastikan alpha diatur ke 0 meskipun objek aktif
            loginFailedCanvasGroup.alpha = 0;
            loginFailedCanvasGroup.interactable = false;
            loginFailedCanvasGroup.blocksRaycasts = false;
        }

        // Setup CanvasGroup untuk connectionFailedUI
        if (connectionFailedUI != null)
        {
            connectionFailedCanvasGroup = connectionFailedUI.GetComponent<CanvasGroup>();
            if (connectionFailedCanvasGroup == null)
            {
                connectionFailedCanvasGroup = connectionFailedUI.AddComponent<CanvasGroup>();
            }
            // Pastikan alpha diatur ke 0 meskipun objek aktif
            connectionFailedCanvasGroup.alpha = 0;
            connectionFailedCanvasGroup.interactable = false;
            connectionFailedCanvasGroup.blocksRaycasts = false;
        }
    }

    public async void Login()
    {
        string email = emailInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            Debug.Log("Harap isi semua field.");
            return;
        }

        try
        {
            // Menggunakan await untuk menunggu hasil login
            var authResult = await FirebaseManager.Instance.auth.SignInWithEmailAndPasswordAsync(email, password);

            FirebaseUser firebaseUser = authResult.User;

            // Simpan UserId untuk Remember Me
            PlayerPrefs.SetString("UserId", firebaseUser.Email);
            PlayerPrefs.Save();

            Debug.Log($"Login sukses: {firebaseUser.Email}");

            // Masuk ke halaman utama
            UnityEngine.SceneManagement.SceneManager.LoadScene("2Main_Pages");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Login gagal: " + e.Message);

            // Tampilkan alert sesuai kondisi
            if (e.Message.Contains("network"))
            {
                if (connectionFailedCanvasGroup != null)
                {
                    StartCoroutine(FadeInAndOut(connectionFailedCanvasGroup));
                }
                else
                {
                    Debug.LogWarning("connectionFailedUI tidak terhubung!");
                }
            }
            else
            {
                if (loginFailedCanvasGroup != null)
                {
                    StartCoroutine(FadeInAndOut(loginFailedCanvasGroup));
                }
                else
                {
                    Debug.LogWarning("loginFailedUI tidak terhubung!");
                }
            }
        }
    }

    private IEnumerator FadeInAndOut(CanvasGroup canvasGroup)
    {
        // Aktifkan UI dengan fade-in
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(Fade(canvasGroup, 0f, 1f, 0.5f));

        // Tahan UI selama 2 detik
        yield return new WaitForSeconds(2f);

        // Nonaktifkan UI dengan fade-out
        yield return StartCoroutine(Fade(canvasGroup, 1f, 0f, 0.5f));
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float startAlpha, float targetAlpha, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
