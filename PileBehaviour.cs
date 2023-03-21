using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CardPile
{
    /// <summary>
    /// This component can be used to create segmented positions along a curved line.
    /// Each segment on the line is called a node, and you can use these nodes to know where to place the appearance of your cards.
    /// After implement this at your own component, you need update the nodes every a change occour. 
    /// Use <see cref="UpdatePile"/> to do it.
    /// 
    /// Keep at mind, that you doesnt need to update it every frame if not necessary. For Static piles like Poker Table cards (Flop, Preflop, Turn and River Cards), you dont need 
    /// update it every time.
    /// 
    /// For Pile that need updates like Hand Cards, you can implement some logic that update only if any changed occour, like a new card or a remove card, etc...
    /// </summary>
    public abstract class PileBehaviour : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _pilePanel;

        [SerializeField]
        private GameObject _visualNodePrefab;

        [SerializeField, Range(0, 1)]
        private float _nodesAmount = 0;

        [SerializeField, Range(0, 1)]
        private float _nodeSpacingAmount = 1;

        [SerializeField, Range(-1, 1)]
        private float _nodesCurvatureAmount = 0;

        [SerializeField]
        private PileSettings _settings;


        /// <summary>
        /// Current quantity of nodes. 
        /// </summary>
        public int nodes => Mathf.RoundToInt(_nodesAmount * _settings.MaxNodes);
        public PileSettings settings => _settings;

        /// <summary>
        /// UI rotation of all nodes at the moment. These rotations already added the offeset rotation
        /// </summary>
        private List<Quaternion> _cachedRotations = new List<Quaternion>();
        /// <summary>
        /// UI position of all nodes at the moment. These positions already added the offeset from <see cref="_cachedOffsets"/>
        /// </summary>
        private List<Vector2> _cachedPositions = new List<Vector2>();
        /// <summary>
        /// Individual offset position list, applied for each node form <see cref="_cachedPositions"/>.
        /// Use this to know the exact position offset. 
        /// </summary>
        private List<Vector2> _cachedOffsets = new List<Vector2>();
        /// <summary>
        /// List of Visual that represents world nodes. Each object is already in its final position, after the offset calculation is applied.
        /// <para>
        /// You can use these objects for various implementations. 
        /// - An example is: Knowing that each node is an object in the world, you can use the node as a parent of cards.This is a very quick way to ensure that the node's child card maintains correct position and rotation.
        /// </para>
        /// </summary>
        private List<GameObject> _nodeObjects = new List<GameObject>();

        /// <summary>
        /// This function ensures that the number of cached items equals the number of nodes currently being used. This prevents accessing invalid indexes.
        /// Run it before calculating the position and rotation of the nodes <see cref="EvalueNodes"/> to ensure that there are no wrong indexes
        /// </summary>
        private void RefreshCaches()
        {
            int changedNodes = nodes - _cachedPositions.Count;

            //Current node count is changed to ++
            if (changedNodes > 0)
            {
                //Lets go to add more items to lists
                for (int i = 0; i < changedNodes; i++)
                {
                    _cachedPositions.Add(transform.localPosition);
                    _cachedOffsets.Add(new Vector2());
                    _cachedRotations.Add(Quaternion.identity);
                    _nodeObjects.Add(Instantiate(_visualNodePrefab, _pilePanel));
                    EvalueNode(i);
                    OnNodeAdded(i);
                }
                return;
            }

            //Current node count is changed to --
            if (changedNodes < 0)
            {
                changedNodes *= -1;

                //Lets go to remove extra items from lists
                for (int i = 0; i < changedNodes; i++)
                {

                    int index = _cachedPositions.Count - 1;

                    OnNodeRemoving(index);

                    GameObject nodeObj = GetNodeObject(index);
                    _cachedPositions.RemoveAt(index);
                    _cachedOffsets.RemoveAt(index);
                    _cachedRotations.RemoveAt(index);
                    _nodeObjects.RemoveAt(index);
                    Destroy(nodeObj);
                    OnNodeRemoved(index);
                }
                return;
            }



        }
        /// <summary>
        /// Calculates the position and rotation of the nodes that are part of the segment. It also populates all cached lists with the correct information about positions, rotations, offsets etc.
        /// <para>
        /// After this evaluation, is safe to use Getters functions
        /// </para>
        /// </summary>
        private void EvalueNodes()
        {
            for (int i = 0; i < nodes; i++)
            {
                EvalueNode(i);
            }
        }
        /// <summary>
        /// Calculat the position and rotation this node. This correct position and rotation will be applied to this node at all cached list of this index
        /// </summary>
        /// <param name="index"></param>
        private void EvalueNode(int index)
        {

            //If only one node in the pile, will centralize this node keeping the correct settings about offsets
            if (index == 0 && nodes == 1)
            {
                _cachedPositions[0] = transform.localPosition + new Vector3(settings.OriginOffset.x + _cachedOffsets[0].x, settings.OriginOffset.y + _cachedOffsets[0].y, 0);
                _cachedRotations[0] = Quaternion.Euler(0, 0, settings.RotationOffset);
                _nodeObjects[0].transform.localPosition = _cachedPositions[0];
                _nodeObjects[0].transform.localRotation = _cachedRotations[0];
                return;
            }

            float _curvature = _settings.MaxCurvature * _nodesCurvatureAmount;
            float _nodeSpacing = _settings.MaxNodeSpacing * _nodeSpacingAmount;
            float _lineWidth = Mathf.Clamp(_nodeSpacing * nodes, _nodeSpacing, _settings.MaxWidth);
            float nodeDistance = _lineWidth / (nodes - 1);

            float xPos = -_lineWidth / 2 + index * nodeDistance;
            float yPos = ((_curvature * 0.001f) * xPos * xPos);

            _cachedPositions[index] = new Vector2(xPos, yPos);

            //Apply default offset to positions
            _cachedPositions[index] += _settings.OriginOffset;

            //Apply local Offset to positions
            _cachedPositions[index] += _cachedOffsets[index];

            //Apply local rotation to rotations
            float angle = xPos * 0.1f * _curvature;
            angle += (xPos * _settings.RotationOffset * 0.01f) * Mathf.Sign(_curvature);
            _cachedRotations[index] = Quaternion.Euler(0, 0, angle);

            //Aplly position, offset and rotation to object node
            _nodeObjects[index].transform.localPosition = _cachedPositions[index];
            _nodeObjects[index].transform.localRotation = _cachedRotations[index];


        }
        private bool IsValidIndex(int index)
        {
            return index >= 0 && index < _nodeObjects.Count;
        }
        /// <summary>
        /// Update all node positions and rotations.
        /// </summary>
        public void UpdatePile()
        {
            RefreshCaches();
            EvalueNodes();
        }

        public GameObject GetNodeObject(int index)
        {
            if (!IsValidIndex(index)) { return null; }

            return _nodeObjects[index];
        }

        /// <summary>
        /// This is Local Position of the node at <see cref="_pilePanel"/>, this position already offset applied. To know how many offset is applied, take a look at
        /// <see cref="GetNodePositionOffset(int)"/>
        /// </summary>
        /// <param name="index">Node index</param>
        /// <returns></returns>
        public Vector2 GetNodePosition(int index)
        {
            if (!IsValidIndex(index)) { return default; }
            return _cachedPositions[index];
        }
        /// <summary>
        /// This is the position offset applied exclusively to this node. Take a look at <see cref="SetNodePositionOffset(int, Vector2)"/>
        /// </summary>
        /// <param name="index">Node index</param>
        /// <returns></returns>
        public Vector2 GetNodePositionOffset(int index)
        {
            if (!IsValidIndex(index)) { return default; }
            return _cachedOffsets[index];
        }
        /// <summary>
        /// This is the current rotation of the this node, calculed automaticaly to keep a correct direction from line curvature.
        /// A offset roration can be applied to all nodes from <see cref="settings"/> and it will be computed in final rotation.
        /// </summary>
        /// <param name="index">Node index</param>
        /// <returns></returns>
        public Quaternion GetNodeRotation(int index)
        {
            if (!IsValidIndex(index)) { return default; }
            return _cachedRotations[index];
        }

        /// <summary>
        /// Set a offset position exclusively to this node
        /// <para>- remember to update the pile <see cref="UpdatePile"/>, if you are doing it manually</para>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="offs"></param>
        public void SetNodePositionOffset(int index, Vector2 offs)
        {
            if (!IsValidIndex(index)) { return; }
            _cachedOffsets[index] = offs;
        }

        /// <summary>
        /// Add a new node to pile if the current <see cref="nodes"/> is less <see cref="PileSettings.MaxNodes"/> 
        /// <para>- remember to update the pile <see cref="UpdatePile"/>, if you are doing it manually</para>
        /// </summary>
        public void AddNode()
        {
            _nodesAmount = Mathf.Clamp01(_nodesAmount + 1.00f / _settings.MaxNodes);
        }
        /// <summary>
        /// Remove a node from pile if exist.
        /// <para>- remember to update the pile <see cref="UpdatePile"/>, if you are doing it manually</para>
        /// </summary>
        public void RemoveNode()
        {
            _nodesAmount = Mathf.Clamp01(_nodesAmount - 1.00f / _settings.MaxNodes);

        }

        /// <summary>
        /// Fired when a new node has been added to the pile. At this moment, the node already has the correct position and rotation on the screen.
        /// </summary>
        /// <param name="index"></param>
        protected abstract void OnNodeAdded(int index);
        /// <summary>
        /// Fired before the node is actually removed from the stack.
        /// You can still use this index in this frame to receive information about position, rotation and object of this node in the pile
        /// </summary>
        /// <param name="index"></param>
        protected abstract void OnNodeRemoving(int index);
        /// <summary>
        /// Fired after the node has been effectively removed from the node pile. 
        /// At this time, this index cannot be used to retrieve any information from the old node, as it has already been
        /// removed. Use this index only to find where the node was removed from or make your self a cached list of nodes and their indexes.
        /// To intercept the removal event before commit, use <see cref="OnNodeRemoving(int)"/>
        /// </summary>
        /// <param name="index"></param>
        protected abstract void OnNodeRemoved(int index);

    }
}