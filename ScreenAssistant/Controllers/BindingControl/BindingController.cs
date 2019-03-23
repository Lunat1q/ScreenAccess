using System;
using GlobalHook;
using GlobalHook.Event;

namespace TiqSoft.ScreenAssistant.Controllers.BindingControl
{
    internal class BindingController
    {
        private KeyboardHook Hook { get; set; }
        private KeyListener Listener { get; set; }

        public BindingController()
        {
            Hook = new KeyboardHook();
            Listener = new KeyListener();
            Hook.EventDispatcher.EventReceived += evt => Listener.Listen(evt);
        }

        public void BindPressToAction(KeyModifier modifiers, char key, Action action)
        {
            Listener.BindKeyPress(modifiers, key, action);
        }

        public void BindDownToAction(KeyModifier modifiers, char key, Action action)
        {
            Listener.BindKeyDown(modifiers, key, action);
        }

        public void BindUpToAction(KeyModifier modifiers, char key, Action action)
        {
            Listener.BindKeyUp(modifiers, key, action);
        }

        public void BindMouseDownToAction(MouseButtons btn, Action action)
        {
            Listener.BindMouseDown(btn, action);
        }

        public void BindMouseUpToAction(MouseButtons btn, Action action)
        {
            Listener.BindMouseUp(btn, action);
        }

        public void Start(bool keyBoardOnly = false)
        {
            Hook.Start();
            if (!keyBoardOnly)
            {
                
            }
        }

        public void Stop()
        {
            Hook.Stop();
        }
    }
}
