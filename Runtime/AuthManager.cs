using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System;
using System.Threading;
using System.IO;
using System.Reflection.Emit;

public class AuthManagerFirebase
{

    private string _signInUrl;
    private string _signUpUrl;
    private string _apiKey;
    private HttpListener _listener;
    private bool isListening;
    private string authCode;
    #region Google-SignIn Params
    private string _clientId;
    private string _clientSecret;

    #endregion Google-SignIn Params

    // Public getters
    public string SignInUrl
    { 
        get { return _signInUrl; } 
    }

    public string SignUpUrl
    {
        get { return _signUpUrl; }
    }

    public string APIKey
    {
        get { return _apiKey; }
    }



    public AuthManagerFirebase(string signInUrl, string signupUrl, string APIkey ,string clientId , string clientSecret)
    {
        this._signInUrl = signInUrl;
        this._signUpUrl = signupUrl;
        this._apiKey = APIkey;
        this._clientId = clientId;
        this._clientSecret = clientSecret;
        this.isListening = false;
        this.authCode = null;
    }


    private class GoogleAuth_Response_Payload
    {
        public string access_token;
        public string expires_in;
        public string refresh_token;
        public string scope;
        public string token_type;
        public string id_token;
        public string userId;

        public GoogleAuth_Response_Payload()
        { }


    }

    /// <summary>
    /// The method is used for signing in a user and generating a token which could be used for accessing the cloud
    /// </summary>
    /// <param name="email">The Email of the user {Must be a valid one}</param>
    /// <param name="pass">The Password of the user {Must be more than 6 characters}</param>
    /// <param name="returnToken">True if you require an ID Token for accessing Cloud. Otherwise false</param>
    /// <returns>
    /// An object of type "Auth_Response Payload" such that :
    /// responseType => True / False based on whether the sign in was successful or not
    /// idToken => Token recieved from the servers 
    /// refreshToken => Token for regenerate the "idToken" when it expires
    /// expiresIn => Time until expiration of the user {in seconds}
    /// localId =>  User ID when signed in.
    /// </returns>
    public async Task<Auth_Response_Payload> SignInUser(string email , string pass , bool returnToken)
    {   
        Auth_Response_Payload payload  = new Auth_Response_Payload();
        UnityWebRequest req = new UnityWebRequest(this._signInUrl+this._apiKey, "POST");
        Payload pl = new Payload(email, pass, returnToken);

        string json = JsonUtility.ToJson(pl);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        var tcs = new TaskCompletionSource<UnityWebRequest>();

        req.SendWebRequest().completed += operation =>  tcs.TrySetResult(req);

        await tcs.Task;

        if (req.result == UnityWebRequest.Result.Success)
        {   
            payload = JsonUtility.FromJson<Auth_Response_Payload>(req.downloadHandler.text);
            payload.responseType = true;
        }


        return payload;
    }

    /// <summary>
    /// The method is used for signing up a user and generating a token which could be used for accessing the cloud
    /// </summary>
    /// <param name="email">The Email of the user {Must be a valid one}</param>
    /// <param name="pass">The Password of the user {Must be more than 6 characters}</param>
    /// <param name="returnToken">True if you require an ID Token for accessing Cloud. Otherwise false</param>
    /// <returns>
    /// An object of type "Auth_Response Payload" such that :
    /// responseType => True / False based on whether the sign up was successful or not
    /// idToken => Token recieved from the servers 
    /// refreshToken => Token for regenerate the "idToken" when it expires
    /// expiresIn => Time until expiration of the user {in seconds}
    /// localId =>  User ID when signed in.
    /// </returns>

    public async Task<Auth_Response_Payload> SignUpUser(string email , string pass , bool returnToken)
    {
        Auth_Response_Payload payload = new Auth_Response_Payload();
        UnityWebRequest req = new UnityWebRequest(this._signUpUrl + this._apiKey, "POST");
        Payload pl = new Payload(email, pass, returnToken);
        string json = JsonUtility.ToJson(pl);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        var tcs = new TaskCompletionSource<UnityWebRequest>();

        req.SendWebRequest().completed += operation =>  tcs.TrySetResult(req);

        await tcs.Task;

        if (req.result == UnityWebRequest.Result.Success)
        {   
            payload = JsonUtility.FromJson<Auth_Response_Payload>(req.downloadHandler.text);
            payload.responseType = true;
        }
        
        Debug.Log(req.downloadHandler.text);
        return payload;
    }

    public void GetAccessCode()
    {
        StartLocalServer();
        Application.OpenURL($"https://accounts.google.com/o/oauth2/v2/auth?client_id={this._clientId}&redirect_uri=http://localhost:8080/auth&response_type=code&scope=email");
        
    }

    public async Task<Auth_Response_Payload> ExchangeAuthCodeWithIDToken(string code)
    {
        GoogleAuth_Response_Payload G_payload = new GoogleAuth_Response_Payload();
        Auth_Response_Payload payload = new Auth_Response_Payload();
        UnityWebRequest req = new UnityWebRequest($"https://oauth2.googleapis.com/token?code={code}" +
                                                  $"&client_id={this._clientId}" +
                                                  $"&client_secret={this._clientSecret}" +
                                                  $"&redirect_uri=http://localhost:8080/auth" +
                                                  $"&grant_type=authorization_code", "POST");
        req.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();

        var tcs = new TaskCompletionSource<UnityWebRequest>();

        req.SendWebRequest().completed += operation => tcs.TrySetResult(req);

        await tcs.Task;

        if (req.result == UnityWebRequest.Result.Success)
        {
            payload.responseType = true;
            G_payload = JsonUtility.FromJson<GoogleAuth_Response_Payload>(req.downloadHandler.text);
            G_payload.userId = DecodeJwt(G_payload.id_token);
            SignInWithToken(G_payload.id_token);
        }
        else 
        {
            Debug.Log(req.downloadHandler.text);
            Debug.Log(req.error);
        }
        MapGoogleAuthPayload(G_payload, ref payload);

        return payload;
        
    }


    /// <summary>
    /// This is used to Map the data recieved from Single Sign On {Google Sign in} to my custom auth payload structure
    /// 
    /// Since the regular email and pass sign had different json attributes from the Google Sign in one...
    /// 
    /// </summary>
    /// <param name="gPayload">Payload recieved from Google Sign in</param>
    /// <param name="payload">Payload to be returned</param>
    /// 
    private void MapGoogleAuthPayload(GoogleAuth_Response_Payload gPayload, ref Auth_Response_Payload payload)
    {
        if (gPayload == null) // Google Payload is null then no need for mapping
            return;

        payload.refreshToken = gPayload.refresh_token;
        payload.idToken = gPayload.id_token;
        payload.expiresIn = gPayload.expires_in;
        payload.localId = gPayload.userId;
    }



    /// <summary>
    /// Method for extracting the User ID from the JWT token recieved after the Google Sign In
    /// </summary>
    /// <param name="jwt">Json Web Token (Must not be NULL)</param>
    /// <returns>User Id</returns>
    /// <additional>This method can extract other attributes from the JWT like email, verified and etc. Hence is liable to be modified as wish for a more deeper look</additional>
    
    string DecodeJwt(string jwt)
    {
        // Extract the payload part of the JWT
        string[] jwtSegments = jwt.Split('.');
        string encodedPayload = jwtSegments[1];

        // Add padding to the payload if needed
        int padding = encodedPayload.Length % 4;
        if (padding > 0)
        {
            encodedPayload += new string('=', 4 - padding);
        }

        // Base64-decode the payload
        byte[] decodedBytes = System.Convert.FromBase64String(encodedPayload);
        string decodedPayload = Encoding.UTF8.GetString(decodedBytes);

        // Parse the JSON payload
        JwtPayload payload = JsonUtility.FromJson<JwtPayload>(decodedPayload);

        if (payload != null)
        {
            // Access claims from the payload
            Debug.Log($"iss: {payload.iss}");
            Debug.Log($"aud: {payload.aud}");
            Debug.Log($"user_id: {payload.user_id}");
            Debug.Log($"sub: {payload.sub}");
            // Add other claims as needed

            // Access the user ID (UID)
            Debug.Log($"User ID (UID): {payload.sub}");
        }
        else
        {
            Debug.LogError("Failed to parse JWT payload.");
        }

        return payload.sub;
    }


    /// ANOTHER METHOD CAN BE ADDED IN FUTURE
    /// void RefreshToken(...) which can be used for requesting a new Id token when it expires.

    public class JwtPayload
    {
        public string iss;
        public string aud;
        public string auth_time;
        public string user_id;
        public string sub;
        public string email;
        public bool email_verified;
        public string at_hash;
        public string iat;
        public string exp;
    }


    private async void SignInWithToken(string idToken)
    {
        UnityWebRequest req = new UnityWebRequest($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithIdp?key={_apiKey}", "POST");
        string json = $"{{\"postBody\":\"id_token={idToken}&providerId=google.com\",\"requestUri\":\"http://localhost\",\"returnIdpCredential\":true,\"returnSecureToken\":true}}";
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        var tcs = new TaskCompletionSource<UnityWebRequest>();
        req.SendWebRequest().completed += operation => tcs.TrySetResult(req);
        await tcs.Task;
        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Sign In Success");
        }
        else
        {
            Debug.Log(req.error);
        }

        Debug.Log(req.downloadHandler.text);
    }



    #region LocalHost Listening

    void StartLocalServer()
    {
        try 
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:8080/auth/");
            _listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            _listener.Start();
            Debug.Log("localhost started");
            isListening = true;
            ListenForAuthCode();
            GameManager.GetInstance().StartCoroutine(WaitForAuthCode());
        }
        catch(Exception e)
        {
            Debug.Log("Error Starting Local Server" + e.Message);
        }
    }

    async void ListenForAuthCode()
    {
        Debug.Log("isListening : "+isListening);
        bool isCoderecieved = false;
        while (isListening)
        {
            try
            {
                HttpListenerContext context =  await _listener.GetContextAsync();
                HttpListenerRequest request = context.Request;
                string url = request.Url.ToString();
                Debug.Log("Received request: " + url);

                // Extract authentication code from URL
                if (url.StartsWith("http://localhost:8080/auth") && !isCoderecieved)
                {
                        isCoderecieved = true;
                        authCode = request.QueryString["code"];
                        Debug.Log("Received authentication code: " + authCode);
                }

                HttpListenerResponse response = context.Response;
                string responseString = "" +
                    "<html>" +
                    "<body>" +
                    "Authorization code received and copied to clip board.\n" +
                    "<br>"+
                    "You can close the window." +
                    "" +
                    "</body></html>";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();

                if (_listener != null && _listener.IsListening && isCoderecieved)
                {
                    isListening = false;
                    _listener.Stop();
                }

            }
            catch (Exception e)
            {
                Debug.LogWarning("Error handling request: " + e.Message);
            }
        }
    }


    private IEnumerator WaitForAuthCode()
    {

        while (authCode == null)
        {

            yield return null;
        }

        GUIUtility.systemCopyBuffer = authCode;
        Debug.Log("Auth Code copied Successfully");


    }


    #endregion


}




