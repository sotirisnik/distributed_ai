using UnityEngine;
using System.Collections;
using System.IO;

public class Test1 : MonoBehaviour {

    // Use this for initialization

    int i = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Input.mousePosition;
            pos.x = Screen.width/ pos.x- (Screen.width/ pos.x )/ 2;
            pos.y = pos.y / Screen.height - 0.5f;
            Debug.Log(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())));
        }
    }
}
