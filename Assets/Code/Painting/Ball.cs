using System;
using UnityEngine;

namespace Code.Painting
{
    public class Ball : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private GameObject explosionPrefab;
        private bool isGhost;
        private int dynamicLevelLayer;

        public void Init(Vector3 velocity, bool isGhost) {
            dynamicLevelLayer = LayerMask.NameToLayer("DynamicLevel");
            this.isGhost = isGhost;
            rb.velocity = velocity;
        }

        public void OnCollisionEnter2D(Collision2D col) {
            if (isGhost) return;
            Instantiate(explosionPrefab, rb.transform.position, Quaternion.FromToRotation(transform.up, col.contacts[0].normal));
            
            if (col.gameObject.layer == dynamicLevelLayer)
            {
                var paintable = col.gameObject.GetComponent<Paintable>();
                if (paintable != null)
                {
                    paintable.onPaint(QueryableHitboxComponent.CalculateContactInformation(col.collider, transform.position));
                }
            }
            Destroy(gameObject);
        }
    }
}
