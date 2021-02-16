using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    PlayerController controller;
    [SerializeField] Transform spherePos;
    [SerializeField] LayerMask mask; 

    private void Start() {
        controller = GetComponent<PlayerController>();
    }

    private void Update() {
        controller.grounded = Physics.CheckSphere(spherePos.position , 0.1f , mask);
    }
}
