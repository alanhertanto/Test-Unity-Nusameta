using System.Collections.Generic;
using UnityEngine;

public class GOHandler : MonoBehaviour
{
    public List<AnimationClip> ThisAnimationClips = new List<AnimationClip>();
    private void Start()
    {
        GridSpawnManager.OnObjectInstantiated += HandleObjectInstantiation;   
    }

    public void HandleObjectInstantiation(GameObject thisObject, GameObject vfxPrefab, AnimationClip animationClip)
    {
        Instantiate(vfxPrefab, thisObject.transform, false);
        Animation animation = thisObject.GetComponent<Animation>();
        animation.clip = animationClip;
        animation.Play();
    }

    private void OnDisable()
    {
        GridSpawnManager.OnObjectInstantiated -= HandleObjectInstantiation;
    }
}
