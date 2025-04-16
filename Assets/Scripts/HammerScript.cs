/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerScript : MonoBehaviour
{
    GameObject laserGunHand;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {   
        if(collision.transform.TryGetComponent(out GnomeScript gnome)) { 
            gnome.DestroyGnome();
            if(!gnome.CheckIfPlayer()){
                RHcontrollerActions gunScript = laserGunHand.GetComponent<RHcontrollerActions>();
                gunScript.DeactivateGun();
            }
        }

        if(collision.transform.TryGetComponent(out GnomePlayer gnome_player)) { 
            gnome_player.DestroyGnome();
            if(!gnome_player.CheckIfPlayer()){
                RHcontrollerActions gunScript = laserGunHand.GetComponent<RHcontrollerActions>();
                gunScript.DeactivateGun();
            } else {
                GameObject.Find("UIManager").GetComponent<UIManager>().removeGnome();
            }
        }
        
        Destroy(gameObject);
        //StartCoroutine(DestroyHammer());

    }

    private IEnumerator DestroyHammer() {
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }

    public void SetGun(GameObject gun) {
        laserGunHand = gun;
    }
}

 */