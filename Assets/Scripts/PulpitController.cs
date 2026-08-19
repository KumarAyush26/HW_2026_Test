using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PulpitController : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text timerText;
    public int PulpitId { get; private set; }

    public event Action<PulpitController> OnAboutToExpire;
    public event Action<PulpitController> OnExpired;

    private float lifeTime;
    private float spawnTriggerTime; // "x": remaining seconds at which the next pulpit should spawn
    private float elapsed;
    private bool hasTriggeredSpawn;

    private Renderer pulpitRenderer;
    private Color baseColor;
    private static readonly Color WarningColor = new Color(0.9f, 0.2f, 0.15f);

    public void Initialize(int id, float lifeTime, float spawnTriggerTime)
    {
        PulpitId = id;
        this.lifeTime = Mathf.Max(0.1f, lifeTime);
       
        this.spawnTriggerTime = Mathf.Clamp(spawnTriggerTime, 0f, this.lifeTime);
        elapsed = 0f;
        hasTriggeredSpawn = false;

        pulpitRenderer = GetComponentInChildren<Renderer>();
        if (pulpitRenderer != null)
        {
            
            pulpitRenderer.material = new Material(pulpitRenderer.material);
            baseColor = pulpitRenderer.material.color;
        }
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float remaining = lifeTime - elapsed;

        if (!hasTriggeredSpawn && remaining <= spawnTriggerTime)
        {
            hasTriggeredSpawn = true;
            OnAboutToExpire?.Invoke(this);
        }

        if (pulpitRenderer != null)
        {
            float warnWindow = Mathf.Max(spawnTriggerTime, 0.01f);
            float t = 1f - Mathf.Clamp01(remaining / warnWindow);
            pulpitRenderer.material.color = Color.Lerp(baseColor, WarningColor, t);
        }
        if (timerText != null)
        {
            timerText.text = Mathf.Max(0f, remaining).ToString("F2");
        }

        if (remaining <= 0f)
        {
            OnExpired?.Invoke(this);
            Destroy(gameObject);
        }
    }
}