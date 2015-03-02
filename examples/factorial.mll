(define (factorial value)
	(if (< value 2)
	    1
	    (* value (factorial (- value 1)))))

(display (factorial 5))
(display (factorial 8))
(display (factorial 10))