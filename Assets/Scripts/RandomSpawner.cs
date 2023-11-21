using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {

    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private GameObject spawnAreaObject;
    [SerializeField] private int numberOfObjects = 10;
    [SerializeField] private float spawnInterval = 2.0f;
    [SerializeField] private float objectLifespan = 5.0f;

    private void Start() {
        StartCoroutine(SpawnObjectsWithInterval());
    }

    private IEnumerator SpawnObjectsWithInterval() {
        Renderer spawnAreaRenderer = spawnAreaObject.GetComponent<Renderer>();
        if(spawnAreaRenderer != null) {
            Vector3 spawnAreaSize = spawnAreaRenderer.bounds.size;

            float squareSizeX = spawnAreaSize.x / 6f;
            float squareSizeZ = spawnAreaSize.z / 6f;

            List<Vector3> occupiedPositions = new List<Vector3>();

            for(int i = 0; i < numberOfObjects; i++) {
                Vector3 randomPosition;

                do {
                    int randomSquareX = Random.Range(0, 6);
                    int randomSquareZ = Random.Range(0, 6);

                    float centerX = spawnAreaObject.transform.position.x - (spawnAreaSize.x / 2) + (squareSizeX * randomSquareX) + (squareSizeX / 2);
                    float centerZ = spawnAreaObject.transform.position.z - (spawnAreaSize.z / 2) + (squareSizeZ * randomSquareZ) + (squareSizeZ / 2);

                    randomPosition = new Vector3(centerX, spawnAreaObject.transform.position.y, centerZ);
                }
                while(IsPositionAdjacent(randomPosition, occupiedPositions));

                occupiedPositions.Add(randomPosition);
                GameObject spawnedObject = Instantiate(objectToSpawn, randomPosition, Quaternion.Euler(0f, 180f, 0f));

                StartCoroutine(DestroyObjectAfterDelay(spawnedObject, objectLifespan));

                yield return new WaitForSeconds(spawnInterval);
            }
        }
        else {
            Debug.LogError("Spawn area object does not have a Renderer component.");
        }
    }

    private IEnumerator DestroyObjectAfterDelay(GameObject obj, float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }

    private bool IsPositionAdjacent(Vector3 position, List<Vector3> occupiedPositions) {
        foreach(Vector3 pos in occupiedPositions) {
            if(Vector3.Distance(pos, position) < 1.1f)
            {
                return true;
            }
        }
        return false;
    }
}
