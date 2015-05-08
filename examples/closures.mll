(define (make-adder n)
  (lambda (x)
    (+ x n)))

(define add1
  (make-adder 1))

(display-line (add1 10))

(define sub1
  (make-adder -1))

(display-line (sub1 10))
