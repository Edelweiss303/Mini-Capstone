using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendController : MonoBehaviour
{
    public float MoveSpeed = 1.0f;
    public GameObject BulletPrefab;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveUpdate();
        ShootUpdate();
    }

    private void MoveUpdate()
    {
        Vector3 movementVector = new Vector3();

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movementVector.x--;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movementVector.x++;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            movementVector.z++;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            movementVector.z--;
        }
        movementVector *= MoveSpeed * Time.deltaTime;
        rb.velocity = new Vector3(movementVector.x, rb.velocity.y, movementVector.z );
    }

    private void ShootUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject temp = Instantiate(BulletPrefab, transform.position, Quaternion.identity, GunnerController.Instance.ProjectilesCollection.transform);
            MoveDirectionBehaviour temp2 = temp.GetComponentInChildren<MoveDirectionBehaviour>();

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                temp2.MoveDirection = new Vector3((hit.point.x - transform.position.x), 0, (hit.point.z - transform.position.z)).normalized;
            }
        }
    }
}
