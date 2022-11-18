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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            WriteOnStream(stream);
        }

        if (stream.IsReading)
        {
            ReadFromStream(stream);
        }
    }
    private void WriteOnStream(PhotonStream stream)
    {
        stream.SendNext(bigCube.transform.position);
        stream.SendNext(bigCube.transform.rotation);

        m_bytesSentThisSecond += CubeState.GetSize();




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

    private void ReadFromStream(PhotonStream stream)
    {
        Vector3 bigCubeNewPos = (Vector3)stream.ReceiveNext();
        Quaternion bigCubeNewRot = (Quaternion)stream.ReceiveNext();

        InterpolateTransform itbc = bigCube.GetComponent<InterpolateTransform>();
        if (itbc)
        {
            itbc.lastPackagePosition = bigCubeNewPos;
            itbc.lastPackageRotation = bigCubeNewRot;
        }

        int receivedCubeStates = (int)stream.ReceiveNext();

        for (int i = 0; i < receivedCubeStates; ++i)
        {
            CubeState cubeState = (CubeState)stream.ReceiveNext();
            Transform smallCube = smallCubesParent.transform.GetChild(cubeState.index);

            Vector3 smallCubeNewPos = Vector3.zero;
            Quaternion smallCubeNewRot = Quaternion.identity;
            cubeState.DecompressData(ref smallCubeNewPos, ref smallCubeNewRot);

            InterpolateTransform itsc = smallCube.GetComponent<InterpolateTransform>();
            if (itsc)
            {
                itsc.lastPackagePosition = smallCubeNewPos;
                itsc.lastPackageRotation = smallCubeNewRot;
            }

            if (cubeState.isInteracting) smallCube.GetComponent<SmallCube>().Interact();
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

        if (!registrationResult)
        {
            Debug.LogError("Custom Type registration Error!");
        }

        Debug.Log("About to connect..");

        PhotonNetwork.ConnectUsingSettings();

        myPhotonView = GetComponent<PhotonView>();
    }

    float m_sendingTime = 0;
    uint m_bytesSentThisSecond = 0;

    void Update()
    {
        if (photonView.IsMine && Time.time >= m_sendingTime + 1)
        {
            Debug.Log(m_bytesSentThisSecond);
            m_sendingTime = Time.time;
            m_bytesSentThisSecond = 0;
        }

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

        Debug.Log("OnJoinedRoom() called!");

        Debug.Log("I am the MasterClient = " + PhotonNetwork.IsMasterClient);

        if (PhotonNetwork.IsMasterClient)
        {
            bigCube.GetComponent<Rigidbody>().isKinematic = false;
            bigCube.GetComponent<Rigidbody>().useGravity = true;
            bigCube.GetComponent<BigCubeController>().enabled = true;

            bigCube.GetComponent<InterpolateTransform>().IsMasterClient = true;

            foreach (Transform smallCube in smallCubesParent.transform)
            {
                smallCube.GetComponent<Rigidbody>().isKinematic = false;
                smallCube.GetComponent<Rigidbody>().useGravity = true;

                smallCube.GetComponent<InterpolateTransform>().IsMasterClient = true;
            }
        }
    }
}