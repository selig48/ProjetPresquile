using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;

#region JSON STRUCTURES

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
    public double? construction_year;   // nullable (important)
    public string heating_energy;
}

#endregion

public class Building
{
    public List<Vector2> polygon;
    public Properties properties;
}

public class BuildingManager : MonoBehaviour
{
    [Header("JSON FILE")]
    public string fileName = "data_BDNB_ONB_grenoble_mapshaper.json";

    [Header("COORDINATE CONVERSION")]
    public double refX = 913384.6;
    public double refY = 6456736.1;
    public float scale = 1f;

    private List<Building> buildings = new List<Building>();

    IEnumerator Start()
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        UnityWebRequest req = UnityWebRequest.Get(path);
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ JSON LOAD ERROR: " + req.error);
            yield break;
        }

        Debug.Log("✅ JSON LOADED");

        FeatureCollection geo =
            JsonConvert.DeserializeObject<FeatureCollection>(req.downloadHandler.text);

        foreach (Feature f in geo.features)
        {
            if (f.geometry.type != "Polygon")
                continue;

            Building b = new Building
            {
                polygon = new List<Vector2>(),
                properties = f.properties
            };

            double[][] ring = f.geometry.coordinates[0];

            foreach (double[] p in ring)
            {
                float x = (float)((p[0] - refX) * scale);
                float y = (float)((p[1] - refY) * scale);
                b.polygon.Add(new Vector2(x, y));
            }

            buildings.Add(b);
        }

        Debug.Log("🏢 BUILDINGS LOADED: " + buildings.Count);
    }

    // 🔍 SEARCH BY ID (used by UI)
    public Building GetBuildingByID(string id)
    {
        foreach (Building b in buildings)
        {
            if (b.properties.batiment_construction_id == id)
                return b;
        }
        return null;
    }

    // (Optional) access to all buildings
    public List<Building> GetBuildings()
    {
        return buildings;
    }
}
