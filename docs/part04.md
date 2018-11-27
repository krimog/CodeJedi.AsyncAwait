# Attendre et être attendu

[Retour au sommaire](./../README.md)

## Pouvoir être attendu

Certaines méthodes *peuvent être attendues* ou (si vous utilisez Visual Studio en anglais) sont *awaitable*.

![awaitable](./assets/intellisense-awaitable.png)

### Qu'est-ce que ça veut dire

```pseudo
instruction1
instruction2
instruction3
```

Lorsque l'on développe de manière synchrone, l'`instruction1` sera exécutée, puis l'`instruction2` et enfin l'`instruction3`. Cependant, si l'`instruction2` est exécutée sur un autre thread, l'`instruction3` sera potentiellement exécutée avant la fin de l'`instruction2`. Cependant, si l'on attend l'`instruction2` (sous réserve qu'elle puisse être attendue et qu'on puisse l'attendre), on garantit d'exécuter l'`instruction3` après la fin de l'exécution de l'`instruction2`.

### Peut-on m'attendre ?

Une méthode est *awaitable* si ces deux conditions sont respectées :

* La méthode renvoie une `Task`, `Task<T>` ou `ValueTask<T>` (il existe d'autres types dont on ne parlera pas).
* La tâche renvoyée est démarrée ou déjà finie.

> Visual Studio n'est pas capable de savoir si la tâche est démarrée ou non, donc affichera ou non la méthode comme *awaitable* seulement selon la première condition.

### Le suffixe

Par convention, le nom d'une méthode *awaitable* est suffixé par `Async`.

* `GetData()` → `GetDataAsync()`
* `DoSomeStuff(int value)` → `DoSomeStuffAsync(int value)`

Pour éviter de confondre, une méthode non *awaitable* ne finira pas son nom par `Async`.

> Ce suffixe au nom des méthodes est indépendant du mot-clé `async`.

## Pouvoir attendre

Maintenant qu'on sait qu'une méthode peut être attendue, il faut pouvoir l'attendre. Là, il existe deux moyens : le mauvais (quand on n'a pas le choix) et le bon.

### J'attends et je bloque

La mauvaise méthode c'est d'attendre de manière synchrone. En quoi est-ce mal ? Tout simplement parce qu'un thread qui attend est un thread non disponible le temps de l'attente (ce qui va freezer l'interface pour un client lourd ou limiter le nombre de requêtes traitables pour un site web).
Cela pose également un risque de deadlock, dont je parlerai ultérieurement.

On peut appeler la méthode `Wait()` ou la propriété `Result` sur une tâche afin de l'attendre de manière synchrone. En revanche, si une exception est levée dans la tâche, la stacktrace de l'exception ne prendra pas en compte la méthode appelante.

Pour la prendre en compte, voici la façon de faire :

* `var myResult = MyAwaitableMethodAsync().GetAwaiter().GetResult();`

Cette méthode marche aussi bien pour les `Task<T>` et `ValueTask<T>`, auxquels cas cela renverra une valeur de type `T`, que pour les `Task`, pour lesquelles le type de retour de `GetResult()` est `void`.

### Je reviens quand c'est fini

Ce qu'on aimerait, ce serait, en libérant le thread courant, exécuter une tâche sur un autre thread, puis revenir sur le thread précédent afin d'exécuter l'instruction suivante.

C'est exactement ce à quoi sert le mot-clé `await` (enfin !).

```csharp
ExecuteMyFirstInstruction();
await ExecuteMySecondInstructionAsync();
ExecuteMyThirdInstruction();
```

La première instruction sera exécutée de manière synchrone sur le thread appelant (qu'on va appeler T0), puis T0 sera libéré (et donc disponible pour autre chose) le temps de l'exécution de la deuxième instruction. Enfin, une fois la deuxième instruction terminée, on retournera sur T0 afin d'exécuter la troisième instruction.

Cependant, ce mot-clé `await` nécessite la présente du mot-clé `async`.

[Suite](./part05.md)