using UnityEngine;
using TMPro; // Import TextMeshPro

public class TouchDetector : MonoBehaviour
{
    public GameObject plane;             // Le plan invisible
    public TextMeshProUGUI coordText;    // UI TextMeshPro pour afficher coordonnées

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == plane)
                    {
                        Vector3 hitPoint = hit.point;

                        // Convertir en coordonnées écran
                        Vector3 screenPos = Camera.main.WorldToScreenPoint(hitPoint);

                        // Afficher en haut à droite
                        coordText.text = $"X: {screenPos.x:0}, Y: {screenPos.y:0}";
                    }
                }
            }
        }
    }
}
