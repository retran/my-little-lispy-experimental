(let loop ((i 0))
  (if (< i 10)
      (begin
        (display-line i)
        (loop (+ i 1)))))
