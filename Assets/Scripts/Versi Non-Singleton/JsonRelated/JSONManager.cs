using System.Collections;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;

public class JSONManager : MonoBehaviour
{
    //Public
    public APIWearable APIWearable;
    
    //SerializeField
    [SerializeField]private ConfigManager ConfigManager;

    //Private
    private string APIURL;
    
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        APIURL = ConfigManager.CachedConfig[0] + ConfigManager.CachedConfig[1];
        StartCoroutine(GetAPIWearable(APIURL));
    }
    public IEnumerator GetAPIWearable(string url)
    {
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
                yield return new WaitUntil(()=>webRequest.isDone);
                if (www.downloadHandler.text != null)
                {
                    JSONNode parsedJSON = JSON.Parse(www.downloadHandler.text);
                    var dataParse = JSONArray.Parse(parsedJSON);
                    APIWearable.statusCode = parsedJSON["statusCode"];
                    APIWearable.message = parsedJSON["message"];
                    APIWearable.metadata = parsedJSON["metadata"];
                    APIWearable.service = parsedJSON["service"];
                    APIWearable.timestamp = parsedJSON["timestamp"];
                    for (int i = 0; i < parsedJSON["data"].Count; i++)
                    {
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
