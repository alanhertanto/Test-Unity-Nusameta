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

public class APIData
{
    public int id;
    public int wearableId;
    public string wearableName;
    public int tokenId;
    public string trxHash;
    public string fileMeta;
    public int amount;
    public string version;
    public string createdAt;
    public string updatedAt;
}