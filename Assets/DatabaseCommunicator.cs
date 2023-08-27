using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

 /*
    List of functions that are passed:
    AIterminated()
    AIcontinue()
    ProgressBarExpired()
    Win()
    GameOver()
    NextLevel()
    Restart()
*/

public class DatabaseCommunicator : MonoBehaviour
{

    [System.Serializable]
    public class ActionLog
    {
        public string Time;
        public string Action;
    }

    [System.Serializable]
    public class Form
    {
        public string Why;
        public string Bugs;
        public int MOS;
        public bool LowAttention;
    }

    [System.Serializable]
    public class GameData
    {
        public bool AcceptedParticipation;
        public List<ActionLog> Level1;
        public List<ActionLog> Level2;
        public Form FormInfo;

        public GameData()
        {
            AcceptedParticipation = false;
            Level1 = new List<ActionLog>();
            Level2 = new List<ActionLog>();
        }
    }

    private GameData gameData = new GameData();
    public bool send_live_data = false;

    private static DatabaseCommunicator instance;
    void Awake()
    {
        // MANAGE THE DONTDESTROYONLOAD THING
        if (instance == null)
        {
            instance = this;
            gameObject.tag = "Database";
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {

    }

    private void UpdateLog(string action)
    {
        string time = System.DateTime.Now.ToString("o");
        ActionLog actionLog = new ActionLog
        {
            Time = time,
            Action = action
        };

        Scene scene = SceneManager.GetActiveScene();
        string levelKey = (scene.name == "Scene1") ? "Level1" : "Level2";
        
        if (levelKey == "Level1")
        {
            gameData.Level1.Add(actionLog);
        }
        else
        {
            gameData.Level2.Add(actionLog);
        }
    }

    public string GetLogJSON()
    {
        // Serialize the game data to a JSON string
        return JsonUtility.ToJson(gameData);
    }

    public void Accepted()
    {
        Debug.Log("Form accepted");
        gameData.AcceptedParticipation = true;
    }

    public void SendLiveData(string action)
    {
        UpdateLog(action);
        //Debug.Log(GetLogJSON());

        var function = action + "()";

        if (send_live_data)
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
            Application.ExternalEval(function);
            #endif
        }

        //Debug.Log("Sending data from Unity: " + function);
    }

    public void SendFullData()
    {
        string logJSON = GetLogJSON();

        //Debug.Log("Sending data from Unity: " + "FinalData()");
        //Debug.Log(logJSON);

        string escapedData = logJSON;
        string call = "FinalData('" + escapedData + "')";

        #if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval(call);
        #endif        
    }

    public void SendForm(string why, string bugs, int mos, bool low_attention)
    {
        // Check if 'why' is null, empty, or consists only of white-space characters
        if (string.IsNullOrWhiteSpace(why))
        {
            why = "(No comment)";
        }
        // Check if 'bugs' is null, empty, or consists only of white-space characters
        if (string.IsNullOrWhiteSpace(bugs))
        {
            bugs = "(No comment)";
        }

        string escapedWhy = EscapeString(why);
        string escapedBugs = EscapeString(bugs);

        gameData.FormInfo = new Form
        {
            Why = escapedWhy,
            Bugs = escapedBugs,
            MOS = mos,
            LowAttention = low_attention
        };

        SendFullData();
    }

    private string EscapeString(string input)
    {
        return input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("\r", "\\r");
    }
}
