using System;
using System.Collections.Generic;

[Serializable]
public class APIWearable
{
    public int statusCode;
    public string message;
    public List<APIData> datas;
    public string metadata;
    public string service;
    public string timestamp;
}
[Serializable]
public class APIData
{
    public string id;
    public string wearableId;
    public string wearableName;
    public int tokenId;
    public string trxHash;
    public FileMeta fileMeta;
    public int amount;
    public string version;
    public string createdAt;
    public string updatedAt;
}

[Serializable]
public class FileMeta
{
    public string configUrl;
    public string manifestUrl;
    public string assetBundleUrl;
}