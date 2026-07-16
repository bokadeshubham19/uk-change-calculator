# UK Change Calculator

A .NET 10 console application that calculates the change owed to a customer and
breaks it down into the **minimum number of UK notes and coins**, largest
denomination first.

---

## Project Overview

Given an **amount paid** and a **product price**, the application:

1. Validates both inputs.
2. Computes `change = amountPaid − productPrice`.
3. Returns the change using the fewest possible UK denominations, largest first.

### Example

```
Enter amount paid:    £20
Enter product price:  £5.50

Your change is: (£14.50)
1 x £10
2 x £2
1 x 50p
```

### Behaviour rules

| Scenario                        | Result                                                        |
| ------------------------------- | ------------------------------------------------------------ |
| `AmountPaid > ProductPrice`     | Prints the denomination breakdown.                           |
| `AmountPaid == ProductPrice`    | Prints `No change.`                                          |
| `AmountPaid < ProductPrice`     | Prints an error with the shortfall and exits with code `1`.  |
| Non-numeric input               | Re-prompts until a valid number is entered.                  |
| Negative or sub-penny amount    | Prints a validation error and exits with code `1`.           |

### Supported denominations

**Notes:** £50, £20, £10, £5
**Coins:** £2, £1, 50p, 20p, 10p, 5p, 2p, 1p

---

## Project Structure

```
UkChangeCalculator.sln
│
├── UkChangeCalculator/                  # console app
│   ├── Program.cs                       # DI container setup + entry point
│   ├── ChangeApp.cs                     # console input/output (injected services)
│   ├── Models/
│   │   ├── Denomination.cs              # a note/coin (label + value in pence)
│   │   └── ChangeItem.cs               # one breakdown line (denomination + quantity)
│   ├── Services/
│   │   ├── IDenominationProvider.cs
│   │   ├── UkDenominationProvider.cs    # UK notes & coins, largest-first
│   │   ├── IChangeCalculator.cs
│   │   └── ChangeCalculator.cs          # the greedy algorithm
│   ├── Utilities/
│   │   └── InputValidator.cs            # input checks + change amount
│   └── Exceptions/
│       └── InsufficientPaymentException.cs
│
└── UkChangeCalculator.Tests/            # NUnit tests
    ├── ChangeCalculatorTests.cs
    └── InputValidatorTests.cs
```

> The layout follows a light **clean-architecture** separation even for a console
> app: `Program.cs` handles presentation only, `Services` hold the business logic
> behind interfaces, `Models` are plain data, and `Utilities` guard the inputs.

---

## Build Instructions

Requires the **.NET 10 SDK** (the projects target `net10.0`).

```bash
# From the repository root
dotnet build
```

---

## Run Instructions

```bash
dotnet run --project UkChangeCalculator
```

Then enter the amount paid and the product price when prompted. You can also pipe
input for non-interactive use:

```bash
printf '20\n5.50\n' | dotnet run --project UkChangeCalculator
```

### Run the tests

```bash
dotnet test
```

---

## Assumptions

- Inputs are in **pounds** using a decimal point (invariant culture), e.g. `5.50`.
- Currency precision is **one penny**; amounts with more than two decimal places
  (e.g. `5.555`) are rejected as invalid rather than silently rounded.
- The till is assumed to hold an **unlimited supply** of every denomination.
  (The design makes a limited-float variant easy to add — see below.)
- Amounts must be **non-negative**.
- The largest change handled comfortably fits within `decimal`; the internal
  pence value is a 64-bit integer, so values up to ~£92 quadrillion are safe.

---

## Algorithm Explanation

The change is broken down with a **greedy algorithm**:

1. Convert the change amount to an integer number of **pence** (all arithmetic is
   then done in integers, eliminating fractional-currency errors).
2. Iterate the denominations from **largest to smallest**.
3. For each denomination, take as many as fit: `quantity = remaining / value`,
   then reduce the balance: `remaining = remaining % value`.
4. Stop as soon as the remaining balance reaches zero.

Because the denomination set is fixed at construction and simply iterated, there
are **no per-denomination `if/else` branches** — adding or changing a
denomination is a data change, not a code change.

### Why the Greedy Algorithm works here

Greedy change-making is **only guaranteed optimal for a _canonical_ coin
system** — one where taking the largest coin that fits never forces a worse total
later. The UK denomination set (1, 2, 5, 10, 20, 50, 100, 200, 500, 1000, 2000,
5000 pence) **is canonical**, so the greedy choice at each step is provably part
of an optimal solution. Every denomination is a clean multiple/combination of the
smaller ones, so the modulo remainder can always be completed by the remaining
smaller denominations without waste. Hence greedy yields the **minimum number of
notes and coins**.

> For an arbitrary/non-canonical set (e.g. coins of 1, 3, 4) greedy can be
> sub-optimal and you would need dynamic programming. That is not the case for UK
> currency.

### Time Complexity

**O(d)** where `d` is the number of denominations (a small constant, 12 for UK
currency). Each denomination is visited exactly once with O(1) work, so runtime is
effectively **constant** and independent of the change amount.

### Space Complexity

**O(d)** in the worst case for the output list (at most one `ChangeItem` per
denomination). Auxiliary working space is **O(1)**.

---

## Why `decimal` over `double` for currency

- `double` is **binary** floating point: values like `0.1` and `5.50` cannot be
  represented exactly, so sums drift (e.g. `0.1 + 0.2 == 0.30000000000000004`).
  For money this produces wrong totals and rounding bugs.
- `decimal` is **base-10** floating point with 28–29 significant digits. It
  represents everyday monetary values **exactly** and rounds predictably.
- This application takes it one step further: after validation it converts pounds
  to **integer pence** and does the greedy division/modulo in `long`, so the core
  algorithm is entirely exact-integer arithmetic.

---

## SOLID Principles Used

- **S — Single Responsibility.** Each class has one reason to change:
  `InputValidator` validates, `ChangeCalculator` breaks down an amount,
  `UkDenominationProvider` supplies data, `ChangeApp` handles console I/O.
- **O — Open/Closed.** New behaviour is added by extension, not modification.
  A different currency or a **limited till float** is supported by writing a new
  `IDenominationProvider`; the calculator needs no changes. Denominations are
  data in a collection, not hard-coded branches.
- **L — Liskov Substitution.** Any `IDenominationProvider` /
  `IChangeCalculator` implementation can replace another without breaking
  callers; the calculator depends only on the interface contract.
- **I — Interface Segregation.** Interfaces are small and focused
  (`IDenominationProvider`, `IChangeCalculator`) — no client depends on methods it
  does not use.
- **D — Dependency Inversion.** Nothing `new`s up a service. Concrete types are
  registered against their interfaces in `Program.cs` using
  `Microsoft.Extensions.DependencyInjection`, and the container injects them via
  constructors: `ChangeCalculator` receives an `IDenominationProvider`, and
  `ChangeApp` receives an `IChangeCalculator`. This keeps the business logic
  decoupled from concrete implementations and easily unit-testable.

---

## Testing

NUnit tests (`dotnet test`) cover:

- The task's worked example and general normal scenarios.
- **Exact payment** (`No change`) and the empty breakdown for zero change.
- **Invalid payment** (amount paid < price) and negative/malformed amounts.
- **Decimal** values, **large** values (£1,234.56 → 24 × £50 …), and **small**
  values (1p / 2p / 5p).
- Edge cases: ordering is always largest-first, the breakdown is provably the
  minimum piece count, sub-penny rounding, and constructor guard clauses.
