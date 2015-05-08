(define (factorial-program)
  (define (f value)
	(if (< value 2)
	    1
	    (* value (f (- value 1)))))
  (display-line (f 10)))

(factorial-program)


