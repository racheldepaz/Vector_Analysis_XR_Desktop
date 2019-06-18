using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace Magic
{
    public class MenuToggle : MonoBehaviour
    {
        #region Public Variables
        public Canvas canvas;
        public Camera camera;
        #endregion

        #region Private Variables
        private MLInputController _controller;

        [Space, SerializeField, Tooltip("ControllerConnectionHandler reference.")]
        private ControllerConnectionHandler _controllerConnectionHandler = null;
        #endregion

        #region Unity Methods
        // Start is called before the first frame update
        void Start()
        {
            MLInput.Start();
            _controller = MLInput.GetController(MLInput.Hand.Left);

        }

        void OnDestroy()
        {
            MLInput.Stop();
        }

        void Update()
        {
            updateTransform();
           // MLInput.OnControllerButtonDown += handleOnHomeButtonDown();
        }
        #endregion

        #region Private Methods
        private void updateTransform()
        {
            transform.position = _controller.Position;
            transform.rotation = _controller.Orientation;
        }

        private void handleOnHomeButtonDown(byte controllerID, MLInputController button)
        {
            //if (_controllerConnectionHandler.IsControllerValid(controllerId) && button == MLInputControllerButton.HomeTap)
            //{ }
        }

        #endregion

    }
}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                             