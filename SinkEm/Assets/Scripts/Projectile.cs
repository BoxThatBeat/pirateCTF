using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Mirror
{
    public enum Dir { left, right }

    public class Projectile : NetworkBehaviour
    {
        private Rigidbody rb;
        public float destroyAfter = 5;
        public float force = 1000;

        public Dir direction;

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfter);
        }

        // set velocity for server and client. this way we don't have to sync the
        // position, because both the server and the client simulate it.
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (direction == Dir.right)
                rb.AddForce(-transform.right * force);
            else
                rb.AddForce(transform.right * force);
        }

        // destroy for everyone on the server
        [Server]
        void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        // ServerCallback because we don't want a warning if OnTriggerEnter is
        // called on the client
        [ServerCallback]
        void OnTriggerEnter(Collider co)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
