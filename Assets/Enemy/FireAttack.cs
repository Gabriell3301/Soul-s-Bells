using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAttack : MonoBehaviour
{
    public GameObject Projectile;
    [SerializeField] private Transform launchPoint;

    [SerializeField] private float fireInterval = 5f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AutoFire());
    }

    private void Launch()
    {
        GameObject BolaDeFogo = Instantiate(Projectile, launchPoint.position, launchPoint.rotation);
    }

    private IEnumerator AutoFire()
    {
        while (true)
        {
            Launch();
            yield return new WaitForSeconds(fireInterval);
        }
    }
}
