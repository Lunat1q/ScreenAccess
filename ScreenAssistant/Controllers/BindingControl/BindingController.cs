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
            this.Hook = new KeyboardHook();
            this.Listener = new KeyListener();
            this.Hook.EventDispatcher.EventReceived += evt => this.Listener.Listen(evt);
        }

        public void BindPressToAction(KeyModifier modifiers, char key, Action action)
        {
            this.Listener.BindKeyPress(modifiers, key, action);
        }

        public void BindDownToAction(KeyModifier modifiers, char key, Action action)
        {
            this.Listener.BindKeyDown(modifiers, key, action);
        }

        public void BindUpToAction(KeyModifier modifiers, char key, Action action)
        {
            this.Listener.BindKeyUp(modifiers, key, action);
        }

        public void UnBindKeyUp(KeyModifier modifiers, char key)
        {
            this.Listener.UnBindKeyUp(modifiers, key);
        }
        public void UnBindKeyUpAction(Action action)
        {
            this.Listener.UnBindKeyUpAction(action);
        }

        public void BindMouseDownToAction(MouseButtons btn, Action action)
        {
            this.Listener.BindMouseDown(btn, action);
        }

        public void BindMouseUpToAction(MouseButtons btn, Action action)
        {
            this.Listener.BindMouseUp(btn, action);
        }

        public void Start(bool keyBoardOnly = false)
        {
            this.Hook.Start();
            if (!keyBoardOnly)
            {
                
            }
        }

        public void Stop()
        {
            this.Hook.Stop();
        }
    }
}
