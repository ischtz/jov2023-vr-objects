using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Valve.VR;

namespace UXF
{
    /// <summary>
    /// Attach this component to a gameobject and assign it in the trackedObjects field in an ExperimentSession to automatically record position/rotation of the object at each frame.
    /// </summary>
    public class VRUserTracker : Tracker
    {
        public Transform head;
        public Transform leftController;
        public Transform rightController;

        public SteamVR_Action_Single squeezeAction;
        public SteamVR_Action_Boolean gripAction;
        public SteamVR_Action_Boolean triggerAction;

        public override string MeasurementDescriptor => "move";
        public override IEnumerable<string> CustomHeader => new string[] { "head_pos_x", "head_pos_y", "head_pos_z", "head_rot_x", "head_rot_y", "head_rot_z",
                                                                           "left_pos_x", "left_pos_y", "left_pos_z", "left_rot_x", "left_rot_y", "left_rot_z", 
                                                                           "right_pos_x", "right_pos_y", "right_pos_z", "right_rot_x", "right_rot_y", "right_rot_z",
                                                                           "grab_force", "grab", "trigger"};

        /// <summary>
        /// Returns current position and rotation values
        /// </summary>
        /// <returns></returns>
        protected override UXFDataRow GetCurrentValues()
        {

            Vector3 head_p = Vector3.zero;
            Vector3 head_r = Vector3.zero;
            Vector3 left_p = Vector3.zero;
            Vector3 left_r = Vector3.zero;
            Vector3 right_p = Vector3.zero;
            Vector3 right_r = Vector3.zero;

            if (head != null) {
                head_p = head.transform.position;
                head_r = head.transform.eulerAngles;                
            }

            if (leftController != null) {
                left_p = leftController.transform.position;
                left_r = leftController.transform.eulerAngles;                
            }

            if (rightController != null) {
                right_p = rightController.transform.position;
                right_r = rightController.transform.eulerAngles;                
            }

            var values = new UXFDataRow()
            {
                ("head_pos_x", head_p.x),
                ("head_pos_y", head_p.y),
                ("head_pos_z", head_p.z),
                ("head_rot_x", head_r.x),
                ("head_rot_y", head_r.y),
                ("head_rot_z", head_r.z),
                ("left_pos_x", left_p.x),
                ("left_pos_y", left_p.y),
                ("left_pos_z", left_p.z),
                ("left_rot_x", left_r.x),
                ("left_rot_y", left_r.y),
                ("left_rot_z", left_r.z),
                ("right_pos_x", right_p.x),
                ("right_pos_y", right_p.y),
                ("right_pos_z", right_p.z),
                ("right_rot_x", right_r.x),
                ("right_rot_y", right_r.y),
                ("right_rot_z", right_r.z),
                ("grab_force", squeezeAction.axis),
                ("grab", gripAction.state),
                ("trigger", triggerAction.state)
            };

            return values;
        }
    }
}