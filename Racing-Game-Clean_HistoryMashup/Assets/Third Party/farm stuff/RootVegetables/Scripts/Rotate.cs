using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Party
{
public class Rotate : MonoBehaviour
{

    void Update()
    {
        transform.Rotate(new Vector3(0, 360f, 0), Time.deltaTime * 45, Space.World);
    }
}
}