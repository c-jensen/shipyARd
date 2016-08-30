using UnityEngine;
using System.Collections;

public class MarkerDistributionScript : MonoBehaviour
{

    private int[] weights;
    private int weightTotal;

    //These hashtables safe the coherence between the markerID and the player ID / photon network ID
    //If you have the markerID you can find out the player or network ID with no effort
    private Hashtable markerToPlayer;
    private Hashtable markerToPhotonID;
    private Hashtable markerToTool;

    public MarkerDistributionScript()
    {
        //init hastables
        markerToPlayer = new Hashtable();
        markerToPhotonID = new Hashtable();
        markerToTool = new Hashtable();
    }

    //this method distributes the tools to the markers
    //each tool has a weight depending on the likelihood, that it will spawn
    //Rope is the most common, then handcuffs and injection is the rarest
    //Depending on rarity, the items are more powerful
    public void generateToolDistribution()
    {
        weights = new int[3];

        //weighting of each tool, high number means more occurrance
        weights[(int)Tool.HANDCUFFS] = 100;
        weights[(int)Tool.INJECTION] = 10;
        weights[(int)Tool.ROPE] = 200;

        weightTotal = 0;

        foreach (int w in weights)
        {
            weightTotal += w;
        }

        //populate the tool markers
        for (int i = 0; i < (int)Target.MAX_NUM_OF_TARGETS; i++)
        {
            markerToTool[i] = (Tool)RandomWeighted();
        }
    }

    //weighted random function, choosing a tool for the current marker
    private int RandomWeighted()
    {
        int result = 0, total = 0;
        int randVal = Random.Range(0, weightTotal + 1);
        for (result = 0; result < weights.Length; result++)
        {
            total += weights[result];
            if (total >= randVal) break;
        }
        return result;
    }

    //returns the playerID belonging to the markerID
    public int getMarkerToPlayer(int index)
    {
        return (int)markerToPlayer[index];
    }

    //returns the photon belonging to the markerID
    public int getMarkerToPhotonID(int index)
    {
        return (int)markerToPhotonID[index];
    }

    //returns the tool belonging to the markerId
    public Tool getMarkerToTool(int index)
    {
        return (Tool)markerToTool[index];
    }

    //returns the size of Marker to player relations
    public int getMarkerToPlayerCount()
    {
        return markerToPlayer.Count;
    }

    //returns the size of Marker to photon id relations
    public int getMarkerToPhotonIDCount()
    {
        return markerToPhotonID.Count;
    }

    //returns the size of Marker to tool id relations
    public int getMarkerToToolCount()
    {
        return markerToTool.Count;
    }

    //adds a markerID to playerID relation
    public void setMarkerToPlayer(int markerID, int playerID)
    {
        markerToPlayer[markerID] = playerID;
    }

    //adds a markerID to photon relation
    public void setMarkerToPhotonID(int markerID, int photonID)
    {
        markerToPhotonID[markerID] = photonID;
    }

    //adds a markerID to tool relation
    public void setMarkerToTool(int markerID, Tool toolID)
    {
        markerToTool[markerID] = toolID;
    }
}
