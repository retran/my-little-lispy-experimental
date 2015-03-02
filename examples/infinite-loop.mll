(define (loop-impl a)
  (let ((b (+ a 1)))
    (begin
      (display b)
      (loop-impl b))))

(define (start-loop)
  (loop-impl 0))

(start-loop)


