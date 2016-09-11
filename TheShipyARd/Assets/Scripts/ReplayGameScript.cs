using UnityEngine;
using System.Collections;

public class ReplayGameScript : MonoBehaviour {

    public GameObject player;

    void Start()
    {
        player = GameObject.Find("Player");
    }

	public void replayGame ()
    {
        player.GetComponent<PhotonView>().RPC("rpc_restartGame", PhotonTargets.All);
    }
}
