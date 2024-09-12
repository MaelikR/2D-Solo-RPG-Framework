using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public void SaveGame(PlayerController player)
    {
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetFloat("Health", player.currentHealth);
    }

    public void LoadGame(PlayerController player)
    {
        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");
        player.transform.position = new Vector2(x, y);
        player.currentHealth = PlayerPrefs.GetFloat("Health");
    }
}
