using UnityEngine;
using UnityEngine.Pool;

public class EnemyPoolManager : MonoBehaviour
{
    public static EnemyPoolManager Instance { get; private set; }

    [SerializeField] private EnemyShell shellPrefab;
    private IObjectPool<EnemyShell> _pool;

    void Awake()
    {
        Instance = this;
        _pool = new ObjectPool<EnemyShell>(
            createFunc: () => Instantiate(shellPrefab),
            actionOnGet: (shell) => shell.gameObject.SetActive(true),
            actionOnRelease: (shell) => shell.gameObject.SetActive(false),
            actionOnDestroy: (shell) => Destroy(shell.gameObject),
            collectionCheck: false,
            defaultCapacity: 50,
            maxSize: 200
        );
    }

    public void Spawn(Vector3 position, EnemyConfigSO config)
    {
        EnemyShell shell = _pool.Get();
        shell.transform.position = position;
        shell.Initialize(config, _pool);

        
    }
}