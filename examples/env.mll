(cond
 (system-is-windows? (display-line "windows"))
 (system-is-linux? (display-line "linux"))
 (system-is-macos? (display-line "macos")))

(display-line my-little-lispy-runtime-version)
