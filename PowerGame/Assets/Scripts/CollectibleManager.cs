using System.Collections.Generic;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    [Header("Liste des objets ramassables")]
    public List<GameObject> collectibles = new List<GameObject>();

    void Update()
    {
        // Appuyer sur C pour afficher les objets dans la console
        if (Input.GetKeyDown(KeyCode.C))
        {
            ShowCollectibles();
        }
    }

    void ShowCollectibles()
    {
        if (collectibles.Count == 0)
        {
            Debug.Log("Aucun objet ramassable n'est défini.");
            return;
        }

        Debug.Log("Objets ramassables :");
        foreach (GameObject obj in collectibles)//Une boucle foreach pour afficher les objets de la liste dans la console.
        {
            Debug.Log($"- {obj.name}");
        }
    }
}
