using ObjectTub;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject BulletPrefab = default;
    [SerializeField] private Transform BulletSpawnPoint = default;

    private Vector2 rotation = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        if (BulletPrefab == null)
        {
            throw new MissingReferenceException("Player needs bullet prefab");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = (false);

        rotation.y += Input.GetAxis("Mouse X");
        rotation.x += -Input.GetAxis("Mouse Y");
        transform.eulerAngles = (Vector2)rotation * 5f;

        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = ObjectPool.TakeObjectFromTub(BulletPrefab);
            bullet.transform.position = BulletSpawnPoint ? BulletSpawnPoint.position : transform.position;
            bullet.GetComponent<Rigidbody>().AddForce(transform.forward * 800f);
        }
    }
}
