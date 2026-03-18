using UnityEngine;

public class LevelPoints : MonoBehaviour
{
    [SerializeField]
    private Transform[] waitPoints;

    [SerializeField]
    private Transform[] spawnPoints;

    public Transform[] WaitPoints => waitPoints;
    public Transform[] SpawnPoints => spawnPoints;
}
