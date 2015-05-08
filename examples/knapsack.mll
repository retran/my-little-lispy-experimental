(define (build-list n proc)
  (do ((i 0 (+ i 1)) (lst '()))
      ((= i n) lst)
    (set! lst (append lst (list (proc i))))))

(define (best a b est)
  (if (> (est a) (est b)) a b))

(define (item weight cost)
  (list weight cost #f))

(define (weight item)
  (car item))

(define (cost item)
  (car (cdr item)))

(define (sack size . items) 
  (cons size (cons 0 items)))

(define (sack-size sack)
  (car sack))

(define (sack-cost sack)
  (car (cdr sack)))

(define (items sack)
  (cdr (cdr sack)))

(define (nth-item sack index)
  (list-ref (items sack) index))

(define (place item)
  (list (weight item) (cost item) #t))

(define (sack-with-nth-item sack index)
  (let* ((l (length (items sack)))
         (placed-item (place (nth-item sack index)))
         (new-size (- (sack-size sack) (weight placed-item)))
         (new-cost (if (< new-size 0) 0 (+ (sack-cost sack) (cost placed-item)))))
    (cons new-size
          (cons new-cost
                (build-list l (lambda (i) (if (= i index) placed-item (nth-item sack i))))))))

(define (solve-impl sack index len)
  (if (and (>= (sack-size sack) 0) (< index len))
      (best (solve-impl (sack-with-nth-item sack index) (+ index 1) len)
            (solve-impl sack (+ index 1) len)
            sack-cost)
  sack))

(define (solve sack)
  (solve-impl sack 0 (length (items sack))))

(define test
 (sack 7
  (item 4 5)
  (item 3 2)
  (item 2 3)
  (item 3 4)
  (item 1 10)))

(display-line (solve test))
