using UnityEngine;
using Unity.Netcode;

public class PlayerData : NetworkBehaviour
{
    // [SerializeField] private PlayerController player;
    [SerializeField] private float _cheapInterpolationTime = 5.0f;

    struct PlayerNetworkData : INetworkSerializable
    {
        private float _x, _y;
        private short _yRot;

        internal Vector2 Position
        {
            get => new Vector2(_x, _y);
            set
            {
                _x = value.x;
                _y = value.y;
            }
        }

        internal short YRot
        {
            get => _yRot;
            set => _yRot = value;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue<float>(ref _x);
            serializer.SerializeValue<float>(ref _y);

            serializer.SerializeValue<short>(ref _yRot);
        }
    }

    private readonly NetworkVariable<PlayerNetworkData> _netState = new NetworkVariable<PlayerNetworkData>(writePerm: NetworkVariableWritePermission.Owner);


    private void Update()
    {
        if (IsOwner) TransmitData();
        else ConsumeData();
    }

    private void TransmitData()
    {
        _netState.Value = new PlayerNetworkData
        {
            Position = transform.position,
            YRot = (short)transform.rotation.y
        };
    }

    private void ConsumeData()
    {
        transform.position = Vector2.Lerp(transform.position, _netState.Value.Position, _cheapInterpolationTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(transform.rotation.x, _netState.Value.YRot, transform.rotation.z, transform.rotation.w), _cheapInterpolationTime);
    }

}
