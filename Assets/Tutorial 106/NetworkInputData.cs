using Fusion;
using UnityEngine;

namespace Tutorial_106
{
    public struct NetworkInputData : INetworkInput
    {
        public const byte MOUSE_BUTTON1 = 0x01;
        public const byte MOUSE_BUTTON2 = 0x02;

        public byte Buttons;
        public Vector3 Direction;
    }
}