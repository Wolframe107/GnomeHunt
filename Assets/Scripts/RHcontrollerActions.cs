using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RHcontrollerActions : MonoBehaviour
{
    //[SerializeField] private InputActionReference grip;
    [SerializeField] private InputActionReference throwHammerValue;
    [SerializeField] private float deactivateTime;
    [SerializeField] GameObject rightHand;
    [SerializeField] GameObject laser;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject pulse;
    [SerializeField] private float hammerSpeed;
    [SerializeField] private XRBaseController rController;
    private bool gunActive;

    private bool throwReady;
    // Start is called before the first frame update
    void Start()
    {
        throwReady = true;
        gunActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        //float gripValue = grip.action.ReadValue<float>();
        float throwValue = throwHammerValue.action.ReadValue<float>();
        if(throwValue != 0.0f && throwReady) {
            throwHammer();
        }
        if(throwValue == 0.0f && !throwReady) {
            throwReady = true;
        }
    }

    private void throwHammer() {
        if(throwReady && gunActive) {
            throwReady = false;
            Vib(rController, 0.3f);
            Vector3 position = rightHand.transform.position+(rightHand.transform.forward/2);
            GameObject spawnLaser = Instantiate(laser, position, Quaternion.identity);
            Quaternion offset = Quaternion.Euler(90, 0, 0);
            spawnLaser.transform.rotation = rightHand.transform.rotation * offset;
            spawnLaser.GetComponent<HammerScript>().SetGun(gameObject);
            spawnLaser.GetComponent<Rigidbody>().velocity = rightHand.transform.forward * hammerSpeed;
        }
    }

    public void DeactivateGun() {
        gunActive = false;
        StartCoroutine(DeactivateHelper());
    }

    private IEnumerator DeactivateHelper() {
        GameObject e = Instantiate(explosion, rightHand.transform.position, Quaternion.identity);
        int pulses = 0;
        while(pulses < deactivateTime) {
            GameObject p = Instantiate(pulse, rightHand.transform.position, Quaternion.identity);
            p.GetComponent<PulseScript>().SetGun(rightHand);
            Vib(rController, 0.9f);
            yield return new WaitForSeconds(1f);
            Destroy(p);
            pulses++;
        }
        //yield return new WaitForSeconds(deactivateTime);
        Destroy(e);
        gunActive = true;
    }

    public void Vib(XRBaseController controller, float intensity) {
        controller.SendHapticImpulse(intensity ,0.1f);
    }
}
