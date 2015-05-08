(defmacro main commands
  `(let ((c 1))
     (defmacro p (value) `(display-line ,value))
     ,@commands
     c))

(main
 (p 10)
 (p c))
