(define (factorial x acc)
  (if (= x 0)
      acc
      (factorial (- x 1) (* x acc))))

(factorial 100000 1)