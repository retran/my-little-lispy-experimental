(define (sum-all value)
  (do ((x 0 (+ x 1))
       (acc 0))
      ((> x value) acc)
    (set! acc (+ acc x))))

(display (sum-all 10))
(display (sum-all 100))
(display (sum-all 1000))
