using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
   private Transform followRoot => SystemManager.Instance.cameraRoot;
   
    private void Update()
    {
        transform.position = followRoot.position;
    }
}
