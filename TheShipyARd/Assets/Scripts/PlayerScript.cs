using UnityEngine;
using System.Collections;


public enum Target
{
    TARGET_A,
    TARGET_B,
    TARGET_C,
    TARGET_D,
    TARGET_E,
    TARGET_F,
    TARGET_G,
    TARGET_H,
    TARGET_I,
    TARGET_J,
    MAX_NUM_OF_TARGETS,
    UNKNOWN

}

public enum Tool
{
    HANDCUFF,
    CABLE_TIE,
    ROPE,
    MAX_NUM_OF_TOOLS,
    NONE
}

public class PlayerScript : MonoBehaviour {

    

    public float health = 100f;
    public int score = 0;

    public bool playerDead = false;
    public Target targetPlayer = Target.UNKNOWN;
    public Target playerID = Target.UNKNOWN;
    public Tool activeTool = Tool.NONE;

    // Use this for initialization
    void Start()
    {

        GameObject go = new GameObject();
        go.AddComponent<GUIText>();

        GUIText guiText = go.GetComponent<GUIText>();
        go.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
        guiText.text = "Target NR: " + (int)targetPlayer + " Player ID: " + (int)playerID;

        //requestTargetAndPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0f)
        {
            // Player currently dying
            if (!playerDead)
            {
                playerDying();
            }
            // Player is already dead
            else
            {
                playerIsDead();
            }
        }

    }

    void playerDying()
    {
        playerDead = true;
    }

    void playerIsDead()
    {

    }

    public void takeDamage(float amount)
    {
        health -= amount;
    }

    public void requestTargetAndPlayer()
    {
        NetworkManager.photonView.RPC("rpc_sendTargetAndPlayerToClient", PhotonTargets.MasterClient, PhotonNetwork.player);
    }

    [PunRPC]
    public void rpc_receiveTarget(Target target)
    {
        targetPlayer = target;
    }

    [PunRPC]
    public void rpc_reveivePlayer(Target player)
    {
        playerID = player;
    }
}
