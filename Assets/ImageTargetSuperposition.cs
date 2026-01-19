using UnityEngine;
using Vuforia;

public class ImageTarget9 : MonoBehaviour
{
    public GameObject axe_ville;  // ton objet 3D
    public GameObject plane;      // ton plan invisible pour les touches

    private static int activeTargets = 0; // nombre de targets détectés

    void Start()
    {
        var observer = GetComponent<ObserverBehaviour>();
        observer.OnTargetStatusChanged += OnStatusChanged;
    }

    private void OnStatusChanged(ObserverBehaviour behaviour, TargetStatus status)
    {
        if (status.Status == Status.TRACKED || status.Status == Status.EXTENDED_TRACKED)
        {
            activeTargets++;
        }
        else if (status.Status == Status.NO_POSE)
        {
            activeTargets--;
            if (activeTargets < 0) activeTargets = 0;
        }

        // Active l'objet et le plan seulement si au moins un target est détecté
        bool isActive = activeTargets > 0;
        axe_ville.SetActive(isActive);
        if (plane != null)
            plane.SetActive(isActive);
    }
}
