(define x 0)

(define (loop)
    (begin
      (p x)
      (set! 'x (+ x 1))
      (loop)))

(p (loop))
