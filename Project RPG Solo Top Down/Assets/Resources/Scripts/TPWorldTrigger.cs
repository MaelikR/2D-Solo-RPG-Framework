using UnityEngine;

public class TPWorldTrigger : MonoBehaviour
{
    public AudioSource[] audioSources;  // Liste des sources audio
    public Transform teleportDestination; // Point de destination pour la téléportation
    public int destinationZoneIndex; // Index pour choisir la source audio à jouer

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifier si le joueur est entré dans la zone
        if (!other.CompareTag("Player")) return;

        // Si c'est bien le joueur, on le téléporte
        TeleportPlayer(other.gameObject);
        UpdateAudioSources(destinationZoneIndex); // Mise à jour des sources audio
    }

    private void TeleportPlayer(GameObject player)
    {
        // Déplacer le joueur à la destination de téléportation
        player.transform.position = teleportDestination.position;
        // Vous pouvez ajouter ici des effets visuels ou sonores supplémentaires pour la téléportation
    }

    private void UpdateAudioSources(int activeZoneIndex)
    {
        // Boucler à travers les sources audio pour activer celle correspondant à la zone actuelle
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
