using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSpawnManager : MonoBehaviour
{
    public int gridWidth;
    public int gridHeight;
    public float cellGap;
    public float waitDuration;

    public List<GameObject> Clones = new List<GameObject>();

    private List<AnimationClip> AnimationClips = new List<AnimationClip>();
    
    
    public delegate void ObjectInstantiatedEventHandler(GameObject instantiatedObject, GameObject vfxPrefab, AnimationClip clip);

    public static event ObjectInstantiatedEventHandler OnObjectInstantiated;

    public void BeginSpawning(GameObject vfxPrefab, Transform GOPlaceholder, GameObject gameObjectToClone)
    {
        AnimationClips.Clear();
        foreach (AnimationState animationState in gameObjectToClone.GetComponent<Animation>())
        {
            AnimationClips.Add(animationState.clip);
        }
        StartCoroutine(SpawnObject(vfxPrefab, GOPlaceholder, gameObjectToClone));
    }
    
    private IEnumerator SpawnObject(GameObject vfxPrefab, Transform GOPlaceholder, GameObject gameObjectToClone)
    {
        Clones.Clear();
        for (int currx = 0; currx < gridWidth; currx++)
        {
            for (int currz = 0; currz < gridHeight; currz++)
            {
                Quaternion randomizedDirection = Quaternion.Euler(new Vector3(0,Random.Range(0,360),0));
                Vector3 gridPosition = new Vector3(currx * (cellGap + gameObjectToClone.transform.localScale.x), 0,currz * (cellGap + gameObjectToClone.transform.localScale.z));
                GameObject instantiatedObject = Instantiate(gameObjectToClone, gridPosition,randomizedDirection);
                instantiatedObject.transform.parent = GOPlaceholder;
                Clones.Add(instantiatedObject);
                instantiatedObject.AddComponent<GOHandler>();
                int randomIndex = Random.Range(0, AnimationClips.Count);
                AnimationClip clip = AnimationClips[randomIndex];
                AnimationClips.RemoveAt(randomIndex);
                yield return new WaitForSeconds(waitDuration);
                OnObjectInstantiated?.Invoke(instantiatedObject, vfxPrefab, clip);
            }
        }
        gameObjectToClone.SetActive(false);
    }
}