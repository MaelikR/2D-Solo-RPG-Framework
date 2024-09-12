using UnityEngine;

public class TPWorldTrigger : MonoBehaviour
{
    public AudioSource[] audioSources;  // Liste des sources audio
    public Transform teleportDestination; // Point de destination pour la t�l�portation
    public int destinationZoneIndex; // Index pour choisir la source audio � jouer

    private void OnTriggerEnter2D(Collider2D other)
    {
        // V�rifier si le joueur est entr� dans la zone
        if (!other.CompareTag("Player")) return;

        // Si c'est bien le joueur, on le t�l�porte
        TeleportPlayer(other.gameObject);
        UpdateAudioSources(destinationZoneIndex); // Mise � jour des sources audio
    }

    private void TeleportPlayer(GameObject player)
    {
        // D�placer le joueur � la destination de t�l�portation
        player.transform.position = teleportDestination.position;
        // Vous pouvez ajouter ici des effets visuels ou sonores suppl�mentaires pour la t�l�portation
    }

    private void UpdateAudioSources(int activeZoneIndex)
    {
        // Boucler � travers les sources audio pour activer celle correspondant � la zone actuelle
        for (int i = 0; i < audioSources.Length; i++)
        {
            if (i == activeZoneIndex)
            {
                audioSources[i].Play();
            }
            else
            {
                audioSources[i].Stop();
            }
        }
    }
}
