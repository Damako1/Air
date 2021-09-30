using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInput : ScriptableObject, InputActions.IGamePlayActions
{
    

    public event UnityAction<Vector2> onMove=delegate { };
    public event UnityAction onStapMove = delegate { };
    public event UnityAction onFire = delegate { };
    public event UnityAction onStopFire = delegate { };
    public event UnityAction onDodge = delegate { };
    public event UnityAction onOverdrive = delegate { };


    InputActions inputActions;

    private void OnEnable()
    {
        inputActions = new InputActions();

        inputActions.GamePlay.SetCallbacks(this);
    }

    void OnDisable()
    {
        DisableAllInputs();
    }

    public void DisableAllInputs()
    {
        inputActions.GamePlay.Disable();
    }


    //激活gamePlay动作表
    public void EnableGameplayInput()
    {
        inputActions.GamePlay.Enable();

        //将鼠标隐藏
        //Cursor.visible = false;
        //将鼠标锁定
        //Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //InputActionPhase.Disabled  当动作表被禁用的时候
        //InputActionPhase.Waiting   当动作表被启用,但没有相应的输入信号传入时
        //InputActionPhase.Sarted    当按键被按下的那一帧,相当于Input.GetKeyDown() 
        //InputActionPhase.Performed 当输入动作已执行的时候,包含按下按键和按住按键的两个阶段,相当于Input.GetKey()
        //InputActionPhase.Canceled  当输入信号停止,也就输松开按键的那一帧，相当于Input.GetKeyUp()
        if (context.performed)
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }
        if (context.canceled)
        {
            onStapMove.Invoke();
        }

    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onFire.Invoke();
        }
        if (context.canceled)
        {
            onStopFire.Invoke();
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
            onDodge.Invoke();
        }
    }

    public void OnOverdrive(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onOverdrive.Invoke();
        }
    }
}
