using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace FL_KeyForward
{
    public partial class MainWindow : Form
    {

        string[] m_potentialProcessNames = { "FL32", "FL64", "FL Studio", "FL Studio 20", "FL Studio 21", "FL Studio 22" };

        Process m_flProcess;

        private bool m_isRunning = false;

        GlobalKeyboardHook m_hook;

        public MainWindow() {
            InitializeComponent();
            m_hook = new GlobalKeyboardHook();
            m_hook.KeyPressed += Hook_KeyPressed;
            m_hook.KeyUp += Hook_KeyUp;
            m_hook.HookKeyboard();
        }

        private void SetOff() {
            m_isRunning = false;
            button1.Text = "OFF";
        }

        private void SetOn() {
            m_isRunning = true;
            button1.Text = "ON";
        }

        private void FindProcess() {
            var processes = Process.GetProcesses();
            var potentialMatches = processes.Where(p => m_potentialProcessNames.Any(term => p.ProcessName.StartsWith(term, StringComparison.OrdinalIgnoreCase))).ToArray();
            if (potentialMatches.Length > 0) {
                m_flProcess = potentialMatches[0];
                label1.Text = "Status: Process Found: " + m_flProcess.MainWindowTitle;
          
            }
            else {
                label1.Text = "Status: Process Not Found";
                button1.Text = "OFF";
                button1.Enabled = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            FindProcess();
        }

        private void Hook_KeyPressed(object sender, GlobalKeyboardHook.KeyPressedEventArgs e) {
            if (m_isRunning) {
                var keyChar = GetCharFromKeys(e.KeyPressed);
                FlBridge.SendKeyDown(m_flProcess.MainWindowHandle, keyChar);
            }
        }

        private void Hook_KeyUp(object sender, GlobalKeyboardHook.KeyUpEventArgs e) {
            if (m_isRunning) {
                var keyChar = GetCharFromKeys(e.KeyUp);
                FlBridge.SendKeyUp(m_flProcess.MainWindowHandle, keyChar);
            }
        }

        public static char GetCharFromKeys(Keys keys) {
            int keyCode = (int)(keys & Keys.KeyCode);
            if (keyCode >= 32 && keyCode <= 126) {
                return (char)keyCode;
            }
            switch (keys) {
                case Keys.Enter:
                    return '\n';
                case Keys.Tab:
                    return '\t';
                default:
                    return '\0';
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (m_isRunning) {
                SetOff();
            }
            else {
                SetOn();
            }
        }
    }
}
