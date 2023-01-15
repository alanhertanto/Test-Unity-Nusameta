using UnityEngine;

public class GridSpawnManager : MonoBehaviour
{
    public static GridSpawnManager Instance;
    
    public Vector2 GridSize;

    public Vector2 CellGap;

    public Vector2Int currentCell;

    private void Awake()
    {
        Instance = this;
    }

    public GameObject SpawningObject(GameObject objectToSpawn, Transform placeHolder)
    {
        Quaternion randomizedDirection = Quaternion.Euler(new Vector3(0,Random.Range(0,360),0));
        GameObject spawned = Instantiate(objectToSpawn, placeHolder.position,randomizedDirection);
        if (currentCell.x < GridSize.x-1)
        {
            spawned.transform.parent = placeHolder;
            spawned.transform.position = new Vector3(currentCell.x, placeHolder.position.y,
                currentCell.y);
            currentCell.x += 1;
        }
        else
        {
            spawned.transform.parent = placeHolder;
            spawned.transform.position = new Vector3(currentCell.x, placeHolder.position.y,
                currentCell.y);
            currentCell.x = 0;
            currentCell.y += 1;
        }
        return spawned;
        
    }
}