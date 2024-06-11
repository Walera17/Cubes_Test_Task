using UnityEngine;
using UnityEngine.Events;

namespace Scripts
{
    public class Bullet : MonoBehaviour
    {
        public static event UnityAction<Cube> OnHitCube; 
        public static event UnityAction<Bullet> OnRelease; 

        [SerializeField] Rigidbody rb;
        [SerializeField] private float speed = 25f;

        void FixedUpdate()
        {
            rb.velocity = transform.forward * speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out Cube cube))
                OnHitCube?.Invoke(cube);

            OnRelease?.Invoke(this);
        }
    }
}