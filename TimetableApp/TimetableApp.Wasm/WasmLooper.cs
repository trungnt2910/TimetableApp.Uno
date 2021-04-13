using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uno.Foundation;

namespace TimetableApp.Wasm
{
    public class WasmLooper
    {
        public static EventHandler Loop;
        private static void OnLoop()
        {
            System.Diagnostics.Debug.WriteLine("Looping...");
            Loop?.Invoke(null, null);
        }
        public static void StartLoop(int milliseconds)
        {
            WebAssemblyRuntime.InvokeJS($"StartLoop({milliseconds})");
        }
    }
}
