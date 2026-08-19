using System.Collections.Generic;
using UnityEngine;

public class PulpitSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pulpitPrefab;
    [SerializeField] private float pulpitSize = 9f; 

    private static readonly Vector3[] Directions =
    {
        Vector3.forward, Vector3.back, Vector3.left, Vector3.right
    };

    private readonly List<PulpitController> activePulpits = new List<PulpitController>();
    private PulpitController lastSpawned;
    private int nextId;
    private bool isSpawning;

    public PulpitController FirstPulpit { get; private set; }

    public void BeginSpawning()
    {
        isSpawning = true;
        SpawnFirstPulpit();
        SpawnNext(FirstPulpit.transform.position, exclude: null);
    }

    public void StopSpawning()
    {
        isSpawning = false;
    }

    public void ResetSpawner()
    {
        isSpawning = false;
        foreach (var pulpit in activePulpits)
        {
            if (pulpit != null)
                Destroy(pulpit.gameObject);
        }
        activePulpits.Clear();
        lastSpawned = null;
        FirstPulpit = null;
        nextId = 0;
    }

    private void SpawnFirstPulpit()
    {
        FirstPulpit = CreatePulpit(Vector3.zero);
        lastSpawned = FirstPulpit;
    }

    private void SpawnNext(Vector3 fromPosition, PulpitController exclude)
    {
        if (!isSpawning) return;
        if (activePulpits.Count >= 2) return; 

        Vector3 spawnPos = FindValidAdjacentPosition(fromPosition);
        PulpitController pulpit = CreatePulpit(spawnPos);
        lastSpawned = pulpit;
    }


    private Vector3 FindValidAdjacentPosition(Vector3 fromPosition)
    {
        List<Vector3> shuffled = new List<Vector3>(Directions);
        
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        foreach (Vector3 dir in shuffled)
        {
            Vector3 candidate = fromPosition + dir * pulpitSize;
            if (!IsOccupied(candidate))
                return candidate;
        }

        Vector3 fallbackDir = shuffled[0];
        return fromPosition + fallbackDir * pulpitSize * 2f;
    }

    private bool IsOccupied(Vector3 position)
    {
        const float epsilon = 0.5f;
        foreach (var pulpit in activePulpits)
        {
            if (pulpit == null) continue;
            if (Vector3.Distance(pulpit.transform.position, position) < epsilon)
                return true;
        }
        return false;
    }

    private PulpitController CreatePulpit(Vector3 position)
    {
        GameObject go = Instantiate(pulpitPrefab, position, Quaternion.identity);
        PulpitController pulpit = go.GetComponent<PulpitController>();

        var diary = ConfigLoader.Instance.Config.pulpit_data;
        float lifeTime = Random.Range(diary.min_pulpit_destroy_time, diary.max_pulpit_destroy_time);

        pulpit.Initialize(nextId++, lifeTime, diary.pulpit_spawn_time);
        pulpit.OnAboutToExpire += HandleAboutToExpire;
        pulpit.OnExpired += HandleExpired;

        activePulpits.Add(pulpit);
        return pulpit;
    }

    private void HandleAboutToExpire(PulpitController pulpit)
    {
        SpawnNext(pulpit.transform.position, pulpit);
    }

    private void HandleExpired(PulpitController pulpit)
    {
        activePulpits.Remove(pulpit);
        pulpit.OnAboutToExpire -= HandleAboutToExpire;
        pulpit.OnExpired -= HandleExpired;
    }
}
