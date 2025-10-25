using System.Collections.Generic;

public enum SceneType
{
    Menu,
    Game,
}

public static class SceneNames
{
    public static readonly Dictionary<SceneType, string> Map = new()
    {
        { SceneType.Menu, "Menu" },
        { SceneType.Game, "Game" },
    };
}