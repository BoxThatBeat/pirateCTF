using System;
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
        public float cannonReloadSpeed;

        public GameObject projectilePrefab;
        public Transform projectileMountRight;
        public Transform projectileMountLeft;

        private Boolean canFireLeftCannon = true;
        private Boolean canFireRightCannon = true;

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
            if (Input.GetKeyDown("e") && canFireRightCannon)
            {
                CmdFireRight();

            }
            if (Input.GetKeyDown("q") && canFireLeftCannon)
            {
                CmdFireLeft();
            }
        }


        // this is called on the server
        [Command]
        void CmdFireRight()
        {
            StartCoroutine(BlockCannonFire(cannonReloadSpeed, Dir.right));
            GameObject projectile = Instantiate(projectilePrefab, projectileMountRight.position, transform.rotation);
            Projectile cannonBall = projectile.GetComponent<Projectile>();
            cannonBall.direction = Dir.right;

            NetworkServer.Spawn(projectile);
            RpcOnFire();
        }

        [Command]
        void CmdFireLeft()
        {
            StartCoroutine(BlockCannonFire(cannonReloadSpeed, Dir.left));
            GameObject projectile = Instantiate(projectilePrefab, projectileMountLeft.position, transform.rotation);
            Projectile cannonBall = projectile.GetComponent<Projectile>();
            cannonBall.direction = Dir.left;

            NetworkServer.Spawn(projectile);
            RpcOnFire();
        }

        private IEnumerator BlockCannonFire(float time, Dir direction)
        {
            if (direction == Dir.left)
            {
                canFireLeftCannon = false;
                yield return new WaitForSeconds(time);
                canFireLeftCannon = true;
            }
            else
            {
                canFireRightCannon = false;
                yield return new WaitForSeconds(time);
                canFireRightCannon = true;
            }
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
