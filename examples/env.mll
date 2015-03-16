(cond
 (system-is-windows? (display "windows"))
 (system-is-linux? (display "linux"))
 (system-is-macos? (display "macos")))

(display my-little-lispy-runtime-version)
