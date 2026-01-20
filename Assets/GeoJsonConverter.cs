using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

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
}

public class GeoJsonConverter : MonoBehaviour
{
    public string fileName = "data_BDNB_ONB_grenoble_mapshaper.json";

    // reference near data (can be min or center)
    public double refX = 913350.0;
    public double refY = 6456700.0;

    // 1 unity unit = 1 meter 
    public float scale = 1f;

    public List<List<Vector2>> unityPolygons = new();

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

            double[][] ring = f.geometry.coordinates[0];
            List<Vector2> poly = new();

            foreach (double[] p in ring)
            {
                float x = (float)((p[0] - refX) * scale);
                float y = (float)((p[1] - refY) * scale);
                poly.Add(new Vector2(x, y));
            }

            unityPolygons.Add(poly);
        }
    }
}
