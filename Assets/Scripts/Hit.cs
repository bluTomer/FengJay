using UnityEngine;

namespace Scripts
{
    public class Hit
    {
        public RaycastHit hit;

        public Hit(RaycastHit hit)
        {
            this.hit = hit;
        }
    }
}