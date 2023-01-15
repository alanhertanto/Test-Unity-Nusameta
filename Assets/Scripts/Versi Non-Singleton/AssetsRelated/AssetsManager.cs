using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GLTFast;
using GLTFast.Loading;
using TMPro;
using UnityEngine.UI;
using Random = System.Random;

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

    private List<GameObject> SpawnedClones = new List<GameObject>();

    private List<AnimationClip> animations = new List<AnimationClip>();

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
        //find if the GO that from GLTFast has been instantiated      
        var gameObjectSpawned = OriSpawnedGOList.Find(x => x.name == objectName);
        CloseListObjectFromAPI();
        ButtonAnimator.Play("OpenLoading");
        LoadingCanvasGroup.gameObject.SetActive(true);
        LoadingCanvasGroup.alpha = 1;
        LoadingCanvasGroup.interactable = true;
        if (gameObjectSpawned != null)
        {
            //Set the Original Instantiated from GLTFast GO to Active instead of destroy it
            GameObject spawned = OriSpawnedGOList[OriSpawnedGOList.IndexOf(gameObjectSpawned)];
            spawned.SetActive(true);
            DestroySpawnedGO();
            StartCoroutine(CloneGameObjectTo2x4(spawned));
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
            StartCoroutine(CloneGameObjectTo2x4(gameObjectSpawned));
            ButtonAnimator.Play("CloseLoading");
         }
        else {
            Debug.LogError("Loading glTF failed!");
        }
    }
    
    private IEnumerator CloneGameObjectTo2x4(GameObject gameObjectToClone)
    {
        foreach (var animationClip in GetAnimationFromObject(gameObjectToClone))
        {
            yield return new WaitForSeconds(0.25f);
            //Spawn The Object
            var spawnedGOClone = GridManager.SpawningObject(gameObjectToClone,GOPlaceholder);
            Animation spawnedAnimation = spawnedGOClone.GetComponent<Animation>();
            spawnedAnimation.clip = animationClip; 
            spawnedAnimation.Play();
            //Instantiate The SpawnerVFX and add it to the child of the clone
            Instantiate(SpawnerVFXPrefab, spawnedGOClone.transform, false);
            SpawnedClones.Add(spawnedGOClone);
        }
        gameObjectToClone.SetActive(false);
    }
    private void DestroySpawnedGO()
    {
        foreach (var spawnedClone in SpawnedClones)
        {
            DestroyImmediate(spawnedClone);
        }
        SpawnedClones.Clear();
    }
    
    private List<AnimationClip> GetAnimationFromObject(GameObject spawned)
    {
        foreach (AnimationState animation in spawned.GetComponent<Animation>())
        {
            animations.Add(animation.clip);            
        }
        return animations;
    }
    private bool CheckURLValid(string url)
    {
        if (string.IsNullOrEmpty(url)||!url.Contains(".gltf"))
            return false;
        
        return true;
    }
    
}
