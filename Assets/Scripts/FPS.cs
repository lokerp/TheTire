using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS : MonoBehaviour
{
    public int frameRate;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = frameRate;
    }

}
