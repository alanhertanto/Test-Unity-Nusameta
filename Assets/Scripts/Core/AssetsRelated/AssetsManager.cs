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
    public string BaseURL;

    private List<Button> BGameObjectToSpawn = new List<Button>();

    private List<GameObject> SpawnedClones = new List<GameObject>();
    
    public List<GameObject> OriSpawnedGOList = new List<GameObject>();

    
    public GameObject ButtonPrefab;

    public Transform ButtonPlaceholder;
    
    public Transform GOPlaceholder;
    
    public Animator ButtonAnimator;
    
    public CanvasGroup LoadingCanvasGroup;
    
    public GameObject SpawnerVFXPrefab;
    
    private List<AnimationClip> animations = new List<AnimationClip>();

    public TypewriterEffect LoadingText;

    public void GetListOfObjectFromAPI()
    {
        ButtonAnimator.Play("OpenSpawnList");
        foreach (var parsedData in JSONManager.Instance.APIWearable.datas)
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
            DestroyImmediate(spawnedButton.gameObject);
        }
        BGameObjectToSpawn.Clear();
        ButtonAnimator.Play("CloseSpawnList");
    }
    
    private async void GLTFSpawn(string url,string objectName)
    {
        var gameObjectSpawned = OriSpawnedGOList.Find(x => x.name == objectName);

        CloseListObjectFromAPI();
        ButtonAnimator.Play("OpenLoading");
        LoadingCanvasGroup.gameObject.SetActive(true);
        LoadingCanvasGroup.alpha = 1;
        LoadingCanvasGroup.interactable = true;

        if (gameObjectSpawned != null)
        {
            GameObject spawned = OriSpawnedGOList[OriSpawnedGOList.IndexOf(gameObjectSpawned)];
            spawned.SetActive(true);
            DestroySpawnedGO();
            StartCoroutine(CloneGameObjectTo2x4(spawned));
            ButtonAnimator.Play("CloseLoading");
            return;
        }
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
            var spawnedGOClone = GridSpawnManager.Instance.SpawningObject(gameObjectToClone,GOPlaceholder);
            Animation spawnedAnimation = spawnedGOClone.GetComponent<Animation>();
            spawnedAnimation.clip = animationClip; 
            spawnedAnimation.Play();
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
        animations.Clear();
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
