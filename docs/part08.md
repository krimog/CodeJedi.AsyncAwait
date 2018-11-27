# `async`, `await` et parallélisme

Jusque là, on a utilisé `async` et `await` pour effectuer des traitements asynchrones. Mais il est également possible d'effectuer simplement du parallélisme avec ces mots-clés.

## `Task.WhenAll`

Comme on l'a vu, une méthode *awaitable* renvoie une tâche commencée, et `await` permet d'attendre la fin de la tâche avant d'aller à la ligne suivante.

```csharp
var result1 = await MyMethod1Async();
var result2 = await MyMethod2Async();
var result3 = await MyMethod3Async();
```

Cependant, rien n'oblige à faire le `await` à la même ligne que l'appel à la méthode. On peut donc appeler une autre méthode asynchrone sans avoir attendu le résultat de la première, puis attendre le résultat de chacune des deux (ou plus) tâches.

```csharp
var task1 = MyMethod1Async();
// On démarre la tâche 2 sans attendre la fin de la tâche 1
var task2 = MyMethod2Async();
// On démarre la tâche 3 sans attendre la fin de la tâche 1 ou de la tâche 2
var task3 = MyMethod3Async();
```

Il existe ensuite plusieurs façons d'attendre la fin de ces tâches et d'obtenir leurs résultats.

```csharp
var result1 = await task1;
var result2 = await task2;
var result3 = await task3;
```

```csharp
await task1;
await task2;
await task3;
var result1 = task1.Result;
var result2 = task2.Result;
var result3 = task3.Result;
```

```csharp
await Task.WhenAll(task1, task2, task3); // On peut fournir soit les tâches les unes après les autres
// soit fournir une collection
var result1 = task1.Result;
var result2 = task2.Result;
var result3 = task3.Result;
```

```csharp
var results = await Task.WhenAll(task1, task2, task3);
var result1 = results[0];
var result2 = results[1];
var result3 = results[2];
```

> Attention :
>
> `Task.WhenAll` s'arrête à la première exception levée au sein d'une des tâches.

## `Task.WhenAny`

Et si j'ai plein de tâches à lancer et que je veux voir l'avancement ?

Là, c'est un peu plus complexe. Il faut toutes les démarrer, puis attendre que n'importe laquelle d'entre elles soit finie, faire une action, puis attendre que n'importe laquelle des **autres** tâches soit finie et ainsi de suite.

Commençons par commencer toutes nos tâches comme précédemment :

```csharp
var task1 = MyMethod1Async();
// On démarre la tâche 2 sans attendre la fin de la tâche 1
var task2 = MyMethod2Async();
// On démarre la tâche 3 sans attendre la fin de la tâche 1 ou de la tâche 2
var task3 = MyMethod3Async();
```

Ensuite, on va sauvegarder ces tâches dans une collection, de laquelle on va retirer une tâche quand elle sera terminée. Le type de collection le plus approprié pour ça est donc `HashSet<T>` puisque cette collection permet de retirer un élément donné avec une complexité *O(1)*, alors qu'une liste va avoir une complexité *O(n)* pour la même action.

```csharp
var runningTasks = new HashSet<Task<int>>();
runningTasks.Add(task1);
runningTasks.Add(task2);
runningTasks.Add(task3);
```

Vient alors la boucle dans laquelle on fait une action dès qu'une tâche est terminée.

```csharp
while (runningTasks.Any())
{
    var completedTask = await Task.WhenAny(runningTasks));

    // Puisque la tâche est terminée, on la retire de runningTasks
    runningTasks.Remove(completedTask);
    var taskResult = completedTask.Result;
    // Là, on peut utiliser le résultat comme on le souhaite
    // On peut également utiliser le nombre de tâches qu'on a terminé pour afficher l'avancement.
}
```