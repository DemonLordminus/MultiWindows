using UnityEngine;

public class DontDestroyOnLoadTool : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


}
