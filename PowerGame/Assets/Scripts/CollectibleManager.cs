
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

        bool wasActive = collectibles[0].activeSelf; // vérifier si le premier est actif
        bool setActive = !wasActive; // inverser l'état pour tous

        foreach (GameObject obj in collectibles)
        {
            obj.SetActive(setActive); // toggle actif/inactif
        }

        Debug.Log($"Les objets ont été {(setActive ? "affichés" : "cachés")}.");
    }
    //L’implémentation CollectibleManager que j fait :
   // crée une liste publique d’objets ramassables(List<GameObject>),
//permet via la touche C de les activer/désactiver en séquence,
//est clairement observable dans le jeu pour démontrer l’usage de la structure.
}
