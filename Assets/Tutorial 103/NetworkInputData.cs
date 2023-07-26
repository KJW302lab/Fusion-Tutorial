using Fusion;
using UnityEngine;

namespace Tutorial_103
{
    public struct NetworkInputData : INetworkInput
    {
        public Vector3 Direction;

        public const byte MOUSE_BUTTON1 = 0x01;
        public byte Buttons;
    }
}
