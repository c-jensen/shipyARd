using UnityEngine;
using System.Collections;

//enum for the different tool types
public enum Tool
{
    HANDCUFFS,
    INJECTION,
    ROPE,
    MAX_NUM_OF_TOOLS,
    NONE
}

public class PlayerToolScript : MonoBehaviour
{

    //the effectiveness of the different tools
    //declares how fast the target will be jailed
    private const float HANDCUFFS_DAMAGE = 20.0f;
    private const float INJECTION_DAMAGE = 100.0f;
    private const float ROPE_DAMAGE = 10.0f;

    private float toolDamage;

    private Tool toolType;

    public PlayerToolScript(Tool playerToolType)
    {

        //depending on type of tool, another effectiveness 
        if (playerToolType == Tool.NONE)
        {
            toolDamage = 0.0f;
            toolType = Tool.NONE;
        }

        else if (playerToolType == Tool.HANDCUFFS)
        {
            toolDamage = HANDCUFFS_DAMAGE;
            toolType = Tool.HANDCUFFS;
        }
        else if (playerToolType == Tool.INJECTION)
        {
            toolDamage = INJECTION_DAMAGE;
            toolType = Tool.INJECTION;
        }
        else if (playerToolType == Tool.ROPE)
        {
            toolDamage = ROPE_DAMAGE;
            toolType = Tool.ROPE;
        }
    }

    //getter for tool effectiveness
    public float getToolDamage()
    {
        return toolDamage;
    }

    //getter for tool type
    public Tool getToolType()
    {
        return toolType;
    }
}
