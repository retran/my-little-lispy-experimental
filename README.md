damn-small-lisp-machine
========================

Overview
--------


Examples
--------

```scheme
(define (square x) (* x x))
	
(square 10)

(define constx 5)
(define consty 5)

(define (distance x y) (+ (square x) (square y)))
(distance 4 5)
(distance 2 3)

(distance constx consty)

(define (fact v) (if (< v 2) 1 (* v (fact (- v 1)))))
(fact 5)
(fact 10)            
```

Todo
----
