# Firebase Package for VR Development.
This Repository is for Unity in order to implement firebase operations for VR builds (both PC and Standalone). 
## Setup Instructions
Firebase Project Setup: Create a Firebase project and configure it according to your VR
application's needs. Follow the setup guide in the [Firebase Console](https://console.firebase.google.com/).
Authentication and Authorization: Set up your Firebase project to use the REST API for
authentication and authorization. Consult the [Firebase Authentication REST API documentation](https://firebase.google.com/docs/reference/rest/auth).

## User Authentication
Getting Started with Firebase Auth REST API
Utilize the [Firebase Authentication REST API](https://firebase.google.com/docs/reference/rest/auth) for user authentication. <br>
For using the SDK you are required to use initialize the required fields in **GameManager.cs** which can found mostly in Firebase console and docs.
## Creating VR UI Elements
Design and implement VR-compatible UI elements for user interactions such as registration,
login, and logout etc.
## User Authentication Scripts
● The AuthManager.cs in Unity is designed to handle user authentication using a RESTful
API. It includes methods for signing in and signing up users.<br>
● It makes use of the Unity UnityWebRequest for communication with a server.<br>
● The authentication-related URLs and API key are set during the manager's initialization.<br>
● The Auth_Response_Payload class represents the payload structure for the response
obtained from user authentication operations.<br>
● It contains fields representing various pieces of information returned by the
authentication service, such as tokens, expiration details, and the user's local identifier.<br>
● The GoogleAuth_Response_Payload is used in terms of getting the response from
different endpoint {oauth2.googleapis.com} and later is parsed into the original structure
format {Auth_Response_Payload}<br>

## Authentication
1) Email and Password Sign-up / Sign-in Section:
● Create a new email and password user by issuing an HTTP POST request
to the Auth signupNewUser endpoint.<br>
● Sign-Up
URL:https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=[API_KEY]
● Sign in a user with an email and password by issuing an HTTP POST
request to the Auth verifyPassword endpoint.<br>
● Sign-In URL :
https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=[API_
KEY]<br>
2) Google Sign-In Setup
● Implement Google Sign-In using the REST API. Follow the guidelines for [Google
Sign-In with Firebase REST API](https://firebase.google.com/docs/reference/rest/auth#section-create-auth-provider).<br>
● For obtaining the Client ID and Client Secret, follow the instructions in this
[YouTube Tutorial](https://www.youtube.com/watch?v=FvJ2zWKshF4&list=PLH1EsavvpUovSzv5e_lWTnU54wwRUxYT4&index=19) starting from 4:50.<br>
● Since we are using REST API, the use of HTTP Listener is taken into action such
that we can listen for the response via a free port,(here we have used 8080). This
methodology is used to open the browser window for sign-in purposes and get
the “OAuth code” in order to exchange from the token ID in later execution of
code.<br>
● The Google Sign In is method is not supported on VR devices, according to the
documentation, Google sign in on android devices can only be done through the
Google Sign in SDK, which is not supported on VR Devices.<br>
For more info:
[https://developers.google.com/identity/protocols/oauth2/resources/loopback-migration](https://developers.google.com/identity/protocols/oauth2/resources/loopback-migration). <br>




## Handling Main Thread Dispatch
If you encounter threading issues, ensure that your authentication-related code is executed on
the main Unity thread. Consider integrating a mechanism to handle actions in the main Unity
thread.
## Firestore Database
Getting Started
-Set up your Firestore database and configure it for REST API access. Refer [Firebase Authentication REST API docs](https://firebase.google.com/docs/reference/rest/auth).
● The FirestoreManager class provides functionalities for interacting with Google Firestore,
including creating collections, retrieving field values, updating fields, and deleting
documents.<br>
● It utilizes Unity's UnityWebRequest for communication with Firestore REST API
endpoints.<br>
##Realtime Database
Getting Started<br>
Set up your real-time database and configure it for REST API access. [Firebase Authentication REST API docs](https://firebase.google.com/docs/reference/rest/database).
Database Script<br>
● The DatabaseManager script is a versatile tool designed for real-time interaction with a
Firebase Realtime Database using Unity's UnityWebRequest.<br>
● It facilitates common operations such as updating, creating, retrieving, and deleting
nodes in the database.<br>
## Storage
Getting Started<br>
-Set up your Cloud Storage and configure it for REST API access. Refer to the Firestore REST
API documentation.<br>
● The StorageManager script is responsible for managing the upload of image and audio
files to a specified storage bucket.<br>
● It utilizes Unity's UnityWebRequest for making HTTP requests to upload files.<br>
● In the SDK we have developed only upload functionality of Image and Audio files.
Security Rules<br>
##Firestore and Storage Rules
Customize Firestore and Storage security rules based on your project requirements. [Refer to
the Firestore Security Rules](https://firebase.google.com/docs/reference/rest/storage/rest)<br>
## Common Issues and Solutions<br>
Firebase REST API Key: Ensure your Firebase REST API key is securely stored and not
exposed in the client-side code.<br>
Error Handling: Implement robust error handling mechanisms for API calls, considering
potential connectivity issues and API response errors.<br>
Threading Issues: If threading issues persist, consider designing a solution to handle actions in
the main Unity thread when necessary<br>



