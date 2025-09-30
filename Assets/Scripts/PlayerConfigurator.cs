using UnityEngine;

public sealed class PlayerConfigurator
{
    public void Configure(GameObject playerObject, PlayerInput.ControlScheme controlScheme)
    {
        if (playerObject == null)
        {
            return;
        }

        PlayerInput playerInput = playerObject.GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogWarning($"PlayerConfigurator: {playerObject.name} is missing a PlayerInput component.", playerObject);
            return;
        }

        playerInput.Configure(controlScheme);
    }
}
