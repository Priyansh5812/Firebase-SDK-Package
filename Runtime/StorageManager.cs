using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
public class StorageManager
{
    [SerializeField] private string bucketUrl;

    public StorageManager(string bucketUrl) 
    {
        this.bucketUrl = bucketUrl;
    }

    public async Task<bool> UploadImage(string filePath , string uploadUrl)
    {
        //Required Checks
        bool result = false;
            
            if (this.bucketUrl == null)
            {
                return result;
            }

            if (uploadUrl == null)
            {
                return result;
            }
            else
            {
                ExtractUploadUrl(ref uploadUrl);
            }

            if (filePath == null)
            {
                return result;
            }


            string url = $"{this.bucketUrl}{uploadUrl}";// Setting up the url
            //Debug.Log(url);
            byte[] byteData = await GetByte(filePath);// Getting the byte array of the file

            //Setting up the required web request.

            UnityWebRequest req = new UnityWebRequest(url, "POST");
            req.uploadHandler = new UploadHandlerRaw(byteData); ;
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "image/jpeg"); // Careful here its different for different types of upload
            ///
            /// Uncomment the Authorization Header and set the required ID TOKEN when:
            /// 1) You have an Id token to get the Authorization
            /// 2) When you have the rules set as "read : if request.auth != null;" Consider the documentation : "https://firebase.google.com/docs/rules/get-started" 
            /// req.SetRequestHeader("Authorization" , <ID_Token>);
            ///
            var tcs = new TaskCompletionSource<UnityWebRequest>();

            req.SendWebRequest().completed += operation => tcs.SetResult(req);

            await tcs.Task;

            if (req.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Upload complete!");
                result = true;
            }
        



        return result;


    }

    public async Task<bool> UploadAudio(string filePath, string uploadUrl)
    {   
        //Required Checks
        bool result = false;
        if (this.bucketUrl == null)
        {
            return result;
        }

        if (uploadUrl == null)
        {
            return result;
        }
        else
        {
            ExtractUploadUrl(ref uploadUrl);
        }

        if (filePath == null)
        {
            return result;
        }


        string url = $"{this.bucketUrl}{uploadUrl}";// Setting up the url
        byte[] byteData = await GetByte(filePath);// Getting the byte array of the file
        UnityWebRequest 
            
        //Setting up the required web request.

        req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(byteData); ;
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "audio/mpeg");// Careful here its different for different types of upload
        ///
        ///Uncomment the Authorization Header and set the required ID TOKEN when:
        /// 1) You have an Id token to get the Authorization
        /// 2) When you have the rules set as "read : if request.auth != null;" Consider the documentation : "https://firebase.google.com/docs/rules/get-started" 
        /// req.SetRequestHeader("Authorization" , <ID_Token>);
        ///
        var tcs = new TaskCompletionSource<UnityWebRequest>();

        req.SendWebRequest().completed += operation => tcs.SetResult(req);

        await tcs.Task;

        if (req.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Upload complete!");
            result = true;
        }


        return result;
    }


    #region Primitives

    /// <summary>
    /// This method is responsible for converting the upload url to the valid-network upload path.
    /// </summary>
    /// <param name="uploadUrl">Parameter where the user's desired upload url gets passed
    /// uploadUrl {Parameter}: "Images/MyImages/image.jpg"
    /// Expected return value : "Images%2FMyImages%2Fimage.jpg"
    /// </param>
    private void ExtractUploadUrl(ref string uploadUrl) 
    {
        string str = uploadUrl;
        uploadUrl = string.Empty;
        foreach (var i in str)
        {
            uploadUrl += (i == '/') ? ("%2F") : (i);
        }
    }
    /// <summary>
    /// This method just returns a byte array of the file using its file path
    /// 
    /// </summary>
    /// <param name="filePath">Local machine file Location</param>
    /// <returns>A Byte array</returns>
    private async Task<byte[]> GetByte(string filePath)
    {
        return await System.IO.File.ReadAllBytesAsync(filePath);
    }
    #endregion Primitives

}
