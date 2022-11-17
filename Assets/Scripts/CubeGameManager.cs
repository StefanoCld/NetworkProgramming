using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CubeGameManager : MonoBehaviourPunCallbacks, IPunObservable
{

    [Header("Room stuff")]
    [SerializeField] private const string roomName = "CaldanaRicci's Room";
    [SerializeField] private const uint maxPlayersNum = 2;

    [Header("Cubes references")]
    [SerializeField] private const uint smallCubesNumber = 900;
    [SerializeField] private GameObject smallCubesParent;
    [SerializeField] private GameObject bigCube;

    //Private stuff
    private PhotonView myPhotonView;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // STO SCRIVENDO I DATI DA MANDARE VIA RETE PER QUESTO OGGETTO
        }
        if (stream.IsReading)
        {
            // HO RICEVUTO E STO LEGGENDO I DATI PER QUESTO OGGETTO
        }
    }

    void Start()
    {
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

        if (!(PhotonNetwork.IsMasterClient))
        {
            bigCube.GetComponent<Rigidbody>().isKinematic = true;
            bigCube.GetComponent<Rigidbody>().useGravity = false;
            bigCube.GetComponent<BigCubeController>().enabled = false;

            foreach(Transform smallCube in smallCubesParent.transform)
            {
                smallCube.GetComponent<Rigidbody>().isKinematic = true;
                smallCube.GetComponent<Rigidbody>().useGravity = false;
            }
        }
    }
}
