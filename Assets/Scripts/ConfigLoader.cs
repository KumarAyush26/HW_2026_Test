using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ConfigLoader : MonoBehaviour
{
    public static ConfigLoader Instance { get; private set; }
    public DoofusDiary Config { get; private set; }
    public bool IsLoaded { get; private set; }

    private const string DiaryUrl =
        "https://s3.ap-south-1.amazonaws.com/superstars.assetbundles.testbuild/doofus_game/doofus_diary.json";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Load(Action onComplete)
    {
        StartCoroutine(LoadRoutine(onComplete));
    }

    private IEnumerator LoadRoutine(Action onComplete)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(DiaryUrl))
        {
            request.timeout = 10; 
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[ConfigLoader] Failed to fetch diary ({request.error}). Using default values.");
                Config = DoofusDiary.Default();
            }
            else
            {
                try
                {
                    DoofusDiary parsed = JsonUtility.FromJson<DoofusDiary>(request.downloadHandler.text);

                    if (parsed == null || parsed.player_data == null || parsed.pulpit_data == null)
                        throw new Exception("Parsed JSON missing expected fields.");

                    Config = SanitizeConfig(parsed);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[ConfigLoader] Failed to parse diary JSON ({e.Message}). Using default values.");
                    Config = DoofusDiary.Default();
                }
            }
        }

        IsLoaded = true;
        onComplete?.Invoke();
    }

    private DoofusDiary SanitizeConfig(DoofusDiary diary)
    {
        if (diary.player_data.speed <= 0f)
            diary.player_data.speed = 3f;

        if (diary.pulpit_data.min_pulpit_destroy_time <= 0f)
            diary.pulpit_data.min_pulpit_destroy_time = 4f;

        if (diary.pulpit_data.max_pulpit_destroy_time < diary.pulpit_data.min_pulpit_destroy_time)
            diary.pulpit_data.max_pulpit_destroy_time = diary.pulpit_data.min_pulpit_destroy_time + 1f;

        if (diary.pulpit_data.pulpit_spawn_time <= 0f)
            diary.pulpit_data.pulpit_spawn_time = diary.pulpit_data.min_pulpit_destroy_time * 0.5f;

        return diary;
    }
}
