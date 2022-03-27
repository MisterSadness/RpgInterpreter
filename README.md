# RpgInterpreter
Interpreter of a simple scripting language, written from scratch.

## Tech stack
Written in C# 9 (.NET 6.0). The only outside dependency is the [`Optional` library](https://github.com/nlkl/Optional), used primarily in the lexer part of the project,

## Implementation
The code is divided by interpretation phases, of which there are four:
- lexing, with lookahead 1,
- parsing, implemented as an RD parser,
- type checking
- execution

## Running the interpreter
You can just run
```
cd ./RpgInterpreter
dotnet run example.rpg
```
That should execute an example program, if you have all required packages and tools. Output of the `example.rpg` script should look something like this:
```
Marcus is ambushed by a bandit. A fight commences!
Marcus has 9 health
Otto the Highwayman has 9 health
Marcus lost 3 health.
Fight continues!
Otto the Highwayman has 9 health
Marcus has 6 health
Marcus lost 3 health.
Fight continues!
Otto the Highwayman has 9 health
Marcus has 3 health
Otto the Highwayman lost 4 health.
Fight continues!
Marcus has 3 health
Otto the Highwayman has 5 health
Marcus lost 3 health.
Marcus has died!
Otto the Highwayman has won!
```

## Tests
The interpreter is tested using both unit and e2e tests, which are located in the `RpgInterpreterTests` folder.
