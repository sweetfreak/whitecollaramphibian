using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FindAnimatedTarget : MonoBehaviour
{
    private CinemachineStateDrivenCamera cinemachineStateDrivenCamera;
void Start()
{
    cinemachineStateDrivenCamera = GetComponent<CinemachineStateDrivenCamera>();
}

    // Update is called once per frame
    void Update()
    {
        if (!cinemachineStateDrivenCamera.m_AnimatedTarget)
        {
            cinemachineStateDrivenCamera.m_AnimatedTarget = FindObjectOfType<PlayerManager>().GetComponent<Animator>();
            if (cinemachineStateDrivenCamera.m_AnimatedTarget)
            {
                Debug.Log("Found an animator!");
            }
        } 
    }
}
