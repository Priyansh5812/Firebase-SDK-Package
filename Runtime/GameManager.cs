using UnityEngine;
using UnityEngine.Android;

#region Additional Structs and Classes
/// <summary>
/// In this region classes and structs which are being used for purposes like... deserialization of data  or initialization of Generic Type and etc.
/// </summary>
[System.Serializable]
public class Player
{
    public int Health;
    public int Attack;
    public Player()
    {
        // Must be empty.
    }

    public Player(int Health, int Attack)
    {
        this.Health = Health;
        this.Attack = Attack;
    }



}

[System.Serializable]

public class Auth_Response_Payload
{
    public bool responseType;
    public string idToken;
    public string refreshToken;
    public string expiresIn;
    public string localId; //Also known as UserID
    public Auth_Response_Payload() 
    {
        responseType = false;
    }

    public Auth_Response_Payload(string idToken, string refreshToken, string expiresIn, string localId)
    {   
        this.idToken = idToken;
        this.refreshToken = refreshToken;
        this.expiresIn = expiresIn;
        this.localId = localId;
    }
}

public class Payload
{
    public string email, password;
    public bool returnSecureToken;

    public Payload(string email, string password, bool returnSecureToken)
    {
        this.email = email;
        this.password = password;
        this.returnSecureToken = returnSecureToken;
    }
}


#endregion Additional Structs and Classes




/// <summary>
/// Class used for managing the Firebase Instance and its own Singleton Instance
/// Other fields are required to be filled via editor like urls and IDs and etc...
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    
    #region Firebase - Based references
    //RealTime Database based
    [SerializeField]private string databaseUrl;
    //Authentication Based
    [SerializeField] private string signInUrl;
    [SerializeField] private string signUpUrl;
    [SerializeField] private string clientId;
    [SerializeField] private string clientSecret;
    [SerializeField] private string APIKey;
    [SerializeField] private string projectID;
    [SerializeField] private string bucketUrl;
    private FirebaseManager _fbManager;

    public FirebaseManager fbManager
    {
        get
        {
            if (this._fbManager == null)
            {
                InitializeFirebase();
            }
            return this._fbManager;
        }
    }

    #endregion Firebase - Based references
    void Awake()
    {
        //Initializing the instance of its own and Firebase
        InitializeInstance();
        InitializeFirebase();


        // Checking for the memory read access
        if(Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead) == false)
            Permission.RequestUserPermission(Permission.ExternalStorageRead); // If does not have, then request it.

        NetworkReachability reachability = Application.internetReachability;

        // Handle network reachability status
        switch (reachability)
        {
            case NetworkReachability.NotReachable:
                Debug.Log("No network access available.");
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                Debug.Log("Network access available via mobile data.");
                break;
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                Debug.Log("Network access available via Wi-Fi or LAN.");
                break;
        }


    }

    private void InitializeInstance()
    { 
        _instance = this;
    }

    private void InitializeFirebase()
    {
        if (_fbManager != null)
        {
            return;
        }

        if(databaseUrl.Length == 0)
        {
            return;
        }

        _fbManager = new FirebaseManager(databaseUrl , signInUrl, signUpUrl, APIKey, projectID , bucketUrl, clientId , clientSecret);
    }

    public static GameManager GetInstance()
    {
        return _instance;
    } //GameManager Instance can be accessed via this static method





    
}


