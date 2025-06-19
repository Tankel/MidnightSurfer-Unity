using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject cloudPrefab; // Prefab de nube único
    public Transform player; // Referencia al transform del jugador
    public float spawnInterval = 5.0f; // Intervalo de tiempo entre spawns
    public float spawnRangeX = 20.0f; // Rango horizontal para spawnear nubes
    public float spawnMinY = 10.0f; // Altura mínima para spawnear nubes
    public float spawnMaxY = 20.0f; // Altura máxima para spawnear nubes
    public float scaleMin = 0.5f; // Escala mínima
    public float scaleMax = 1.5f; // Escala máxima
    public float spawnOffsetX = 10.0f; // Desplazamiento horizontal desde la posición del jugador
    public int maxClouds = 10; // Número máximo de nubes permitidas
    private float timer = 0.0f; // Temporizador para controlar el spawn
    private List<GameObject> clouds = new List<GameObject>(); // Lista de nubes generadas
    private void Start() {
        SpawnCloud();
    }
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval && player.position.x > transform.position.x)
        {
            SpawnCloud();
            timer = 0.0f; // Reiniciar el temporizador
        }
    }

    private void SpawnCloud()
    {
        float randomY = Random.Range(spawnMinY, spawnMaxY); // Posición Y aleatoria dentro del rango
        Vector3 spawnPosition = new Vector3(player.position.x + spawnOffsetX + Random.Range(0, spawnRangeX), randomY, 0); // Calcular la posición de spawn
        GameObject cloud = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity); // Instanciar la nube

        // Asignar un color aleatorio a la nube
        Color randomColor = new Color(
            Random.Range(100f / 255f, 255f / 255f),
            Random.Range(100f / 255f, 255f / 255f),
            Random.Range(100f / 255f, 255f / 255f)
        );
        cloud.GetComponent<SpriteRenderer>().color = randomColor;

        // Escalar aleatoriamente con el mismo valor en los ejes X e Y
        float randomScale = Random.Range(scaleMin, scaleMax);
        cloud.transform.localScale = new Vector3(randomScale, randomScale, 1);

        clouds.Add(cloud); // Agregar la nube a la lista de nubes generadas

        // Borrar nubes antiguas si se excede el límite máximo
        if (clouds.Count > maxClouds)
        {
            Destroy(clouds[0]);
            clouds.RemoveAt(0);
        }
    }
}
