using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CardPile
{

    [System.Serializable]
    public class PileSettings
    {
        [SerializeField, Tooltip("Max nodes allowed for this Pile")]
        private int _maxNodes = 100;
        [SerializeField, Tooltip("Max curvature to segments of node allowed for this Pile")]
        private float _maxCurvature = 1;
        [SerializeField, Tooltip("Max spacing to segments of nodes allowed for this Pile")]
        private float _maxNodeSpacing = 100;
        [SerializeField, Tooltip("A offset rotation applied for eac node of segment")]
        private float _rotationOffset = 0;
        [SerializeField, Tooltip("Max screen width for node segments.")]
        private float _maxWidth = 800;
        [SerializeField, Tooltip("Offset position to origin from center of UI Panel")]
        private Vector2 _originOffset;


        /// <summary>
        /// Max nodes allowed for this Pile. Use this to limit the node quantity amount of the pile
        /// </summary>
        public int MaxNodes => _maxNodes;
        /// <summary>
        /// Max curvature to segments of nodes allowed for this Pile. Use this to limit the curvature amount of line segments
        /// </summary>
        public float MaxCurvature => _maxCurvature;
        /// <summary>
        /// Max spacing between each node of the segment. Use this to limit the spacing amount
        /// </summary>
        public float MaxNodeSpacing => _maxNodeSpacing;
        /// <summary>
        /// Mas screen width for node segments. This keep all segments inner this width.
        /// </summary>
        public float MaxWidth => _maxWidth;
        /// <summary>
        /// Offset position to origin from center of UI Panel
        /// </summary>
        public Vector2 OriginOffset => _originOffset;
        /// <summary>
        /// A offset rotation that can be applied for eac node of segment
        /// </summary>
        public float RotationOffset => _rotationOffset;
    }


}