using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseScript : MonoBehaviour
{
    GameObject gunHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UpdatePosition();
    }

    private void UpdatePosition() {
        gameObject.transform.parent = gunHand.transform;
    }

    public void SetGun(GameObject gun) {
        gunHand = gun;
    }
}
