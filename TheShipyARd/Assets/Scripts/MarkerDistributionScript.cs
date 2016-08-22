using UnityEngine;
using System.Collections;

public class MarkerDistributionScript : MonoBehaviour {

    private int[] weights;
    private int weightTotal;

    private Hashtable markerToPlayer;
    private Hashtable markerToPhotonID;
    private Hashtable markerToTool;

    public MarkerDistributionScript()
    {
        markerToPlayer = new Hashtable();
        markerToPhotonID = new Hashtable();
        markerToTool = new Hashtable();
    }

    public void generateToolDistribution()
    {
        weights = new int[4]; //number of things

        //weighting of each thing, high number means more occurrance
        weights[(int)Tool.HANDCUFFS] = 100;
        weights[(int)Tool.INJECTION] = 1;
        weights[(int)Tool.ROPE] = 200;

        weightTotal = 0;

        foreach (int w in weights)
        {
            weightTotal += w;
        }

        for (int i = 0; i < (int)Target.MAX_NUM_OF_TARGETS; i++)
        {
            markerToTool[i] = (Tool)RandomWeighted();
        }
    }

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

    public int getMarkerToPlayer(int index)
    {
        return (int)markerToPlayer[index];
    }

    public int getMarkerToPhotonID(int index)
    {
        return (int)markerToPhotonID[index];
    }

    public Tool getMarkerToTool(int index)
    {
        return (Tool)markerToTool[index];
    }

    public int getMarkerToPlayerCount()
    {
        return markerToPlayer.Count;
    }

    public int getMarkerToPhotonIDCount()
    {
        return markerToPhotonID.Count;
    }

    public int getMarkerToToolCount()
    {
        return markerToTool.Count;
    }

    public void setMarkerToPlayer(int markerID, int playerID)
    {
        markerToPlayer[markerID] = playerID;
    }

    public void setMarkerToPhotonID(int markerID, int photonID)
    {
        markerToPhotonID[markerID] = photonID;
    }

    public void setMarkerToTool(int markerID, Tool toolID)
    {
        markerToTool[markerID] = toolID;
    }
}
