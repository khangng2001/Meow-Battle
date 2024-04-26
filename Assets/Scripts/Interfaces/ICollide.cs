using UnityEngine;

namespace Interfaces
{
    public interface ICollide
    {
        public void OnDestroyByBomb(GameObject owner);

        public void OnCollideByBullet(float timeFreeze, float pushForce, Vector3 directionPush);

        public void OnCollideByMelee(float timeStun, Vector3 pointHit, int strength);
    }

    
}
