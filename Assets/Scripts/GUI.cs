using UnityEngine;
using UnityEngine.UI;

public class GUI : View
{
    [SerializeField] Button gameSettingsButton;

    public override void Init()
    {
        if (gameSettingsButton != null)
        {
            gameSettingsButton.onClick.AddListener(() => ViewManager.Show<GameSettingsView>());
        }
    }
}
