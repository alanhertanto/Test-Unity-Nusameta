using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
    //Public
    [Header("Hasil dari Config.cfg")]
    [Tooltip("File berada di Streaming Assets")]
    [Space]
    public List<string> CachedConfig = new List<string>();
    //Private
    private string _path;
    private void Awake()
    {
        _path = Path.Combine(Application.streamingAssetsPath,"Config.cfg");
        Read();
    }
    private void Read()
    {
        if (!File.Exists(_path))
        {
            Debug.Log("Config file not found!");
            return;
        }
        var lines = File.ReadLines(_path).ToList();
        lines.ForEach(x =>
        {
            CachedConfig.Add(x.Split(new string[]{": "},StringSplitOptions.None)[1]);
        });
    }
}
