using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ChunkSpawner : MonoBehaviour
{
    public GameObject[] chunkPrefabs; // Array de prefabs de los chunks
    public Transform player; // Referencia al transform del jugador
    private float chunkWidth = 350.0f; // Ancho de cada chunk
    private List<GameObject> spawnedChunks = new List<GameObject>(); // Lista de chunks generados
	public GameObject coinPrefab;
	private GameObject coinRef;

    private void Start()
    {
        // Generar los primeros 3 chunks
        for (int i = 0; i < 3; i++)
        {
            SpawnChunk(i * chunkWidth);
        }
    }

    private void Update()
    {
        // Obtener el chunk más lejano en x
        GameObject farthestChunk = spawnedChunks[spawnedChunks.Count - 1];

        // Si el jugador ha cruzado el punto x del chunk más lejano
        if (player.position.x > farthestChunk.transform.position.x - chunkWidth / 2)
        {
            // Generar un nuevo chunk
            SpawnChunk(farthestChunk.transform.position.x + chunkWidth);
        }
    }

    private void SpawnChunk(float xPosition)
    {
        int randomIndex = Random.Range(0, chunkPrefabs.Length); // Seleccionar un prefab al azar
        Vector3 spawnPosition = new Vector3(xPosition, 0, 0); // Calcular la posición de spawn
        GameObject newChunk = Instantiate(chunkPrefabs[randomIndex], spawnPosition, Quaternion.identity); // Instanciar el chunk
		
		for(int i = 0; i < chunkWidth; i +=5)
		{
			coinRef = Instantiate(coinPrefab, new Vector3(i + xPosition, 500, 0), Quaternion.identity);
			coinRef.transform.SetParent(newChunk.transform);
			RaycastHit2D hit = Physics2D.Raycast(coinRef.transform.position, -Vector3.up, 1000f, 1 << 6);
			
			if(hit)
			{
				//Debug.Log("Chunk hit at " + hit.collider.name);
				coinRef.transform.position = hit.point;
				coinRef.transform.position += new Vector3(0, 1, 0);
			}
			else
			{
				Destroy(coinRef);
			}
			
		}
		
        spawnedChunks.Add(newChunk); // Agregar el nuevo chunk a la lista

        // Opcional: destruir chunks antiguos para evitar consumo excesivo de memoria
        if (spawnedChunks.Count > 3)
        {
            Destroy(spawnedChunks[0]);
            spawnedChunks.RemoveAt(0);
        }
    }
}
