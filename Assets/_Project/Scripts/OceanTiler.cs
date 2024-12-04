using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanTiler : MonoBehaviour
{
    public GameObject oceanTile;
    public int tilesX = 5;
    public int tilesZ = 5;
    public float tileSize = 10f;
    public Vector3 oceanOffset = Vector3.zero; // Add this line

    void Start()
    {
        CreateOceanGrid();
    }

    void CreateOceanGrid()
    {
        Vector3 startPosition = transform.position;
        for (int x = 0; x < tilesX; x++)
        {
            for (int z = 0; z < tilesZ; z++)
            {
                Vector3 position = startPosition + new Vector3(x * tileSize, 0, z * tileSize);
                GameObject tile = Instantiate(oceanTile, position, Quaternion.identity, transform);
                tile.name = $"OceanTile_{x}_{z}";
            }
        }
    }
}