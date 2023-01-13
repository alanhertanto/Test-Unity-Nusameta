using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONManager : MonoBehaviour
{
    public JSONManager Instance;
    public APIWearable APIWearable;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        
    }
}
