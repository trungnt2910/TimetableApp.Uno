using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableApp
{
    public enum Platform
    {
        // Good old UWP
        UWP,
        // Skia heads
        WPF,
        GTK,
        Tizen,
        // Xamarin
        Android,
        iOS,
        macOS,
        // WASM
        WASM
    }
}
