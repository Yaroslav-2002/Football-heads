using UnityEngine;

[CreateAssetMenu(
    fileName = "Config",
    menuName = "GameConfig")]
public class GameConfiguration : ScriptableObject
{
    public GameMode GameMode;
}
