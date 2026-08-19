using System;

[Serializable]
public class PlayerData
{
    public float speed;
}

[Serializable]
public class PulpitData
{
    public float min_pulpit_destroy_time;
    public float max_pulpit_destroy_time;
    public float pulpit_spawn_time;
}

[Serializable]
public class DoofusDiary
{
    public PlayerData player_data;
    public PulpitData pulpit_data;


    public static DoofusDiary Default()
    {
        return new DoofusDiary
        {
            player_data = new PlayerData { speed = 3f },
            pulpit_data = new PulpitData
            {
                min_pulpit_destroy_time = 4f,
                max_pulpit_destroy_time = 5f,
                pulpit_spawn_time = 2.5f
            }
        };
    }
}
