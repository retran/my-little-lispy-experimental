(define (factorial value)
	(if (< value 2)
	    1
	    (* value (factorial (- value 1)))))

(display-line (factorial 5))
(display-line (factorial 8))
(display-line (factorial 10))