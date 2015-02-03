(define (square x) (* x x))

(define (sum-of-squares x y)
  (+ (square x) (square y)))

(define (abs x)
  (cond ((> x 0) x)
	((= x 0) 0)
	((< x 0) (- 0 x))))

(define (sqrt-iter guess x)
  (if (good-enough? guess x)
      guess
    (sqrt-iter (improve guess x)
	       x)))

(define (improve guess x)
  (average guess (/ x guess)))

(define (average x y)
  (/ (+ x y) 2))

(define (good-enough? guess x)
  (< (abs (- (square guess) x)) 1))

(define (sqrt x)
  (sqrt-iter 1 x))

(p (sqrt 9))
(p (sqrt 25))
(p (sqrt 100))