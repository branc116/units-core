# Units core

![Tests](https://github.com/branc116/units-core/workflows/Tests/badge.svg)

Define your units and use them to check your code for physical correctnes.

### Define your own units
``` units.txt ```
```go
Units(Base) := Mass | Length | Time
Units(Prefixes) := (mili, i/1000) | (micro, i/10e-6) | (kilo, i*1000) | (mega, i*1e6)
Operators(Binary) := (*, Times, 1, 1) | (/, Per, 1, -1)
Operators(Unary) := (Square, 2) | (SquareRoot, 0.5)
Operators(Self) := (<, Lt, bool) | (>, Gt, bool) | (==, Eq, bool) | (+, Plus, null) | (-, Minus, null) | (*, Times, null) | (/, Per, null)

Real(Types) := (System.Single, RealFloat) | (Godot.Vector3, Vec3)

Operator(*) := a = b * c | a = c * b | c = a / b | b = a / c
Operator(/) := a = b / c | c = b / a | b = a * c | b = c * a
Operator(Square) := a = Square b | b = SquareRoot a | a = b * b
Operator(SquareRoot) := a = SquareRoot b | b = Square a | b = a * a

Speed := Length / Time
Acceleration := Speed / Time
Area := Length * Length
Volume := Area * Length
Force := Mass * Acceleration
Pressure := Force / Area
Energy := Force * Length

!Infer

Unit(Mass) := gram(i, g)
Unit(Length) := meter(i, m)
Unit(Time) := second(i, s) | hour(i/3600, h) | minute(i/60, min)
Unit(Temperature) := kelvin(i, K) | celsius(i + 273, ˙C)
Unit(Force) := newton(i, N)

!Export Units ./Units.cs
!Export RealWrapers ./Wrapers.cs
```

### Generate cs types

* Using cli of the units-core tool ```> units-core ./units.txt ```
* Or Run Sample project ```> dotnet run Units.Core.Sample ```

### Use generated struct to help you keep track of the units you defined

[![useCase](https://i.postimg.cc/cJRzKVHq/Units-Show-Case-1.gif)](https://www.youtube.com/embed/ZIIJ2v_PZC0)

### Comparison with [UnitsNet](https://github.com/angularsen/UnitsNet)

UnitsNet is good as long as you are just using the units they provide.
The goal of this project is to permit user to define units that self wants to use.
