using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror
{
    public class ShipController : NetworkBehaviour
    {
        private Rigidbody rb;
        public ShipSettings settings;
        public float speed;

        public GameObject projectilePrefab;
        public Transform projectileMountRight;
        public Transform projectileMountLeft;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            // Don't run script on server
            if (!isLocalPlayer) return;

            // Rotation
            float horizontal = Input.GetAxis("Horizontal");
            transform.Rotate(0, horizontal * settings.rotationSpeed * Time.deltaTime, 0);

            // Movement
            if (Input.GetKey("w"))
            {
                transform.position -= transform.forward * Time.deltaTime * speed;
            }
   

            // Firing
            if (Input.GetKeyDown("e"))
            {
                CmdFire();
            }
            /*if (Input.GetKeyDown("q"))
            {
                CmdFire(projectileMountLeft);
            }*/
        }


        // this is called on the server
        [Command]
        void CmdFire()
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileMountRight.position, transform.rotation);
            NetworkServer.Spawn(projectile);
            RpcOnFire();
        }

        // this is called on the ship that fired for all observers
        [ClientRpc]
        void RpcOnFire()
        {
            //animator.SetTrigger("Shoot");
            //shot explotions spawn
        }
    }
}
