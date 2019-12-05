using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectTub
{
    public abstract class PoolableObject : MonoBehaviour
    {
        // InitializeForUse is called when the object is obtained for reuse
        // Example things to do in the implementation of this method:
        //      Initialize game stats like HP, stamina, etc
        //      Allocate resources the object needs, such as files to read from
        //      Reset the object's transform and rigidbody's velocity and angular velocity
        public abstract void InitializeForUse();

        // PutAway is called when the object will be deactivated and put back in the pool
        // Example things to do in the implementation of this method:
        //      Stop sound effects the object is playing
        //      Release allocated resources
        //      Stop particle effects
        public abstract void PutAway();
    }
}
