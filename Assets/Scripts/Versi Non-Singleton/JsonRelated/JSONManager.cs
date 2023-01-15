using System.Collections;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

public class JSONManager : MonoBehaviour
{
    
    [Header("Config Manager Reference")]
    public ConfigManager ConfigManager;
    
    [Header("Data Get From API")]
    public APIWearable APIWearable;

    //Private URL of The API From Config
    private string APIURL;
    
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        //Get API URL From Config.cfg File Using ConfigManager (Get When Script Called on Awake()
        APIURL = ConfigManager.CachedConfig[0] + ConfigManager.CachedConfig[1];
        StartCoroutine(GetAPIWearable(APIURL));
    }
    public IEnumerator GetAPIWearable(string url)
    {
        //Get API using UnityWebRequest.Get()
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            var webRequest = www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                string errlog = "Error connecting to the server. "+ $"{www.responseCode}";
                Debug.Log (errlog);
            }
            else
            {
                //Wait Until WebRequest Done Get the Data
                yield return new WaitUntil(()=>webRequest.isDone);
                if (www.downloadHandler.text != null)
                {
                    //Parsing the Data from JSON to Serializable
                    JSONNode parsedJSON = JSON.Parse(www.downloadHandler.text);
                    var dataParse = JSONArray.Parse(parsedJSON);
                    APIWearable.statusCode = parsedJSON["statusCode"];
                    APIWearable.message = parsedJSON["message"];
                    APIWearable.metadata = parsedJSON["metadata"];
                    APIWearable.service = parsedJSON["service"];
                    APIWearable.timestamp = parsedJSON["timestamp"];
                    for (int i = 0; i < parsedJSON["data"].Count; i++)
                    {
                        //Loop the datas and file meta based on data field on API Data
                        APIData apiData = new APIData();
                        FileMeta fileMeta = new FileMeta();
                        apiData.id = parsedJSON["data"][i]["id"];
                        apiData.amount = parsedJSON["data"][i]["amount"];
                        apiData.version = parsedJSON["data"][i]["version"];
                        apiData.createdAt = parsedJSON["data"][i]["createdAt"];
                        apiData.tokenId = parsedJSON["data"][i]["tokenId"];
                        apiData.trxHash = parsedJSON["data"][i]["trxHash"];
                        apiData.updatedAt = parsedJSON["data"][i]["updatedAt"];
                        apiData.wearableId = parsedJSON["data"][i]["wearableId"];
                        apiData.wearableName = parsedJSON["data"][i]["wearableName"];
                        if (parsedJSON["data"][i]["fileMeta"] != null)
                        {
                            fileMeta.configUrl = parsedJSON["data"][i]["fileMeta"]["configUrl"];
                            fileMeta.manifestUrl = parsedJSON["data"][i]["fileMeta"]["manifestUrl"];
                            fileMeta.assetBundleUrl = parsedJSON["data"][i]["fileMeta"]["assetBundleUrl"];
                            apiData.fileMeta = fileMeta;
                        }
                        else
                        {
                            apiData.fileMeta = new FileMeta();
                        }

                        APIWearable.datas.Add(apiData);
                    }
                }
            }
        }
    }
}
