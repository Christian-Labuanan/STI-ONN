using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace STI_ONN
{
    internal class AppSettings
    {
        // Property to store the path to the Firebase service account key file
        public string FirebaseCredentialsPath { get; set; }

        // Constructor to initialize the settings (could load from config file or environment)
        public AppSettings()
        {
            // You can initialize the credentials path or load it from a config file/environment variable
            FirebaseCredentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Firebase", "ServiceAccountKey.json");
        }

        // Method to initialize Firebase with the stored credentials
        public FirebaseApp InitializeFirebaseApp()
        {
            if (string.IsNullOrEmpty(FirebaseCredentialsPath) || !File.Exists(FirebaseCredentialsPath))
            {
                throw new Exception("Firebase credentials file not found.");
            }

            try
            {
                return FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(FirebaseCredentialsPath),
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing Firebase app: " + ex.Message);
            }
        }
    }
}
