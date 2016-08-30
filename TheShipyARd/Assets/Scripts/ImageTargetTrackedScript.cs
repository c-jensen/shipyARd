using UnityEngine;
using System.Collections;

namespace Vuforia
{
    /// A custom handler that implements the ITrackableEventHandler interface.
    public class ImageTargetTrackedScript : MonoBehaviour,
                                                ITrackableEventHandler
    {
        #region PRIVATE_MEMBER_VARIABLES

        //Trackable Behaviour is to register the event handler
        private TrackableBehaviour mTrackableBehaviour;
        private PlayerScript playerScript = null;

        #endregion // PRIVATE_MEMBER_VARIABLES

        public PlayerReadyScript playerReadyScript;

        #region UNTIY_MONOBEHAVIOUR_METHODS

        void Start()
        {
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }
        }

        #endregion // UNTIY_MONOBEHAVIOUR_METHODS



        #region PUBLIC_METHODS

        /// <summary>
        /// Implementation of the ITrackableEventHandler function called when the
        /// tracking state changes.
        /// </summary>
        public void OnTrackableStateChanged(
                                        TrackableBehaviour.Status previousStatus,
                                        TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED)
            {
                //Marker detected
                OnTrackingFound();
            }
            else
            {
                //Marker lost
                OnTrackingLost();
            }
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS


        private void OnTrackingFound()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);


            // Enable rendering of the AR content:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = true;
            }

            // Enable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = true;
            }

            //Set the tracked target to the player
            //this can either be another player or a tool
            if (playerReadyScript.ready)
            {
                if (playerScript == null)
                {
                    GameObject player = GameObject.Find("Player");
                    playerScript = player.GetComponent<PlayerScript>();
                }


                //PLAYER MARKERS =========================================

                if (mTrackableBehaviour.TrackableName == "p0")
                {
                    playerScript.setTrackedTarget(0);
                }
                else if (mTrackableBehaviour.TrackableName == "p1")
                {
                    playerScript.setTrackedTarget(1);
                }
                else if (mTrackableBehaviour.TrackableName == "p2")
                {
                    playerScript.setTrackedTarget(2);
                }
                else if (mTrackableBehaviour.TrackableName == "p3")
                {
                    playerScript.setTrackedTarget(3);
                }
                else if (mTrackableBehaviour.TrackableName == "p4")
                {
                    playerScript.setTrackedTarget(4);
                }
                else if (mTrackableBehaviour.TrackableName == "p5")
                {
                    playerScript.setTrackedTarget(5);
                }
                else if (mTrackableBehaviour.TrackableName == "p6")
                {
                    playerScript.setTrackedTarget(6);
                }
                else if (mTrackableBehaviour.TrackableName == "p7")
                {
                    playerScript.setTrackedTarget(7);
                }
                else if (mTrackableBehaviour.TrackableName == "p8")
                {
                    playerScript.setTrackedTarget(8);
                }
                else if (mTrackableBehaviour.TrackableName == "p9")
                {
                    playerScript.setTrackedTarget(9);
                }

                //TOOL MARKERS =========================================

                else if (mTrackableBehaviour.TrackableName == "t0")
                {
                    playerScript.setTrackedToolMarker(0);
                }

                else if (mTrackableBehaviour.TrackableName == "t1")
                {
                    playerScript.setTrackedToolMarker(1);
                }
                else if (mTrackableBehaviour.TrackableName == "t2")
                {
                    playerScript.setTrackedToolMarker(2);
                }
                else if (mTrackableBehaviour.TrackableName == "t3")
                {
                    playerScript.setTrackedToolMarker(3);
                }
                else if (mTrackableBehaviour.TrackableName == "t4")
                {
                    playerScript.setTrackedToolMarker(4);
                }
                else if (mTrackableBehaviour.TrackableName == "t5")
                {
                    playerScript.setTrackedToolMarker(5);
                }
                else if (mTrackableBehaviour.TrackableName == "t6")
                {
                    playerScript.setTrackedToolMarker(6);
                }
                else if (mTrackableBehaviour.TrackableName == "t7")
                {
                    playerScript.setTrackedToolMarker(7);
                }
                else if (mTrackableBehaviour.TrackableName == "t8")
                {
                    playerScript.setTrackedToolMarker(8);
                }
                else if (mTrackableBehaviour.TrackableName == "t9")
                {
                    playerScript.setTrackedToolMarker(9);
                }
            }
        }


        private void OnTrackingLost()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Disable rendering of AR content if marker was lost:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = false;
            }

            // Disable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = false;
            }

            //also inform the player that he is not tracking the current marker anymore
            if (playerReadyScript.ready)
            {
                if (playerScript == null)
                {
                    GameObject player = GameObject.Find("Player");
                    playerScript = player.GetComponent<PlayerScript>();
                }

                //tracked target is unknown again
                playerScript.setTrackedTarget((int)Target.UNKNOWN);
                playerScript.setTrackedToolMarker(-1);
            }
        }
        #endregion // PRIVATE_METHODS
    }
}
