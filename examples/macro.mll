(defmacro for-loop (start end body)
  `(let loop ((i ,start))
     (if (< i ,end)
         (begin
           ,@body
           (loop (+ i 1))))))

(for-loop 1 10 ((display "Hello") (display i)))

