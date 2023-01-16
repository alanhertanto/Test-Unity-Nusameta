using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using TMPro;
using UnityEngine.UI;

public class AssetsManager : MonoBehaviour
{
    [Header("Referenced JSON Manager")]
    public JSONManager JsonManager;
    [Header("Referenced Grid Manager")]
    public GridSpawnManager GridManager;
    [Space]

    [Header("Base URL for Getting The AssetBundleURL")]
    [Space]
    public string BaseURL;

    [Header("Prefab Reference")]
    public GameObject ButtonPrefab;
    public GameObject SpawnerVFXPrefab;

    [Header("Button Parent when Instantiated")]
    public Transform ButtonPlaceholder;
    
    [Header("Clone and Original Object Parent when Instantiated")]
    public Transform GOPlaceholder;

    [Header("Canvas Animator For Animation")]
    public Animator ButtonAnimator;
    
    [Header("Canvas Group for Object List Animation")]
    public CanvasGroup LoadingCanvasGroup;
    
    [Header("Original GameObject List from API")]
    public List<GameObject> OriSpawnedGOList = new List<GameObject>();

    private List<Button> BGameObjectToSpawn = new List<Button>();
    
    public void GetListOfObjectFromAPI()
    {
        ButtonAnimator.Play("OpenSpawnList");
        //Instantiated Button on the List Scrollview based on the API Data field
        foreach (var parsedData in JsonManager.APIWearable.datas)
        {
            Button spawnerButton = Instantiate(ButtonPrefab, ButtonPlaceholder).GetComponent<Button>();
            spawnerButton.GetComponentInChildren<TMP_Text>().text = parsedData.wearableName;
            spawnerButton.gameObject.name = parsedData.wearableName;

            //Adding On Click Event for Instantiated Button
            spawnerButton.onClick.AddListener(() =>
            {
                if (!CheckURLValid(parsedData.fileMeta.assetBundleUrl))
                    return;
                GLTFSpawn(parsedData.fileMeta.assetBundleUrl,parsedData.wearableName);
            });
            
            //Adding the Instantiated Button for Clear and Destroy Purpose or Other Purpose
            BGameObjectToSpawn.Add(spawnerButton);
        }
    }
    public void CloseListObjectFromAPI()
    {
        foreach (var spawnedButton in BGameObjectToSpawn)
        {
            DestroyImmediate(spawnedButton.gameObject);
        }
        BGameObjectToSpawn.Clear();
        ButtonAnimator.Play("CloseSpawnList");
    }
    
    //Make the Function Asynchronous
    private async void GLTFSpawn(string url,string objectName)
    {
        CloseListObjectFromAPI();
        ButtonAnimator.Play("OpenLoading");
        LoadingCanvasGroup.alpha = 1;
        LoadingCanvasGroup.interactable = true;
        
        //find if the GO that from GLTFast has been instantiated      
        var gameObjectSpawned = OriSpawnedGOList.Find(x => x.name == objectName);
        if (gameObjectSpawned != null)
        {
            GameObject spawned = OriSpawnedGOList[OriSpawnedGOList.IndexOf(gameObjectSpawned)];
            //Set the Original Instantiated from GLTFast GO to Active instead of destroy it
            spawned.SetActive(true);
            DestroySpawnedGO();
            GridManager.BeginSpawning(SpawnerVFXPrefab,GOPlaceholder,gameObjectSpawned);
            ButtonAnimator.Play("CloseLoading");
            return;
        }

        //If the GO hasn't been instantiated, then call GLTFImport function
        var gltf = new GLTFast.GltfImport();
        // Create a settings object and configure it accordingly
        var settings = new ImportSettings {
            GenerateMipMaps = true,
            AnisotropicFilterLevel = 3,
            NodeNameMethod = NameImportMethod.OriginalUnique
        };
        // Load the glTF and pass along the settings
        var success = await gltf.Load(BaseURL + url, settings);
        if (success)
        {
            //Wait for The Instantiate Process Done then adding it to the Original GO List
            gameObjectSpawned = new GameObject(objectName);
            await gltf.InstantiateSceneAsync(gameObjectSpawned.transform);
            gameObjectSpawned.transform.parent = GOPlaceholder;
            gameObjectSpawned.GetComponent<Animation>().Play();
            OriSpawnedGOList.Add(gameObjectSpawned);
            DestroySpawnedGO();
            GridManager.BeginSpawning(SpawnerVFXPrefab, GOPlaceholder, gameObjectSpawned);
            ButtonAnimator.Play("CloseLoading");
        }
        else {
            Debug.LogError("Loading glTF failed!");
        }
    }
    
    private void DestroySpawnedGO()
    {
        foreach (var spawnedClone in GridManager.Clones)
        {
            DestroyImmediate(spawnedClone);
        }
        GridManager.Clones.Clear();
    }
    
    private bool CheckURLValid(string url)
    {
        if (string.IsNullOrEmpty(url)||!url.Contains(".gltf"))
            return false;
        
        return true;
    }
    
}
