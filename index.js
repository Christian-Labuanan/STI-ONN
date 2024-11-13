// Import Firebase Admin SDK
var admin = require("firebase-admin");

// Import Firebase config for client-side (optional, if needed)
const firebaseConfig = require("./Firebase/firebaseConfig.json");

// Import the service account key for Firebase Admin SDK
var serviceAccount = require("./Firebase/serviceAccountKey.json");

// Initialize Firebase Admin SDK with service account credentials
admin.initializeApp({
    credential: admin.credential.cert(serviceAccount),
    databaseURL: "https://sti-onn-d0161-default-rtdb.asia-southeast1.firebasedatabase.app"
});

// Optionally, if you want to set up Firebase for the frontend, you can use:
const { initializeApp } = require("firebase/app");
initializeApp(firebaseConfig);

// Now you can use Firebase Admin SDK and other Firebase features in your backend
// For example, accessing Firestore, Authentication, or Realtime Database
