# paskalON.Maths
Mathematical functions used by the services: curves, interpolation, exponential and logarithmic functions, sums, coordinates and random walkers.

Part of the [Common solution](../README.md).


## Namespaces


### Calculuses
The mathematical study of continuous change
- Differentials: concerns instantaneous rates of change, and the slopes of curves
- Integrals: concerns accumulation of quantities, and areas under or between curves

Sub namespaces: Coordinates, Curves, Exponents, Logarithmics, Sums.


### Randoms
Random walkers and random extensions, e.g. for simulations.


## Writing Mathematics in Plain Text
```
The square root of x is "x^(1 / 2)" (parentheses needed)
"e^x" can also be written "exp (x)"
Absolute value: |x|
Approximate(≈): =~              (key next to 1)
Factorial:      x!              (e.g. (n-2)! -> n! = n * x(n-1) *....)
Floor:          floor(x)        (the largest integer ≤ x)
Ceiling:        ceiling(x)      (the smallest integer ≥ x)
Division:       /               (remember the parentheses)
Integrals (∫):  integral()      (e.g. integral({a}{b}) -> {a} lower limit ∫ {b} upper limit )
Limits:         lim()           (e.g. lim({n-5}))
Multiplication: *               (remember the parentheses)
Pair:           (i,j)
Subscript:      _               (underscore)
Sum (∑):        sum()           (e.g. sum({i=0})^{n-1}) -> {i=0} buttom ∑ top {n-1})
Superscripts:   ^
Square Root:    sqrt(x)
Tau (τ)         tau()           (circle constant, equal to 2π = 6.283185307179586)
Two Dimension:  [x,y]
Vector:         vector{x}

Infinity:       'infinity'
Derivatives:    'derivatives'
```


## Repository Documentation
- [Repository README](/README.md)
- [Architecture](/Docs/02-Architect/Overview.md)
- [Design](/Docs/03-Design/Overview.md)
- [Development](/Docs/04-Develop/Overview.md)
- [Test](/Docs/05-Test/Overview.md)
- [Release](/Docs/06-Release/Overview.md)
