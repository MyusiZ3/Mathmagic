using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseAuth auth;
    public FirebaseFirestore firestore;
    public bool isFirebaseInitialized = false; // Flag to check if Firebase is initialized

    private void Awake()
    {
        // Ensure only one instance of FirebaseManager
        if (Instance == null)
        {
            Instance = this;
            // Keep FirebaseManager object alive across scenes
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Initialize Firebase
        Debug.Log("Initializing Firebase...");
        InitializeFirebase();
    }

    public void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Firebase initialization failed: " + task.Exception);
                return;
            }

            // Firebase initialized successfully
            FirebaseApp app = FirebaseApp.DefaultInstance;
            Debug.Log("FirebaseApp initialized successfully.");

            // Initialize Firebase Auth and Firestore
            auth = FirebaseAuth.DefaultInstance;
            firestore = FirebaseFirestore.DefaultInstance;

            if (auth == null || firestore == null)
            {
                Debug.LogError("Failed to initialize Firebase Auth or Firestore.");
            }
            else
            {
                Debug.Log("Firebase Auth and Firestore initialized successfully.");
            }

            isFirebaseInitialized = true; // Set the flag to true when Firebase is ready

            // Call method after Firebase is ready
            OnFirebaseInitialized();
        });
    }

    // Method to be called after Firebase has been initialized
    private void OnFirebaseInitialized()
    {
        if (auth == null || firestore == null)
        {
            Debug.LogError("Firebase auth or firestore is not initialized properly.");
        }
        else
        {
            Debug.Log("Firebase is ready to use.");
        }
    }
}
