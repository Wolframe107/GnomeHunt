using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomeScript : MonoBehaviour
{

    /*
    OBS DENNA FIL ÄR NOG REDUNDANT
    kan tas bort?
    */

    //public GameObject VR_audio;  // The other GameObject that has the AudioSource
    //private AudioSource audioSource;
    private bool isPlayer;
   
    void Start()
    {
        // Get the AudioSource component
        //audioSource = VR_audio.GetComponent<AudioSource>();
        isPlayer = false;
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyGnome() {
        //DestroyHelper();
        //audioSource.Play();

        Destroy(gameObject);
    }

    private IEnumerator DestroyHelper() {
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }

    public bool CheckIfPlayer() {
        return isPlayer;
    }

}
