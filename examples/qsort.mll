(define (qsort lst)
  (let ((l (length lst)))
    (if (< l 2)
        lst
        (let* ((pivot (car lst))
               (tail (cdr lst))
               (left (filter (lambda (v) (<= v pivot)) tail))
               (right (filter (lambda (v) (> v pivot)) tail)))
          (append (qsort left)
                  (append (list pivot) (qsort right)))))))

(display (qsort '(1 3 6 5 7 10 8 2)))
(display (qsort '(1 3 3 5 2 1 8 2 1 1 5)))
