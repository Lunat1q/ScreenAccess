using System;
using System.Collections.Generic;
using System.Linq;
using GlobalHook;
using GlobalHook.Event;

namespace TiqSoft.ScreenAssistant.Controllers.BindingControl
{
    public class KeyListener
    {
        public KeyListener()
        {
            this.BindingsKeyPress = new Dictionary<KeyEventArguments, Action>(new KeysComparer());
            this.BindingsKeyDown = new Dictionary<KeyEventArguments, Action>(new KeysComparer());
            this.BindingsKeyUp = new Dictionary<KeyEventArguments, Action>(new KeysComparer());
            this.BindingsMouseDown = new Dictionary<MouseButtons, Action>();
            this.BindingsMouseUp = new Dictionary<MouseButtons, Action>();
        }

        private Dictionary<MouseButtons, Action> BindingsMouseUp { get; }
        private Dictionary<MouseButtons, Action> BindingsMouseDown { get; }
        private Dictionary<KeyEventArguments, Action> BindingsKeyPress { get; }
        private Dictionary<KeyEventArguments, Action> BindingsKeyDown { get; }
        private Dictionary<KeyEventArguments, Action> BindingsKeyUp { get; }

        public void BindKeyPress(KeyModifier modifiers, char key, Action action)
        {
            this.BindingsKeyPress.Add(new KeyEventArguments{ KeyPressed =  key, Modifiers = modifiers }, action);
        }

        public void BindKeyDown(KeyModifier modifiers, char key, Action action)
        {
            this.BindingsKeyDown.Add(new KeyEventArguments{ KeyPressed =  key, Modifiers = modifiers }, action);
        }

        public void BindKeyUp(KeyModifier modifiers, char key, Action action)
        {
            this.BindingsKeyUp.Add(new KeyEventArguments{ KeyPressed =  key, Modifiers = modifiers }, action);
        }

        public void UnBindKeyUp(KeyModifier modifiers, char key)
        {
            this.BindingsKeyUp.Remove(new KeyEventArguments {KeyPressed = key, Modifiers = modifiers});
        }

        public void UnBindKeyUpAction(Action action)
        {
            var key = this.BindingsKeyUp.FirstOrDefault(x => x.Value == action).Key;
            if (key != null)
            {
                this.BindingsKeyUp.Remove(key);
            }
        }

        public void BindMouseDown(MouseButtons btn,Action action)
        {
            this.BindingsMouseDown.Add(btn, action);
        }

        public void BindMouseUp(MouseButtons btn, Action action)
        {
            this.BindingsMouseUp.Add(btn, action);
        }

        private void HandleEvent(KeyPressEvent eve)
        {
            if (eve.Keys == null) return;
            this.BindingsKeyPress.TryGetValue(eve.Keys, out var act);
            act?.Invoke();
        }

        private void HandleEvent(KeyDownEvent eve)
        {
            if (eve.Keys == null) return;
            this.BindingsKeyDown.TryGetValue(eve.Keys, out var act);
            act?.Invoke();
        }

        private void HandleEvent(KeyUpEvent eve)
        {
            if (eve.Keys == null) return;
            this.BindingsKeyUp.TryGetValue(eve.Keys, out var act);
            act?.Invoke();
        }

        private void HandleEvent(MouseDownEvent eve)
        {
            this.BindingsMouseDown.TryGetValue(eve.Button, out var act);
            act?.Invoke();
        }

        private void HandleEvent(MouseDownUp eve)
        {
            this.BindingsMouseUp.TryGetValue(eve.Button, out var act);
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
