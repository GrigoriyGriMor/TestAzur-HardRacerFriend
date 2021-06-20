using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

    public class ButtonManager : MonoBehaviour
    {
        private static ButtonManager instance;
        public static ButtonManager Instance => instance;

        [SerializeField] private float timeAlfa = 0.1f;
        [SerializeField] private Image leftButton;
        [SerializeField] private Image RightButton;

        public float moveAxisX = 0.0f;


        private Vector2 lastPointerPos;
        [HideInInspector] public bool drag = false;

        public void InputPosition(InputAction.CallbackContext cont)
        {
            if (cont.started)
                drag = true;
            else if (cont.canceled)
                drag = false;
        }

        private void Update()
        {
            switch (drag)
            {
                case true:
                {
                    Vector2 pointerPos = Mouse.current.position.ReadValue();

                    if (pointerPos.x < lastPointerPos.x)
                    {
                        moveAxisX = AxisMove(moveAxisX, -1);
                        leftButton.color = new Color(leftButton.color.r, leftButton.color.g, leftButton.color.b, Mathf.Abs(moveAxisX));
                    }
                    else
                    if (pointerPos.x > lastPointerPos.x)
                    {
                        moveAxisX = AxisMove(moveAxisX, 1);
                        RightButton.color = new Color(leftButton.color.r, leftButton.color.g, leftButton.color.b, Mathf.Abs(moveAxisX));
                    }
                    /*else //можно и этот вариант включить было, но он мне что-то не понравился
                    {
                        moveAxisX = 0;
                        leftButton.color = new Color(leftButton.color.r, leftButton.color.g, leftButton.color.b, 0.0f);
                        RightButton.color = new Color(leftButton.color.r, leftButton.color.g, leftButton.color.b, 0.0f);
                    }*/
                    
                    lastPointerPos = pointerPos;
                    break;
                }

                case false:
                    {
                        moveAxisX = 0;
                        leftButton.color = new Color(leftButton.color.r, leftButton.color.g, leftButton.color.b, 0.0f);
                        RightButton.color = new Color(leftButton.color.r, leftButton.color.g, leftButton.color.b, 0.0f);
                        break;
                    }
            }
        }



        private float AxisMove(float startAxis, int target)
        {
            if (startAxis < target && target != 0)
                startAxis += timeAlfa;
            else if (startAxis > target && target != 0)
                startAxis -= timeAlfa;

            return startAxis;
        }

        private void Awake()
        {
            instance = this;
        }
    }
