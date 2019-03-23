using System;
using System.Collections.Generic;
using GlobalHook;
using GlobalHook.Event;

namespace TiqSoft.ScreenAssistant.Controllers.BindingControl
{
    public class KeyListener
    {
        public KeyListener()
        {
            BindingsKeyPress = new Dictionary<KeyEventArguments, Action>(new KeysComparer());
            BindingsKeyDown = new Dictionary<KeyEventArguments, Action>(new KeysComparer());
            BindingsKeyUp = new Dictionary<KeyEventArguments, Action>(new KeysComparer());
            BindingsMouseDown = new Dictionary<MouseButtons, Action>();
            BindingsMouseUp = new Dictionary<MouseButtons, Action>();
        }

        private Dictionary<MouseButtons, Action> BindingsMouseUp { get; }
        private Dictionary<MouseButtons, Action> BindingsMouseDown { get; }
        private Dictionary<KeyEventArguments, Action> BindingsKeyPress { get; }
        private Dictionary<KeyEventArguments, Action> BindingsKeyDown { get; }
        private Dictionary<KeyEventArguments, Action> BindingsKeyUp { get; }

        public void BindKeyPress(KeyModifier modifiers, char key, Action action)
        {
            BindingsKeyPress.Add(new KeyEventArguments{ KeyPressed =  key, Modifiers = modifiers }, action);
        }

        public void BindKeyDown(KeyModifier modifiers, char key, Action action)
        {
            BindingsKeyDown.Add(new KeyEventArguments{ KeyPressed =  key, Modifiers = modifiers }, action);
        }

        public void BindKeyUp(KeyModifier modifiers, char key, Action action)
        {
            BindingsKeyUp.Add(new KeyEventArguments{ KeyPressed =  key, Modifiers = modifiers }, action);
        }

        public void BindMouseDown(MouseButtons btn,Action action)
        {
            BindingsMouseDown.Add(btn, action);
        }

        public void BindMouseUp(MouseButtons btn, Action action)
        {
            BindingsMouseUp.Add(btn, action);
        }

        private void HandleEvent(KeyPressEvent eve)
        {
            if (eve.Keys == null) return;
            BindingsKeyPress.TryGetValue(eve.Keys, out var act);
            act?.Invoke();
        }

        private void HandleEvent(KeyDownEvent eve)
        {
            if (eve.Keys == null) return;
            BindingsKeyDown.TryGetValue(eve.Keys, out var act);
            act?.Invoke();
        }

        private void HandleEvent(KeyUpEvent eve)
        {
            if (eve.Keys == null) return;
            BindingsKeyUp.TryGetValue(eve.Keys, out var act);
            act?.Invoke();
        }

        private void HandleEvent(MouseDownEvent eve)
        {
            BindingsMouseDown.TryGetValue(eve.Button, out var act);
            act?.Invoke();
        }

        private void HandleEvent(MouseDownUp eve)
        {
            BindingsMouseUp.TryGetValue(eve.Button, out var act);
            act?.Invoke();
        }

        public void Listen(IEvent evt)
        {
            try
            {
                dynamic eve = evt;
                HandleEvent(eve);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private class KeysComparer : IEqualityComparer<KeyEventArguments>
        {
            public bool Equals(KeyEventArguments x, KeyEventArguments y)
            {
                return x?.Modifiers == y?.Modifiers && x?.KeyPressed == y?.KeyPressed;
            }

            public int GetHashCode(KeyEventArguments obj)
            {
                return obj.Modifiers.GetHashCode() + obj.KeyPressed.GetHashCode()*10000;
            }
        }
    }
}
