using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using ViveSR.anipal.Eye;

namespace UXF
{
    /// <summary>
    /// Attach this component to a gameobject and assign it in the trackedObjects field in an ExperimentSession to automatically record position/rotation of the object at each frame.
    /// </summary>
    public class ViveProEyeGazeTracker : Tracker
    {
        public float maxDistance = 10.0f;
        public LayerMask gazeLayerMask;
        public bool retrieveGazeTargetName = false;

        private FocusInfo FocusInfo;
        private static float MISSING = -99999.0f;

        public override string MeasurementDescriptor => "gaze";
        public override IEnumerable<string> CustomHeader => new string[] {"ori_x", "ori_y", "ori_z", "dir_x", "dir_y", "dir_z", "gaze_x", "gaze_y", "gaze_z", "gaze_object", "pupil_l", "pupil_r"};

        /// <summary>
        /// Returns current position and rotation values
        /// </summary>
        /// <returns></returns>
        protected override UXFDataRow GetCurrentValues()
        {
            // Get combined gaze vector
            Vector3 gazeOrigin = new Vector3();
            Vector3 gazeDir = new Vector3();
            SRanipal_Eye_v2.GetGazeRay(GazeIndex.COMBINE, out gazeOrigin, out gazeDir);

            // Get pupil size and other eye data
            VerboseData ed = new VerboseData();
            SRanipal_Eye_v2.GetVerboseData(out ed);

            // 3D gaze point via Raycast
            Vector3 gazePoint = Vector3.one * MISSING;
            Ray GazeRay;
            bool isFixating;
            string object_name = "";
            isFixating = SRanipal_Eye_v2.Focus(GazeIndex.COMBINE, out GazeRay, out FocusInfo, 0, maxDistance);
            if (isFixating) {
                gazePoint = FocusInfo.point;
                if (retrieveGazeTargetName) object_name = (string)FocusInfo.collider.gameObject.name;
            }

            // return position, rotation (x, y, z) as an array
            var values = new UXFDataRow()
            {
                ("ori_x", gazeOrigin.x),
                ("ori_y", gazeOrigin.y),
                ("ori_z", gazeOrigin.z),
                ("dir_x", gazeDir.x),
                ("dir_y", gazeDir.y),
                ("dir_z", gazeDir.z),
                ("gaze_x", gazePoint.x),
                ("gaze_y", gazePoint.y),
                ("gaze_z", gazePoint.z),
                ("gaze_object", object_name),
                ("pupil_l", ed.left.pupil_diameter_mm),
                ("pupil_r", ed.right.pupil_diameter_mm)
            };

            return values;
        }
    }
}