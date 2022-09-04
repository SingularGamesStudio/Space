using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    Matrix4x4 tf = Matrix4x4.identity;
    Mesh mesh;
    public Material material;
    // Start is called before the first frame update
    void Start()
    {
        tf.SetTRS(new Vector3(0, 0), Quaternion.Euler(90, 90, -90), new Vector3(1, 1, 1));
        mesh = gameObject.GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        Graphics.DrawMesh(mesh, tf, material, 0);
    }
}
