using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public GameObject sparkParticle;
    public GameObject whiteBulletToCoinPrefab;

    private bool activeBullet = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        activeBullet = true;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.CompareTag("TargetNumber") && activeBullet)
        {
            activeBullet= false;
            //hit a target, spawn a coin
            SoundManager.instance.PlayTargetHit();
            GameObject newSpark = Instantiate(sparkParticle, transform.position, sparkParticle.transform.rotation);
            TargetCtrl tCtrl = collision.gameObject.GetComponent<TargetCtrl>();
            tCtrl.SetUpCoinWithManager();
            ArcadeManager.instance.StartTargetSwitch();
            GameObject whiteBullet = Instantiate(whiteBulletToCoinPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

}
