using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
//using System.Diagnostics;

public class APIHandler : MonoBehaviour
{
    private const string API_URL = "https://api.jsonbin.io/v3/b";
    private const string API_KEY = "$2b$10$N7OfngmWzAbzoEHQGWVbTOaoV.EFbGbqtuxYuFXqO2JOMfNKPk82q";
    private const string BIN_ID_PREF = "userBinId";
    public bool dataSent;
    public string condition;
    private bool already_communicated = false;

    private void Start()
    {
        if (!PlayerPrefs.HasKey(BIN_ID_PREF))
        {
            StartCoroutine(CreateNewBinForUser());
        }
    }

    private void Update()
    {
        dataSent = PlayerPrefs.GetInt("dataSent", 0) == 1;
    }

    public IEnumerator SetCondition()
    {
        // Fetch the most recent bin_id
        string url = "https://api.jsonbin.io/v3/c/uncategorized/bins/";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("X-Master-key", API_KEY);
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
            yield break;
        }

        JSONNode jsonNode = JSON.Parse(request.downloadHandler.text);
        string mostRecentBinId = jsonNode[0]["record"];

        // Fetch the bin using the most recent bin_id
        string binUrl = $"https://api.jsonbin.io/v3/b/{mostRecentBinId}";
        UnityWebRequest binRequest = UnityWebRequest.Get(binUrl);
        binRequest.SetRequestHeader("X-Master-key", API_KEY);
        binRequest.SetRequestHeader("Content-Type", "application/json");
        yield return binRequest.SendWebRequest();
        if (binRequest.result == UnityWebRequest.Result.ConnectionError || binRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(binRequest.error);
            yield break;
        }

        JSONNode binJson = JSON.Parse(binRequest.downloadHandler.text);
        JSONNode liveData = binJson["record"]["LiveData"];

        if (liveData == null)
        {
            // LiveData field is not present
            yield break;
        }

        string action = liveData[0]["action"];
        if (action == null)
        {
            // action field is not present in the first element
            yield break;
        }

        if (action.EndsWith("Condition set to With"))
        {
            Debug.Log("Last condition: With");
            PlayerPrefs.SetString("Condition", "Without");
            condition = "Without";
        }
        else if (action.EndsWith("Condition set to Without"))
        {
            Debug.Log("Last condition: Without");
            PlayerPrefs.SetString("Condition", "With");
            condition = "With";
        }
    }

    public IEnumerator CreateNewBinForUser()
    {
        yield return SetCondition();

        UnityWebRequest request = new UnityWebRequest(API_URL, "POST");

        // Construct initial data
        InitialData data = new InitialData();
        data.LiveData = new List<LogData>
        {
            new LogData
            {
                time = System.DateTime.UtcNow.ToString("o"), // ISO 8601 format
                action = "New user created; Condition set to " + condition
            }
        };
        string jsonData = JsonUtility.ToJson(data);

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("X-Master-key", API_KEY);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            var jsonResponse = JsonUtility.FromJson<ResponseFormat>(response);
            PlayerPrefs.SetString(BIN_ID_PREF, jsonResponse.metadata.id);
            Debug.Log("Stored userBinId: " + jsonResponse.metadata.id);
        }
    }

    [System.Serializable]
    public class InitialData
    {
        public List<LogData> LiveData;
    }

    [System.Serializable]
    public class LogData
    {
        public string time;
        public string action;
    }


    public void SendDataToBin(string escapedData)
    {
        if (PlayerPrefs.HasKey(BIN_ID_PREF))
        {
            if(dataSent)
            {
                if(! already_communicated)
                {
                    Debug.Log("Data already sent and is valid, skipping further sending.");
                    already_communicated = true;
                }
            } else {
                StopAllCoroutines();
                string binId = PlayerPrefs.GetString(BIN_ID_PREF);
                StartCoroutine(UpdateUserBin(binId, escapedData, true));
            }
        }
        else
        {
            Debug.LogError("Bin ID not found in PlayerPrefs!");
        }
    }

    public IEnumerator UpdateUserBin(string binId, string newData, bool fullData = false)
    {
        if(dataSent)
        {
            if(! already_communicated)
            {
                Debug.Log("Data already sent and is valid, skipping further sending.");
                already_communicated = true;
            }
            yield break;
        }

        // First, retrieve the current data from the bin
        UnityWebRequest getRequest = UnityWebRequest.Get(API_URL + "/" + binId);
        getRequest.SetRequestHeader("X-Master-key", API_KEY);
        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(getRequest.error);
            yield break;
        }

        string currentDataJson = getRequest.downloadHandler.text;
        var currentData = JSON.Parse(currentDataJson);  // Using SimpleJSON to parse

        // Append the new data to the existing data
        // Assuming currentData["record"]["LiveData"] is an array
        if(fullData)
        {
            currentData["record"]["Data"] = JSON.Parse(newData); // Overwrite Data
        } else {
            currentData["record"]["LiveData"][-1] = JSON.Parse(newData);  // Append to LiveData array
        }

        // Now, send the updated (combined) data back to the server
        UnityWebRequest putRequest = new UnityWebRequest(API_URL + "/" + binId, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(currentData["record"].ToString());
        putRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        putRequest.downloadHandler = new DownloadHandlerBuffer();
        putRequest.SetRequestHeader("Content-Type", "application/json");
        putRequest.SetRequestHeader("X-Master-key", API_KEY);

        yield return putRequest.SendWebRequest();

        if (putRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(putRequest.error);
        }
        else
        {
            Debug.Log("Data successfully sent to BIN with ID: " + binId);

            if(fullData && ValidateData(newData))
            {
                PlayerPrefs.SetInt("dataSent", 1); // Full data sent and valid
                Debug.Log("Data saved, completely valid.");
            } else if(fullData){
                Debug.Log("Data not fully valid.");
            }
        }

        getRequest.Dispose();
        putRequest.Dispose();
    }

    private bool ValidateData(string jsonData)
    {
        var data = JSON.Parse(jsonData);

        if (data.HasKey("Level1") && data.HasKey("Level2") && data.HasKey("FormInfo"))
        {
            var level1 = data["Level1"].AsArray;
            var level2 = data["Level2"].AsArray;
            var formInfo = data["FormInfo"].AsObject;

            if (level1 != null && level1.Count > 0 && level2 != null && level2.Count > 0)
            {
                string why = formInfo["Why"].Value;
                string bugs = formInfo["Bugs"].Value;

                if (!string.IsNullOrEmpty(why.Trim()) && !string.IsNullOrEmpty(bugs.Trim()))
                {
                    return true;
                }
            }
        }

        return false;
    }
}


[System.Serializable]
public class ResponseFormat
{
    public Metadata metadata;
}

[System.Serializable]
public class Metadata
{
    public string id;
}
