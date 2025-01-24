using UnityEngine;
using Google;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;

public class GoogleSignInManager : MonoBehaviour
{
    private FirebaseAuth auth;

    private void Start()
    {
        // Inisialisasi Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                Debug.Log("Firebase berhasil diinisialisasi.");
            }
            else
            {
                Debug.LogError($"Firebase tidak tersedia: {task.Result}");
            }
        });
    }

    // Fungsi untuk menangani klik tombol Sign-In
    public void SignInWithGoogle()
    {
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            WebClientId = "953317182090-mjhind3edqi5l5r1trbdcbmsl119hao4.apps.googleusercontent.com", // Ganti dengan Web Client ID dari Firebase
            RequestIdToken = true
        };

        // Mulai proses sign-in
        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Google Sign-In gagal.");
                return;
            }

            GoogleSignInUser googleUser = task.Result;
            Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);

            auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(authTask =>
            {
                if (authTask.IsCanceled || authTask.IsFaulted)
                {
                    Debug.LogError("Firebase Sign-In gagal.");
                    return;
                }

                FirebaseUser newUser = authTask.Result;
                Debug.Log($"Firebase user signed in: {newUser.DisplayName} ({newUser.UserId})");
            });
        });
    }
}
