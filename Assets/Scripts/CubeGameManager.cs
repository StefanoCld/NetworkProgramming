using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class CubeGameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Room stuff")]
    [SerializeField] private const string roomName = "CaldanaRicci's Room";
    [SerializeField] private const uint maxPlayersNum = 2;

    [Header("Cubes references")]
    [SerializeField] private const int smallCubesNumber = 900;
    [SerializeField] private GameObject smallCubesParent;
    [SerializeField] private GameObject bigCube;

    //Private stuff
    private PhotonView myPhotonView;

    private List<CubeState> cubeStates = new List<CubeState>(smallCubesNumber);

    private uint m_bytesSentThisSecond;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(bigCube.transform.position);
            stream.SendNext(bigCube.transform.rotation);

            uint childIndex = 0;
            foreach (Transform smallCube in smallCubesParent.transform)
            {
                if (smallCube.GetComponent<Rigidbody>().velocity != Vector3.zero)
                {
                    CubeState cubeState = new CubeState();
                    cubeState.CompressData(smallCube.position, smallCube.rotation, smallCube.GetComponent<SmallCube>().isInteracting, (ushort)childIndex);
                    cubeStates.Add(cubeState);
                    m_bytesSentThisSecond += CubeState.GetSize();
                }

                ++childIndex;
            }

            stream.SendNext(cubeStates.Count);
            foreach (CubeState cubeState in cubeStates)
            {
                stream.SendNext(cubeState);
            }
            cubeStates.Clear();
        }

        if (stream.IsReading)
        {

        }
    }

    void Start()
    {
        // 42 == Photon code for our custom type
        bool registrationResult = PhotonPeer.RegisterType(
            typeof(CubeState), 
            42, 
            CubeState.Serialize,
            CubeState.Deserialize);

        if (registrationResult)
        {
            Debug.LogError("Custom Type registration Error!");
        }

        PhotonNetwork.ConnectUsingSettings();

        myPhotonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        //Logica send-rate
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("OnConnectedToMaster() called!");

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        if (PhotonNetwork.IsMasterClient)
        {
            bigCube.GetComponent<Rigidbody>().isKinematic = false;
            bigCube.GetComponent<Rigidbody>().useGravity = true;
            bigCube.GetComponent<BigCubeController>().enabled = true;

            foreach(Transform smallCube in smallCubesParent.transform)
            {
                smallCube.GetComponent<Rigidbody>().isKinematic = false;
                smallCube.GetComponent<Rigidbody>().useGravity = true;
            }
        }
    }
}