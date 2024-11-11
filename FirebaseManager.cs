using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System.IO;

namespace STI_ONN
{
    public class FirebaseManager
    {
        // Static field to hold the instance of FirebaseApp
        private static FirebaseApp firebaseApp;

        // Static method to initialize FirebaseApp only once
        public static void InitializeFirebase()
        {
            if (firebaseApp == null)
            {
                try
                {
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Firebase", "ServiceAccountKey.json");

                    if (!File.Exists(filePath))
                    {
                        throw new FileNotFoundException("Service account key file not found.");
                    }

                    // Initialize FirebaseApp with credentials from the service account file
                    firebaseApp = FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(filePath),
                    });

                    Console.WriteLine("Firebase initialized successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error initializing Firebase: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Firebase has already been initialized.");
            }
        }
    }
}
