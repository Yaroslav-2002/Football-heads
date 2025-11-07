using UnityEngine;
using UnityEngine.UI;

public class GUIView : View
{
    [SerializeField] Button gameSettingsButton;
    [SerializeField] private ScoreBoard scoreBoard;

    public ScoreBoard ScoreBoard { get { return scoreBoard; } }

    public override void Show()
    {
        base.Show();
    }

    public override void Init()
    {
        if (gameSettingsButton != null)
        {
            gameSettingsButton.onClick.AddListener(() => ViewManager.Show<GameSettingsView>());
        }
    }
}
