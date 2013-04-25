using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using White.Core.UIItems.Actions;
using White.Core.WindowsAPI;
using Action=White.Core.UIItems.Actions.Action;

namespace White.Core.InputDevices
{
    //BUG: KeysConverter
    /// <summary>
    /// Represents Keyboard attachment to the machine.
    /// </summary>
    public class Keyboard : IKeyboard
    {
        private static readonly List<KeyboardInput.SpecialKeys> scanCodeDependent = new List<KeyboardInput.SpecialKeys>
                                                                               {
                                                                                   KeyboardInput.SpecialKeys.RALT,
                                                                                   KeyboardInput.SpecialKeys.INSERT,
                                                                                   KeyboardInput.SpecialKeys.DELETE,
                                                                                   KeyboardInput.SpecialKeys.LEFT,
                                                                                   KeyboardInput.SpecialKeys.HOME,
                                                                                   KeyboardInput.SpecialKeys.END,
                                                                                   KeyboardInput.SpecialKeys.UP,
                                                                                   KeyboardInput.SpecialKeys.DOWN,
                                                                                   KeyboardInput.SpecialKeys.PAGEUP,
                                                                                   KeyboardInput.SpecialKeys.PAGEDOWN,
                                                                                   KeyboardInput.SpecialKeys.RIGHT,
                                                                                   KeyboardInput.SpecialKeys.LWIN,
                                                                                   KeyboardInput.SpecialKeys.RWIN
                                                                               };

        [DllImport("user32", EntryPoint = "SendInput")]
        private static extern int SendInput(uint numberOfInputs, ref Input input, int structSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        [DllImport("user32.dll")]
        private static extern ushort GetKeyState(uint virtKey);

        //http://msdn.microsoft.com/en-us/library/windows/desktop/ms646305(v=vs.85).aspx
        //http://msdn.microsoft.com/en-us/goglobal/bb896001.aspx
        [DllImport("user32.dll")]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint uFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        private static extern int GetKeyboardLayoutList(int size, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] hkls);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr ActivateKeyboardLayout(int hkl, uint uFlags);

        private readonly List<KeyboardInput.SpecialKeys> heldKeys = new List<KeyboardInput.SpecialKeys>();

        /// <summary>
        /// Use Window.Keyboard method to get handle to the Keyboard. Keyboard instance got using this method would not wait while the application
        /// is busy.
        /// </summary>
        public static readonly Keyboard Instance = new Keyboard();

        private readonly List<int> keysHeld = new List<int>();

        private Keyboard()
        {
        }

        /// <summary>
        /// Retrieves the active input locale identifier (formerly called the keyboard layout) for the specified thread. 
        /// If the idThread parameter is zero, the input locale identifier for the active thread is returned.
        /// </summary>
        /// <param name="idThread">The identifier of the thread to query, or 0 for the current thread.</param>
        /// <returns>The return value is the input locale identifier for the thread. 
        /// The low word contains a Language Identifier for the input language 
        /// and the high word contains a device handle to the physical layout of the keyboard.</returns>
        public virtual int GetActiveInputIdentifier(uint idThread)
        {
            IntPtr identifier = GetKeyboardLayout(idThread);
            return identifier.ToInt32();
        }

        // 
        /// <summary>
        /// Loads a new input locale identifier (formerly called the keyboard layout) into the system.
        /// This function only affects the layout for the current process or thread.
        /// </summary>
        /// <param name="identifierName">The name of the input locale identifier to load.</param>
        /// <param name="flags">Set flags to 1 when trying to change language. 
        /// Set flags to 0 when trying to reverse the change</param>
        /// <returns>The return value is the input locale identifier for the thread. 
        /// If no matching locale is available, the return value is NULL.</returns>
        public virtual int LoadInputLocaleIdentifier(string identifierName, uint flags)
        {
            IntPtr identifier = LoadKeyboardLayout(identifierName, flags);
            return identifier.ToInt32();
        }

        /// <summary>
        /// Sets the input locale identifier (formerly called the keyboard layout handle) for the calling thread or the current process. 
        /// The input locale identifier specifies a locale as well as the physical layout of the keyboard.
        /// The input locale identifier must have been loaded by a previous call to the LoadKeyboardLayout function. 
        /// This parameter must be either the handle to a keyboard layout or one of the following values.
        /// 1 - Selects the next locale identifier in the circular list of loaded locale identifiers maintained by the system.
        /// 0 - Selects the previous locale identifier in the circular list of loaded locale identifiers maintained by the system.
        /// </summary>
        /// <param name="identifierName">Input locale identifier to be activated.</param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public virtual int SetInputLocaleIdentifier(int identifier, uint flags)
        {
            //IntPtr iptr = Marshal.StringToHGlobalUni(identifierName);
            //IntPtr identifier = ActivateKeyboardLayout(iptr, flags);
            IntPtr previousIdentifier = ActivateKeyboardLayout(identifier, flags);
            return previousIdentifier.ToInt32();
        }

        public virtual int GetInputLocaleIdentifiers(ref int [] identifiers)
        {
            IntPtr[] hkls = new IntPtr[identifiers.Length];
            
            int count = GetKeyboardLayoutList(identifiers.Length, hkls);
            for (int i = 0; i < count; i++)
            {
                identifiers[i] = hkls[i].ToInt32();
            }
            return count;
        }

        public virtual void Enter(string keysToType)
        {
            Send(keysToType, new NullActionListener());
        }

        public virtual void Send(string keysToType, ActionListener actionListener)
        {
            if (heldKeys.Count > 0) keysToType = keysToType.ToLower();

            CapsLockOn = false;
            foreach (char c in keysToType)
            {
                short key = VkKeyScan(c);
                if (c.Equals('\r')) continue;

                if (ShiftKeyIsNeeded(key)) SendKeyDown((short) KeyboardInput.SpecialKeys.SHIFT, false);
                if (CtrlKeyIsNeeded(key)) SendKeyDown((short) KeyboardInput.SpecialKeys.CONTROL, false);
                if (AltKeyIsNeeded(key)) SendKeyDown((short) KeyboardInput.SpecialKeys.ALT, false);
                Press(key, false);
                if (ShiftKeyIsNeeded(key)) SendKeyUp((short) KeyboardInput.SpecialKeys.SHIFT, false);
                if (CtrlKeyIsNeeded(key)) SendKeyUp((short) KeyboardInput.SpecialKeys.CONTROL, false);
                if (AltKeyIsNeeded(key)) SendKeyUp((short) KeyboardInput.SpecialKeys.ALT, false);
            }

            actionListener.ActionPerformed(Action.WindowMessage);
        }

        public virtual void PressSpecialKey(KeyboardInput.SpecialKeys key)
        {
            PressSpecialKey(key, new NullActionListener());
        }

        public virtual void PressSpecialKey(KeyboardInput.SpecialKeys key, ActionListener actionListener)
        {
            Send(key, true);
            actionListener.ActionPerformed(Action.WindowMessage);
        }

        public virtual void HoldKey(KeyboardInput.SpecialKeys key)
        {
            HoldKey(key, new NullActionListener());
        }

        internal virtual void HoldKey(KeyboardInput.SpecialKeys key, ActionListener actionListener)
        {
            SendKeyDown((short) key, true);
            heldKeys.Add(key);
            actionListener.ActionPerformed(Action.WindowMessage);
        }

        public virtual void LeaveKey(KeyboardInput.SpecialKeys key)
        {
            LeaveKey(key, new NullActionListener());
        }

        public virtual void LeaveKey(KeyboardInput.SpecialKeys key, ActionListener actionListener)
        {
            SendKeyUp((short) key, true);
            heldKeys.Remove(key);
            actionListener.ActionPerformed(Action.WindowMessage);
        }

        private void Press(short key, bool specialKey)
        {
            SendKeyDown(key, specialKey);
            SendKeyUp(key, specialKey);
        }

        private void Send(KeyboardInput.SpecialKeys key, bool specialKey)
        {
            Press((short) key, specialKey);
        }

        private static bool ShiftKeyIsNeeded(short key)
        {
            return ((key >> 8) & 1) == 1;
        }

        private static bool CtrlKeyIsNeeded(short key)
        {
            return ((key >> 8) & 2) == 2;
        }

        private static bool AltKeyIsNeeded(short key)
        {
            return ((key >> 8) & 4) == 4;
        }

        private void SendKeyUp(short b, bool specialKey)
        {
            if (!keysHeld.Contains(b)) throw new InputDeviceException(string.Format("Cannot press the key {0} as its already pressed", b));
            keysHeld.Remove(b);
            KeyboardInput.KeyUpDown keyUpDown = GetSpecialKeyCode(specialKey, KeyboardInput.KeyUpDown.KEYEVENTF_KEYUP);
            SendInput(GetInputFor(b, keyUpDown));
        }

        private static KeyboardInput.KeyUpDown GetSpecialKeyCode(bool specialKey, KeyboardInput.KeyUpDown key)
        {
            if (specialKey && scanCodeDependent.Contains((KeyboardInput.SpecialKeys) key)) key |= KeyboardInput.KeyUpDown.KEYEVENTF_EXTENDEDKEY;
            return key;
        }

        private void SendKeyDown(short b, bool specialKey)
        {
            if (keysHeld.Contains(b)) throw new InputDeviceException(string.Format("Cannot press the key {0} as its already pressed", b));
            keysHeld.Add(b);
            KeyboardInput.KeyUpDown keyUpDown = GetSpecialKeyCode(specialKey, KeyboardInput.KeyUpDown.KEYEVENTF_KEYDOWN);
            SendInput(GetInputFor(b, keyUpDown));
        }

        private static void SendInput(Input input)
        {
            SendInput(1, ref input, Marshal.SizeOf(typeof (Input)));
        }

        private static Input GetInputFor(short character, KeyboardInput.KeyUpDown keyUpOrDown)
        {
            return InputFactory.Keyboard(new KeyboardInput(character, keyUpOrDown, GetMessageExtraInfo()));
        }

        public virtual bool CapsLockOn
        {
            get
            {
                ushort state = GetKeyState((uint) KeyboardInput.SpecialKeys.CAPS);
                return state != 0;
            }
            set { if (CapsLockOn != value) Send(KeyboardInput.SpecialKeys.CAPS, true); }
        }

        public virtual KeyboardInput.SpecialKeys[] HeldKeys
        {
            get { return heldKeys.ToArray(); }
        }

        public virtual void LeaveAllKeys()
        {
            new List<KeyboardInput.SpecialKeys>(heldKeys).ForEach(LeaveKey);
        }
    }
}