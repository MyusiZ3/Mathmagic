using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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
    public GameObject registrationSuccessUI;
    public GameObject firebaseNotInitializedUI; // New UI for Firebase initialization failure

    private CanvasGroup fillAllFieldsCanvasGroup;
    private CanvasGroup registrationFailedCanvasGroup;
    private CanvasGroup connectionFailedCanvasGroup;
    private CanvasGroup registrationSuccessCanvasGroup;
    private CanvasGroup firebaseNotInitializedCanvasGroup;

    private void Start()
    {
        // Setup CanvasGroup for each UI
        SetupUI();

        // Initialize Firebase
        InitializeFirebase();
    }

    private void SetupUI()
    {
        if (fillAllFieldsUI != null)
            fillAllFieldsCanvasGroup = SetupCanvasGroup(fillAllFieldsUI);

        if (registrationFailedUI != null)
            registrationFailedCanvasGroup = SetupCanvasGroup(registrationFailedUI);

        if (connectionFailedUI != null)
            connectionFailedCanvasGroup = SetupCanvasGroup(connectionFailedUI);

        if (registrationSuccessUI != null)
            registrationSuccessCanvasGroup = SetupCanvasGroup(registrationSuccessUI);

        if (firebaseNotInitializedUI != null)
            firebaseNotInitializedCanvasGroup = SetupCanvasGroup(firebaseNotInitializedUI);
    }

    private void InitializeFirebase()
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.isFirebaseInitialized)
        {
            // Firebase is not initialized yet
            if (firebaseNotInitializedCanvasGroup != null)
            {
                StartCoroutine(FadeInAndOut(firebaseNotInitializedCanvasGroup));
            }
            return;
        }

        FirebaseManager.Instance.InitializeFirebase();
    }

    public void Register()
    {
        string name = nameInput.text;
        string username = usernameInput.text;
        string email = emailInput.text;
        string password = passwordInput.text;
        string confirmPassword = confirmPasswordInput.text;

        // Validate input
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(confirmPassword))
        {
            Debug.Log("Harap isi semua field.");
            if (fillAllFieldsCanvasGroup != null)
            {
                StartCoroutine(FadeInAndOut(fillAllFieldsCanvasGroup));
            }
            return;
        }

        if (password != confirmPassword)
        {
            Debug.Log("Password tidak cocok.");
            return;
        }

        // Ensure Firebase Auth is initialized
        if (FirebaseManager.Instance.auth == null)
        {
            Debug.LogError("Firebase Auth belum diinisialisasi!");
            return;
        }

        // Register user
        FirebaseManager.Instance.auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Registrasi gagal: " + task.Exception);

                // Show appropriate UI based on error type
                if (task.Exception != null && task.Exception.ToString().Contains("network"))
                {
                    if (connectionFailedCanvasGroup != null)
                    {
                        StartCoroutine(FadeInAndOut(connectionFailedCanvasGroup));
                    }
                }
                else
                {
                    if (registrationFailedCanvasGroup != null)
                    {
                        StartCoroutine(FadeInAndOut(registrationFailedCanvasGroup));
                    }
                }
            }
            else
            {
                FirebaseUser firebaseUser = task.Result.User;
                Debug.Log($"Registrasi sukses: {firebaseUser.Email}");

                // Show registration success UI
                if (registrationSuccessCanvasGroup != null)
                {
                    StartCoroutine(FadeInAndOut(registrationSuccessCanvasGroup));
                }

                // Save user data to Firestore
                SaveUserData(firebaseUser.UserId, name, username, email);
            }
        });
    }

    private void SaveUserData(string userId, string name, string username, string email)
    {
        if (FirebaseManager.Instance.firestore == null)
        {
            Debug.LogError("Firestore belum diinisialisasi!");
            return;
        }

        FirebaseFirestore firestore = FirebaseManager.Instance.firestore;

        var userData = new
        {
            userId = userId,
            name = name,
            username = username,
            email = email,
            score = 0, // Default score
            createdAt = Timestamp.GetCurrentTimestamp()
        };

        firestore.Collection("users").Document(userId).SetAsync(userData).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Data pengguna berhasil disimpan di Firestore!");
            }
            else
            {
                Debug.LogError("Gagal menyimpan data pengguna: " + task.Exception);
            }
        });
    }

    private CanvasGroup SetupCanvasGroup(GameObject uiObject)
    {
        CanvasGroup canvasGroup = uiObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = uiObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        return canvasGroup;
    }

    private IEnumerator FadeInAndOut(CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(Fade(canvasGroup, 0f, 1f, 0.5f));

        yield return new WaitForSeconds(2f);

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
