using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GLTFast;
using GLTFast.Loading;
using TMPro;
using UnityEngine.UI;

public class AssetsManager : MonoBehaviour
{
    public JSONManager JsonManager;
    
    public GridSpawnManager GridManager;
    
    public string BaseURL;

    public List<Button> BGameObjectToSpawn;
    
    public GameObject ButtonPrefab;

    public Transform ButtonPlaceholder;
    
    public Transform GOPlaceholder;
    
    public Animator ButtonAnimator;

    public Slider LoadingSlide;

    public CanvasGroup LoadingCanvasGroup;
    
    public void GetListOfObjectFromAPI()
    {
        ButtonAnimator.Play("OpenSpawnList");
        foreach (var parsedData in JsonManager.APIWearable.datas)
        {
            Button spawnerButton = Instantiate(ButtonPrefab, ButtonPlaceholder).GetComponent<Button>();
            spawnerButton.GetComponentInChildren<TMP_Text>().text = parsedData.wearableName;
            spawnerButton.gameObject.name = parsedData.wearableName;
            spawnerButton.onClick.AddListener(() =>
            {
                if (!CheckURLValid(parsedData.fileMeta.assetBundleUrl))
                    return;

                GLTFSpawn(parsedData.fileMeta.assetBundleUrl,parsedData.wearableName);
            });
            BGameObjectToSpawn.Add(spawnerButton);
        }
    }

    public void CloseListObjectFromAPI()
    {
        foreach (var spawnedButton in BGameObjectToSpawn)
        {
            Destroy(spawnedButton.gameObject);
        }
        BGameObjectToSpawn.Clear();
        ButtonAnimator.Play("CloseSpawnList");
    }
    
    public async void GLTFSpawn(string url,string objectName)
    {
        var gltf = new GLTFast.GltfImport();

        // Create a settings object and configure it accordingly
        var settings = new ImportSettings {
            GenerateMipMaps = true,
            AnisotropicFilterLevel = 3,
            NodeNameMethod = NameImportMethod.OriginalUnique
        };

        LoadingCanvasGroup.gameObject.SetActive(true);
        LoadingCanvasGroup.alpha = 1;
        LoadingCanvasGroup.interactable = true;
        ButtonAnimator.Play("OpenLoading");
        
        // Load the glTF and pass along the settings
        var success = await gltf.Load(BaseURL + url, settings);


        
        if (success) {
            var gameObjectSpawned = new GameObject(objectName);
            await gltf.InstantiateMainSceneAsync(gameObjectSpawned.transform);
            gameObjectSpawned.transform.parent = GOPlaceholder;
            gameObjectSpawned.GetComponent<Animation>().Play();
            //GridManager.currentCell.x = 1;
            ButtonAnimator.Play("CloseLoading");
            CloneGameObjectTo2x4(gameObjectSpawned);
        }
        else {
            Debug.LogError("Loading glTF failed!");
        }
    }
    
    public void CloneGameObjectTo2x4(GameObject gameObjectToClone)
    {
        foreach (var animationClip in GetAnimationFromObject(gameObjectToClone))
        {
            // GameObject spawnedGOClone = Instantiate(gameObjectToClone, GOPlaceholder);
            var spawnedGOClone = GridManager.SpawningObject(gameObjectToClone,GOPlaceholder);
            Animation spawnedAnimation = spawnedGOClone.GetComponent<Animation>(); 
            spawnedAnimation.clip = animationClip; 
            spawnedAnimation.Play(); 
        }
        gameObjectToClone.SetActive(false);
    }
    
    

    private List<AnimationClip> GetAnimationFromObject(GameObject spawned)
    {
        List<AnimationClip> animations = new List<AnimationClip>();
        foreach (AnimationState animation in spawned.GetComponent<Animation>())
        {
            animations.Add(animation.clip);            
        }
        //animations.RemoveAt(0);
        return animations;
    }

    private bool CheckURLValid(string url)
    {
        if (string.IsNullOrEmpty(url))
            return false;
        
        return true;
    }
    
}
