using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia pools de objetos para otimização de performance.
/// </summary>
public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance { get; private set; } // Instância singleton

    [System.Serializable]
    public class Pool
    {
        public string tag; // Tag do pool
        public GameObject prefab; // Prefab a ser instanciado
        public int size; // Tamanho inicial do pool
    }

    [Header("Configurações de Pool")]
    [SerializeField] private List<Pool> pools; // Lista de pools
    [SerializeField] private Transform poolParent; // Pai dos objetos do pool

    private Dictionary<string, Queue<GameObject>> poolDictionary; // Dicionário de pools

    /// <summary>
    /// Inicializa o singleton e cria os pools
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = CreateNewObject(pool.prefab);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    /// <summary>
    /// Cria um novo objeto para o pool
    /// </summary>
    /// <param name="prefab">Prefab a ser instanciado</param>
    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.transform.SetParent(poolParent);
        obj.SetActive(false);
        return obj;
    }

    /// <summary>
    /// Spawna um objeto do pool
    /// </summary>
    /// <param name="tag">Tag do pool</param>
    /// <param name="position">Posição do spawn</param>
    /// <param name="rotation">Rotação do spawn</param>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool com tag " + tag + " não existe!");
            return null;
        }

        Queue<GameObject> pool = poolDictionary[tag];
        GameObject objectToSpawn;

        // Se o pool estiver vazio, cria um novo objeto
        if (pool.Count == 0)
        {
            Pool poolConfig = pools.Find(p => p.tag == tag);
            if (poolConfig != null)
            {
                objectToSpawn = CreateNewObject(poolConfig.prefab);
            }
            else
            {
                return null;
            }
        }
        else
        {
            objectToSpawn = pool.Dequeue();
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();
        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        return objectToSpawn;
    }

    /// <summary>
    /// Retorna um objeto ao pool
    /// </summary>
    /// <param name="tag">Tag do pool</param>
    /// <param name="obj">Objeto a ser retornado</param>
    public void ReturnToPool(string tag, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool com tag " + tag + " não existe!");
            return;
        }

        obj.SetActive(false);
        poolDictionary[tag].Enqueue(obj);
    }

    /// <summary>
    /// Expande o tamanho de um pool
    /// </summary>
    /// <param name="tag">Tag do pool</param>
    /// <param name="amount">Quantidade a ser adicionada</param>
    public void ExpandPool(string tag, int amount)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool com tag " + tag + " não existe!");
            return;
        }

        Pool poolConfig = pools.Find(p => p.tag == tag);
        if (poolConfig == null) return;

        for (int i = 0; i < amount; i++)
        {
            GameObject obj = CreateNewObject(poolConfig.prefab);
            poolDictionary[tag].Enqueue(obj);
        }
    }

    /// <summary>
    /// Limpa todos os pools
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var pool in poolDictionary.Values)
        {
            while (pool.Count > 0)
            {
                GameObject obj = pool.Dequeue();
                Destroy(obj);
            }
        }
        poolDictionary.Clear();
    }
}

/// <summary>
/// Interface para objetos que podem ser poolados
/// </summary>
public interface IPooledObject
{
    void OnObjectSpawn();
} 