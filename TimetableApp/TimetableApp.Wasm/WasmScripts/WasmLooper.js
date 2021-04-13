const loopMethod = Module.mono_bind_static_method('[TimetableApp.Wasm] TimetableApp.Wasm.WasmLooper:OnLoop');

function StartLoop(milliseconds) {
    setInterval(loopMethod, milliseconds);
}