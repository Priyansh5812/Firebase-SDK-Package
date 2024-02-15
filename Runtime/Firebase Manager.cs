public class FirebaseManager
{
    
    /// <summary>
    /// Here contains the references of the Managers which can be retrived by using the reference from the GameManager class
    /// </summary>

    #region References

    private DatabaseManager<Player> _dbManager;
    public DatabaseManager<Player> dbManager
    {
        get 
        {

            return this._dbManager;
            
        }
        
    }

    private AuthManagerFirebase _authManager;
    public AuthManagerFirebase authManager
    {
        get 
        {
            return this._authManager;
        }
        
    }

    private FirestoreManager _fsManager;
    public FirestoreManager fsManager
    {
        get 
        {
            return _fsManager;
        }
    }

    private StorageManager _sManager;
    public StorageManager sManager
    {
        get 
        {
            return this._sManager;
        }
    }

    #endregion References

    public FirebaseManager() { }

    public FirebaseManager(string databaseUrl , string signInUrl , string signUpUrl, string APIkey, string projectID ,string bucketUrl, string clientId, string clientSecret)
    {

        //Initializing References
        if (databaseUrl != null)
            this._dbManager = new DatabaseManager<Player>(databaseUrl); // Here I have used a generic as per my choice. You can create your own type of instance of Database as you wish

        if (signInUrl != null && signUpUrl != null && APIkey != null)

            this._authManager = new AuthManagerFirebase(signInUrl, signUpUrl, APIkey, clientId , clientSecret);

        if (projectID != null)
            this._fsManager = new FirestoreManager(projectID);

        if (bucketUrl != null)
            this._sManager = new StorageManager(bucketUrl);
        
  

    }





}
