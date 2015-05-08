(define (sum-all value)
  (do ((x 0 (+ x 1))
       (acc 0))
      ((> x value) acc)
    (set! acc (+ acc x))))

(display-line (sum-all 10))
(display-line (sum-all 100))
(display-line (sum-all 1000))
