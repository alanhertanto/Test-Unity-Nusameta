using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    [Header("Hasil dari Config.cfg")]
    [Tooltip("File berada di Streaming Assets")]
    public List<string> CachedConfig = new List<string>();

    private string _path;
    private void Awake()
    {
        _path = Path.Combine(Application.streamingAssetsPath,"Config.cfg");
        Read();
    }
    private void Read()
    {
        //Search the Config.cfg file on StreamingAssets
        if (!File.Exists(_path))
        {
            Debug.Log("Config file not found!");
            return;
        }
        //Read the Lines from Config.cfg
        var lines = File.ReadLines(_path).ToList();
        lines.ForEach(x =>
        {
            //Split the lines List after ": " to the List
            CachedConfig.Add(x.Split(new string[]{": "},StringSplitOptions.None)[1]);
        });
    }
}
