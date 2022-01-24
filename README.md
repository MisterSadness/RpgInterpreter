# RpgInterpreter
Kod źródłowy interpretera wykonanego na potrzeby przedmioty Metody Translacji.

## Technologia
Interpreter został napisany w języku C# 9 (framework .NET 6.0).

## Uruchomienie
Interpreter najprościej uruchomić
```
cd ./RpgInterpreter
dotnet run example.rpg
```
Polecenie to odpala interpreter na przykładowym pliku wejściowym. `example.rpg` można zastąpić ścieżką do dowolnego pliku wejściowego.

## Output
Obecnie program wypisuje jedynie wynik analizy leksykalnej, ciąg tokenów, z pominięciem tokenów odpowiadających białym znakom.

## Testy
Wszystkie testy zawarte są w projekcie `RpgInterpreterTests`, z konsoli można je uruchomić poleceniem:
```
dotnet test
```
