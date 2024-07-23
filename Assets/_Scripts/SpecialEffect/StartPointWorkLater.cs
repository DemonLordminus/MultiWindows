using Cysharp.Threading.Tasks;
using Unity.Netcode.Components;
using UnityEngine;

public class StartPointWorkLater : MonoBehaviour
{
    [SerializeField] private NetworkTransform networkTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Start()
    {
        await UniTask.Delay(500);
        networkTransform.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
