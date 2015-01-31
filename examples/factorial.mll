(define (factorial value)
	(if (< value 2)
	    1
	    (* value (factorial (- value 1)))))

(p (factorial 5))
(p (factorial 8))
(p (factorial 10))
