using UnityEngine;
using UnityEngine.EventSystems;

public class GridSpawnManager : MonoBehaviour
{
    [Header("Grid Size (Width * Height)")]
    public Vector2 GridSize;

    [Header("Gap Between Cell (Width * Height)")]
    public Vector2 CellGap;

    private Vector2Int currentCell;

    public EventSystem OnSpawning;
    
    public GameObject SpawningObject(GameObject objectToSpawn, Transform placeHolder)
    {
        //Set The Gap to -1, if not reduce it to 1, then the object will actually spawned +2
        CellGap = new Vector2(CellGap.x - 1,CellGap.y -1);

        Quaternion randomizedDirection = Quaternion.Euler(new Vector3(0,Random.Range(0,360),0));

        //Spawn the Object
        GameObject spawned = Instantiate(objectToSpawn, placeHolder.position,randomizedDirection);
        //Check if the cell position are less than the Grid Width
        if (currentCell.x < GridSize.x-1)
        {
            //If current cell are less than Grid Width, then spawn the GameObject with gap for each spawned object,
            //then assign the clone parent to the placeholder
            spawned.transform.parent = placeHolder;
            spawned.transform.position = new Vector3(currentCell.x+CellGap.x, placeHolder.position.y,
                currentCell.y+CellGap.y);
            currentCell.x += 1;
        }
        else
        {
            //If current cell are more than Grid Width, then spawn the GameObject above the current cell with gap
            //for each spawned object, then assign the clone parent to the placeholder
            spawned.transform.parent = placeHolder;
            spawned.transform.position = new Vector3(currentCell.x+CellGap.x, placeHolder.position.y,
                currentCell.y+CellGap.y);
            currentCell.x = 0;
            currentCell.y += 1;
        }
        //Return the spawned GameObject
        return spawned;
    }
}