using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBoxCollider2D : MonoBehaviour
{

    public static List<LightBoxCollider2D> statics = new List<LightBoxCollider2D>();
    public static Mesh staticMesh;

    public static List<LightBoxCollider2D> dynamics = new List<LightBoxCollider2D>();

    public static Mesh dynamicMesh
    {

        get
        {

            return GetMesh(dynamics);

        }

    }

    new Transform transform;

    public bool isStatic;

    public virtual void OnEnable()
    {

        if (isStatic)
        {
            statics.Add(this);
            UpdateStaticMesh();
        }
        else dynamics.Add(this);

    }

    public virtual void OnDisable()
    {

        if (isStatic)
        {
            statics.Remove(this);
            UpdateStaticMesh();
        }
        else dynamics.Remove(this);

    }

    // Get the outline of the object for shadow map rendering
    public void GetEdges(List<Vector2> edges)
    {

        if (transform == null) transform = GetComponent<Transform>();

        /*
        Vector2 scale = transform.localScale / 2f;

        Vector2 v1 = new Vector2(-scale.x, -scale.y);
        Vector2 v2 = new Vector2(+scale.x, -scale.y);
        Vector2 v3 = new Vector2(+scale.x, +scale.y);
        Vector2 v4 = new Vector2(-scale.x, +scale.y);
        */

        Vector2 v1 = new Vector2(-0.5f, -0.5f);
        Vector2 v2 = new Vector2(+0.5f, -0.5f);
        Vector2 v3 = new Vector2(+0.5f, +0.5f);
        Vector2 v4 = new Vector2(-0.5f, +0.5f);

        v1 = transform.localToWorldMatrix.MultiplyPoint(v1);
        v2 = transform.localToWorldMatrix.MultiplyPoint(v2);
        v3 = transform.localToWorldMatrix.MultiplyPoint(v3);
        v4 = transform.localToWorldMatrix.MultiplyPoint(v4);

        edges.Add(v1);
        edges.Add(v2);

        edges.Add(v2);
        edges.Add(v3);

        edges.Add(v3);
        edges.Add(v4);

        edges.Add(v4);
        edges.Add(v1);

    }

    public static void UpdateStaticMesh()
    {

        staticMesh = GetMesh(statics);

    }

    public static Mesh GetMesh(List<LightBoxCollider2D> colliders)
    {
        List<Vector2> edges = new List<Vector2>();
        for (int i = 0; i < statics.Count; i++)
        {
            colliders[i].GetEdges(edges);
        }

        List<Vector3> verts = new List<Vector3>();
        List<Vector2> normals = new List<Vector2>();
        for (int i = 0; i < edges.Count; i += 2)
        {
            verts.Add(edges[i + 0]);
            verts.Add(edges[i + 1]);
            normals.Add(edges[i + 1]);
            normals.Add(edges[i + 0]);
        }

        // Simple 1:1 index buffer
        int[] incides = new int[edges.Count];
        for (int i = 0; i < edges.Count; i++)
        {
            incides[i] = i;
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetUVs(0, normals);
        mesh.SetIndices(incides, MeshTopology.Lines, 0);
        return mesh;

    }

}
