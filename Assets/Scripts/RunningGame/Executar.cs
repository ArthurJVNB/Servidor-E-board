using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Executar : MonoBehaviour
{
    public Fade fade;
    public UnityEngine.UI.Image logo, bg;
    public static Sprite logoSprite, bgSprite;
    public static string path = null;
    public static int jogo_id = -1;
    public static System.Diagnostics.Process proc = new System.Diagnostics.Process();
    public static bool ended = true;
    public static IntPtr gameWindow = IntPtr.Zero;

    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

    const int ALT = 0xA4;
    const int EXTENDEDKEY = 0x1;
    const int KEYUP = 0x2;

    private void RefocusWindow()
    {
        // Simulate alt press
        keybd_event((byte)ALT, 0x45, EXTENDEDKEY | 0, 0);

        // Simulate alt release
        keybd_event((byte)ALT, 0x45, EXTENDEDKEY | KEYUP, 0);

        SetForegroundWindow(gameWindow);
    }

    // Start is called before the first frame update
    void Start()
    {
        logo.sprite = logoSprite;
        bg.sprite = bgSprite;

        if (ended)
        {
            if (path == null)
            {
                fade.FadeOut(true, "Hub");
                ended = true;
            }
            else
            {
                proc.StartInfo = new System.Diagnostics.ProcessStartInfo(path);
                proc.Start();
                ended = false;
            }
        }
        else
        {
            RefocusWindow();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (proc.HasExited && !ended)
        {
            fade.FadeIn(true, "Hub");
            ended = true;
            jogo_id = -1;
        }
    }
}
