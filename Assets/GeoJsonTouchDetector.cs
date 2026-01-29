using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class GeoJsonTouchDetector : MonoBehaviour
{
    public BuildingManager buildingManager;   // SOURCE OF DATA
    public TextMeshProUGUI infoText;

    [Header("Detection")]
    public float touchTolerance = 1500f;

    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            HandleTouch(Input.GetTouch(0).position);

        if (Input.GetMouseButtonDown(0))
            HandleTouch(Input.mousePosition);
    }

    void HandleTouch(Vector2 screenPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
            return;

        Vector2 mapPoint = new Vector2(hit.point.x, hit.point.z);

        foreach (Building b in buildingManager.GetBuildings())
        {
            if (IsPointInsidePolygon(mapPoint, b.polygon) ||
                IsPointNearPolygon(mapPoint, b.polygon, touchTolerance))
            {
                ShowBuildingInfo(b, mapPoint);
                return;
            }
        }

        ShowNoBuildingInfo(mapPoint);
    }

    bool IsPointInsidePolygon(Vector2 point, List<Vector2> poly)
    {
        bool inside = false;
        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
        {
            if (((poly[i].y > point.y) != (poly[j].y > point.y)) &&
                (point.x < (poly[j].x - poly[i].x) *
                (point.y - poly[i].y) /
                (poly[j].y - poly[i].y) + poly[i].x))
            {
                inside = !inside;
            }
        }
        return inside;
    }

    bool IsPointNearPolygon(Vector2 point, List<Vector2> poly, float tol)
    {
        for (int i = 0; i < poly.Count; i++)
        {
            Vector2 a = poly[i];
            Vector2 b = poly[(i + 1) % poly.Count];
            if (DistancePointToSegment(point, a, b) <= tol)
                return true;
        }
        return false;
    }

    float DistancePointToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 ab = b - a;
        float t = Vector2.Dot(p - a, ab) / ab.sqrMagnitude;
        t = Mathf.Clamp01(t);
        Vector2 proj = a + t * ab;
        return Vector2.Distance(p, proj);
    }

    void ShowBuildingInfo(Building b, Vector2 mapPoint)
    {
        infoText.text =
            "<b>BÂTIMENT DÉTECTÉ</b>\n\n" +
            $"ID : {b.properties.batiment_construction_id}\n" +
            $"Usage : {b.properties.usage_main}\n" +
            $"Année : {b.properties.construction_year}\n" +
            $"Chauffage : {b.properties.heating_energy}\n\n" +
            $"Position :\nX = {mapPoint.x:F2}\nY = {mapPoint.y:F2}";

        infoText.gameObject.SetActive(true);
    }

    void ShowNoBuildingInfo(Vector2 mapPoint)
    {
        infoText.text =
            "<b>AUCUN BÂTIMENT</b>\n\n" +
            $"X = {mapPoint.x:F2}\nY = {mapPoint.y:F2}";

        infoText.gameObject.SetActive(true);
    }
}
