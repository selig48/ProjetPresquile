using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

#region GeoJSON data structures

[System.Serializable]
public class FeatureCollection
{
    public Feature[] features;
}

[System.Serializable]
public class Feature
{
    public Geometry geometry;
    public Properties properties;
}

[System.Serializable]
public class Geometry
{
    public string type;
    public double[][][] coordinates;
}

[System.Serializable]
public class Properties
{
    public string batiment_construction_id;
    public string usage_main;
    public double construction_year;
    public string heating_energy;
}

#endregion

#region Runtime building container

public class Building
{
    public List<Vector2> polygon;
    public Properties properties;
}

#endregion

public class GeoJsonBuildingSelector : MonoBehaviour
{
    public string fileName = "data_BDNB_ONB_grenoble_mapshaper.json";

    // Reference origin
    public double refX = 913350.0;
    public double refY = 6456700.0;

    // 1 Unity unit = 1 meter
    public float scale = 1f;

    private List<Building> buildings = new();

    IEnumerator Start()
    {
        string path = System.IO.Path.Combine(
            Application.streamingAssetsPath, fileName
        );

        UnityWebRequest req = UnityWebRequest.Get(path);
        yield return req.SendWebRequest();

        FeatureCollection geo =
            JsonUtility.FromJson<FeatureCollection>(req.downloadHandler.text);

        foreach (Feature f in geo.features)
        {
            if (f.geometry.type != "Polygon")
                continue;

            Building b = new Building
            {
                polygon = new List<Vector2>(),
                properties = f.properties
            };

            // Exterior ring only
            double[][] ring = f.geometry.coordinates[0];

            foreach (double[] p in ring)
            {
                float x = (float)((p[0] - refX) * scale);
                float y = (float)((p[1] - refY) * scale);
                b.polygon.Add(new Vector2(x, y));
            }

            buildings.Add(b);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 world =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 touch = new Vector2(world.x, world.y);

            foreach (Building b in buildings)
            {
                if (IsPointInsidePolygon(touch, b.polygon))
                {
                    ShowBuildingInfo(b, touch);
                    break;
                }
            }
        }
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

    void ShowBuildingInfo(Building b, Vector2 pos)
    {
        Debug.Log(
            $"Building ID: {b.properties.batiment_construction_id}\n" +
            $"Usage: {b.properties.usage_main}\n" +
            $"Year: {b.properties.construction_year}\n" +
            $"Heating: {b.properties.heating_energy}\n" +
            $"Position: {pos}"
        );
    }
}
