(define (make-adder n)
  (lambda (x)
    (+ x n)))

(define add1
  (make-adder 1))

(display (add1 10))

(define sub1
  (make-adder -1))

(display (sub1 10))
