using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Realtime Database 
/// </summary>

public class DatabaseManager<T>
{
    private string databaseUrl;
    public DatabaseManager(string databaseUrl)
    {
        this.databaseUrl = databaseUrl;
    }


    /// <summary>
    /// For updating an existing Node.
    /// Will return false if the node does not exist.
    /// 
    /// </summary>
    /// <param name="nodePath">Path from the root node</param>
    /// <param name="entity">Entity to be updated with</param>
    /// <returns>Operation Status</returns>
    public async Task<bool> UpdateNode(string nodePath, T entity)
    {
        if (entity == null)
        {
            return false;
        }


        bool res = false;
        T temp = await GetNodeData(nodePath);
        if (temp != null)
        {
            res = true;
            await SetNodeData(entity, nodePath);
        }     

        return res;

    } //UPDATE

    /// <summary>
    /// For creating a new Node.
    /// Will return false if node already exists
    /// </summary>
    /// <param name="nodePath">Path from the root node</param>
    /// <param name="entity">Entity to be updated with</param>
    /// <returns>Operation Status</returns>
    public async Task<bool> CreateNode(string nodePath, T entity)
    {
        if (entity == null)
        {
            return false;
        }

        if (nodePath == null || nodePath == "null")
        {
            return false;
        }

        bool res = false;
        T temp = await GetNodeData(nodePath);

        if (temp == null)
        {
            res = await SetNodeData(entity, nodePath);
        }

        return res;
    } // CREATE


    #region Primitives

    /// <summary>
    /// Get the Node from the specified Node path
    /// </summary>
    /// <param name="nodePath">Node path in the database</param>
    /// <returns>
    /// returns an object of Generic type which will be holding the data 
    /// </returns>
    public async Task<T> GetNodeData(string nodePath) //GET
    {
        T entity = default(T);
        UnityWebRequest req = UnityWebRequest.Get(this.databaseUrl + nodePath + ".json");

        var tcs = new TaskCompletionSource<UnityWebRequest>();

        req.SendWebRequest().completed += operation => tcs.TrySetResult(req);

        await tcs.Task;

        if (req.result == UnityWebRequest.Result.Success && req.downloadHandler.text != "null")
        {
            entity = JsonUtility.FromJson<T>(req.downloadHandler.text);
        }
        return entity;

    }

    /// <summary>
    /// Set/Put/Overwrite the data of the existing Node via specified file path and entity 
    /// </summary>
    /// <param name="entity">Object which contains the new/updated values.</param>
    /// <param name="nodePath">Node path in the database</param>
    /// <returns>Operation Status</returns>
    private async Task<bool> SetNodeData(T entity, string nodePath) //PUT
    {
        bool result = false;
        string json = JsonUtility.ToJson(entity);
        UnityWebRequest req = UnityWebRequest.Put(this.databaseUrl + nodePath + ".json", json);

        var tcs = new TaskCompletionSource<UnityWebRequest>();

        req.SendWebRequest().completed += operation => tcs.TrySetResult(req);

        await tcs.Task;

        if (req.result == UnityWebRequest.Result.Success)
        {
            result = true;
        }
        else
        {
            result = false;
        }
        return result;
    }
    /// <summary>
    /// Delete/Remove the node from the database.
    /// It will delete node specified if the node exists otherwise it won't {will not throw an error either}
    /// </summary>
    /// <param name="nodePath">Node path in the database</param>
    /// <returns>Operation Status</returns>
    public async Task<bool> DeleteNode(string nodePath) //DELETE
    {
        bool result = false;
        //string json = JsonUtility.ToJson(entity);
        UnityWebRequest req = UnityWebRequest.Delete(this.databaseUrl +nodePath + ".json");

        var tcs = new TaskCompletionSource<UnityWebRequest>();

        req.SendWebRequest().completed += operation => tcs.TrySetResult(req);

        await tcs.Task;

        if (req.result == UnityWebRequest.Result.Success)
        {
            result = true;
        }
        else
        {
            result = false;
        }
        return result;
    }


    #endregion Primitives
}
