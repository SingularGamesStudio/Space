using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class test : MonoBehaviour
{
    Queue<float> fps = new Queue<float>();
    float sum = 0;
    private void Start() {
        for (int i = 0; i < 10; i++) {
            fps.Enqueue(0);
        }
    }
    void Update()
    {
        sum += (1f / Time.deltaTime);
        fps.Enqueue((1f / Time.deltaTime));
        sum -= fps.Dequeue();
        gameObject.GetComponent<Text>().text = (sum/10f).ToString();
    }
}
