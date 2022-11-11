using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[ExecuteInEditMode]
public class SerializeTest : MonoBehaviour
{
    [SerializeReference]
    public Func f;
    void Start() {
        
    }
    void Update() {
        f = null;
    }
}
