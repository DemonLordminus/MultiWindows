using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdjustTool : MonoBehaviour
{
    [Required] public Button cameraSizeChangeButton;
    [Required] public TMP_InputField cameraSizeText;
    [Required] public CameraGroup cameraGroup;

    private void Start()
    {
        cameraSizeChangeButton.onClick.AddListener(() =>
        {
            //cameraGroup.virtualCamera.m_Lens.OrthographicSize = float.Parse(cameraSizeText.text);
            cameraGroup.AutoSetCameraOrthographic();
        });
    }
}
