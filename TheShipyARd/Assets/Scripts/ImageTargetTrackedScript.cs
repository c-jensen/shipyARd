using UnityEngine;
using System.Collections;

namespace Vuforia
{
    /// <summary>
    /// A custom handler that implements the ITrackableEventHandler interface.
    /// </summary>
    public class ImageTargetTrackedScript : MonoBehaviour,
                                                ITrackableEventHandler
    {
        #region PRIVATE_MEMBER_VARIABLES

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
                newStatus == TrackableBehaviour.Status.TRACKED) //||
                                                                //newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS


        private void OnTrackingFound()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);


            // Enable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = true;
            }

            // Enable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = true;
            }

            if (playerReadyScript.ready)
            {
                if (playerScript == null)
                {
                    GameObject player = GameObject.Find("Player");
                    playerScript = player.GetComponent<PlayerScript>();
                }
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
            }
        }


        private void OnTrackingLost()
        {
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Disable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = false;
            }

            // Disable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = false;
            }

            if (playerReadyScript.ready)
            {
                if (playerScript == null)
                {
                    GameObject player = GameObject.Find("Player");
                    playerScript = player.GetComponent<PlayerScript>();
                }
                playerScript.setTrackedTarget((int)Target.UNKNOWN);
                playerScript.setTrackedToolMarker(-1);
            }
        }
        #endregion // PRIVATE_METHODS
    }
}
