using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


// Helper classes for deserialization
[System.Serializable]
public class FirestoreResponseDocument
{
    public string name;
    public Fields fields;
    public string createTime;
    public string updateTime;

}

[System.Serializable]
public class FirestoreUploadDocument
{
    public Fields fields;
    public FirestoreUploadDocument()
    { 
        fields = new Fields();
    }
}


/// <summary>
/// This class "Fields" used for registering an entity inside the database
/// Customize this class as per your choice
/// </summary>
[System.Serializable]
public class Fields
{
    public Value Health;
    public Value Attack;
    public Value Score;
    public Value Date;
    public Fields()
    { 
        Health = new Value();
        Attack = new Value();
        Score = new Value();
        Date = new Value();
    }
}




[System.Serializable]
public class Value
{
    public string stringValue;
}


/// <summary>
/// This class can also be used with Authentication Header if we have the required Token ID
/// This class need to be communicated with AuthManager.cs such that it can have access to the required token ID and even after generation of the new token ID.
/// </summary>
public class FirestoreManager
{
    public string projectId;


    public FirestoreManager(string projectID)
    {
        this.projectId = projectID;
    }

    
    /// <summary>
    /// This is used to create a Collection using collectionId and documentId
    /// in the Firestore default collection location
    /// </summary>
    /// <param name="collectionId">Collection name</param>
    /// <param name="documentId">Document name</param>
    /// <returns>Operation Status {Whether the operation was sucessful or not}</returns>
    public async Task<bool> CreateCollection(string collectionId, string documentId)
    {   

        bool result = false;
        string apiUrl = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/{collectionId}";

        if (string.IsNullOrEmpty(documentId))
        {
            return result;
        }
        else 
        {
            apiUrl += $"/{documentId}";
        }


        UnityWebRequest req = new UnityWebRequest(apiUrl, "PATCH");
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        var tcs = new TaskCompletionSource<UnityWebRequest>();

        req.SendWebRequest().completed += operation => tcs.TrySetResult(req);

        await tcs.Task;

        if (req.result == UnityWebRequest.Result.Success)
        {
           
            result = true;
        }

        return result;
    }

    /// <summary>
    /// It is used to get Fields under a specific Document
    /// </summary>
    /// <param name="collectionId">Collection Name</param>
    /// <param name="documentId">Document Name</param>
    /// <returns>
    /// 
    /// An object of type FirestoreResponseDocument which contains the values of those fields
    /// 
    /// </returns>

    public async Task<FirestoreResponseDocument> GetCollectionFields(string collectionId, string documentId)
    {

        FirestoreResponseDocument doc = null;
        string apiUrl = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/{collectionId}";
        if (string.IsNullOrEmpty(documentId))
        {
            return doc;
        }
        else
        {
            apiUrl += $"/{documentId}";
        }


        UnityWebRequest req = new UnityWebRequest(apiUrl, "GET");

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        var tcs = new TaskCompletionSource<UnityWebRequest>();

        req.SendWebRequest().completed += operation => tcs.TrySetResult(req);

        await tcs.Task;

        if (req.result == UnityWebRequest.Result.Success)
        {

            doc = JsonUtility.FromJson<FirestoreResponseDocument>(req.downloadHandler.text);
            
        }
        else
        {
            
        }

        return doc;

    }
    /// <summary>
    /// This method is used to Create Fields inside a specific document 
    /// </summary>
    /// <param name="collectionId">Collection name</param>
    /// <param name="documentId">Document name</param>
    /// <returns>Operation Status {Whether the operation was sucessful or not}</returns>
    public async Task<bool> CreateFields(string collectionId, string documentId, FirestoreUploadDocument doc)
    {   


        bool result = false;
        string apiUrl = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/{collectionId}";

        if (string.IsNullOrEmpty(documentId))
        {
            return result;
        }
        else 
        {
            apiUrl += $"/{documentId}";
        }

        if (doc == null)
        {
            return result;
        }

        UnityWebRequest req = new UnityWebRequest(apiUrl, "PATCH");

        string payload = JsonUtility.ToJson(doc);

        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(payload));
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        var tcs = new TaskCompletionSource<UnityWebRequest>();

        req.SendWebRequest().completed += operation => tcs.TrySetResult(req);

        await tcs.Task;

        if (req.result == UnityWebRequest.Result.Success)
        {
            result = true;
        }
        else
        {
            Debug.Log(req.downloadHandler.text);    
        }


        return result;
    }

    /// <summary>
    /// This method is used to update the specific field of a document with the new value
    /// </summary>
    /// <param name="collectionId">Collection name</param>
    /// <param name="documentId">Document name</param>
    /// <param name="fieldName">Field required to be updated</param>
    /// <param name="newfieldValue">Field required to be updated with</param>
    /// <returns>Operation Status {Whether the operation was sucessful or not}</returns>


    public async Task<bool> UpdateField(string collectionId, string documentId, string fieldName, string newfieldValue)
    {

        FirestoreResponseDocument respDoc = await this.GetCollectionFields(collectionId, documentId);

        if (respDoc == null)
        {
            Debug.Log("Yes2");
            return false;
        }

        FirestoreUploadDocument uplDoc = new FirestoreUploadDocument();

        uplDoc.fields = respDoc.fields;

        switch (fieldName)
        {
            case "Health":
                uplDoc.fields.Health.stringValue = newfieldValue; 
                break;
            case "Attack":
                uplDoc.fields.Attack.stringValue = newfieldValue;
                break;
            default:
                return false;
                
        }

        bool res = await this.CreateFields(collectionId, documentId, uplDoc);

        return res;

    }
    /// <summary>
    /// It is used to delete a specific document with all its fields.
    /// </summary>
    /// <param name="collectionId">Collection name</param>
    /// <param name="documentId">Document name</param>
    /// <returns>Operation Status {Whether the operation was sucessful or not}</returns>
    public async Task<bool> DeleteDocument(string collectionId , string documentId)
    {

        bool res = false;
        string apiUrl = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/{collectionId}";

        if (string.IsNullOrEmpty(documentId))
        {
            return res;
        }
        else
        {
            apiUrl += $"/{documentId}";
        }

        UnityWebRequest req = new UnityWebRequest(apiUrl, "DELETE");

        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");
        var tcs = new TaskCompletionSource<UnityWebRequest>();

        req.SendWebRequest().completed += operation => tcs.TrySetResult(req);

        await tcs.Task;

        if (req.result == UnityWebRequest.Result.Success)
        {
            res = true;
        }


        return res;
    }
    public async Task<Dictionary<string, Fields>> RetrieveDocument(string collectionId)
    {
        
        Dictionary<string, Fields> documentDictionary = new Dictionary<string, Fields>();

        
        string apiUrl = $"https://firestore.googleapis.com/v1/projects/{projectId}/databases/(default)/documents/{collectionId}";

        
        UnityWebRequest req = new UnityWebRequest(apiUrl, "GET");

      
        req.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        req.SetRequestHeader("Content-Type", "application/json");

       
        var tcs = new TaskCompletionSource<UnityWebRequest>();

        
        req.SendWebRequest().completed += operation => tcs.TrySetResult(req);

     
        await tcs.Task;

       
        if (req.result == UnityWebRequest.Result.Success)
        {
            
            string responseText = req.downloadHandler.text;

            
            FirestoreResponseDocumentWrapper Wrapper = JsonUtility.FromJson<FirestoreResponseDocumentWrapper>(responseText);

            
            if (Wrapper.documents != null && Wrapper.documents.Length > 0)
            {
              
                foreach (var document in Wrapper.documents)
                {
                   
                    if (document.fields != null)
                    {
                       
                        documentDictionary.Add(document.name, document.fields);
                    }
                }
            }
        }
        else
        {
          
            Debug.LogError($"Failed to retrieve documents. Error: {req.error}");
        }

       
        return documentDictionary;
    }

  
    [System.Serializable]
    public class FirestoreResponseDocumentWrapper
    {
        public FirestoreResponseDocument[] documents;
    }



}
