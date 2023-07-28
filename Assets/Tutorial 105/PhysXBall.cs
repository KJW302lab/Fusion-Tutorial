using Fusion;
using UnityEngine;

namespace Tutorial_105
{
    public class PhysXBall : NetworkBehaviour
    {
        [Networked] private TickTimer Life { get; set; }

        public void Init(Vector3 forward)
        {
            Life = TickTimer.CreateFromSeconds(Runner, 5.0f);
            GetComponent<Rigidbody>().velocity = Vector3.forward;
        }

        public override void FixedUpdateNetwork()
        {
            if (Life.Expired(Runner))
                Runner.Despawn(Object);
        }
    }
}
