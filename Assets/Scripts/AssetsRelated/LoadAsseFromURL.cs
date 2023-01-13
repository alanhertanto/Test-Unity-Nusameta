using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast;
public class LoadAsseFromURL : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var gltf = gameObject.AddComponent<GLTFast.GltfAsset>();
        gltf.Url = "https://dev-pp-bucket-ap-southeast-3.s3.ap-southeast-3.amazonaws.com/3d_assets/wearable/0618e9bd-c53e-4d14-b492-a13a2f0cc0b1/Android/orang_lari_fix.gltf";       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
