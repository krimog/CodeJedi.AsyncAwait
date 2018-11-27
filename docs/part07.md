# `async`, `await` et synchronisme

## Une méthode *awaitable* n'est pas nécessairement asynchrone

Quoi ? Mais si une méthode est synchrone, pourquoi la faire *awaitable* ? Il peut y avoir plusieurs raisons de faire une telle chose, et en voici quelques unes :

### La méthode sera synchrone ou asynchrone selon certaines conditions

Un exemple simple est la récupération d'une donnée depuis un gestionnaire de cache.

Si la donnée n'est pas en cache, il faut obtenir sa valeur. Si la valeur était rapide à récupérer, il serait inutile de la mettre en cache. Si elle est longue à récupérer, il vaut mieux ne pas bloquer de thread pendant sa récupération, donc obtenir sa valeur de manière asynchrone. La méthode doit donc être asynchrone.

En revanche, si la donnée est en cache, une récupération synchrone de la valeur est plus performante. La méthode sera dans tous les cas *awaitable*, mais le traitement sera asynchrone ou non selon le besoin.

### La méthode sera synchrone ou asynchrone selon son implémentation

Là, la raison est encore plus simple : si l'interface est commune, le prototype l'est aussi, et donc le fait qu'une méthode soit *awaitable* l'est également. Si une seule implémentation de l'interface est asynchrone et nécessite donc d'être *awaitable*, alors toutes les autres implémentations devront également être *awaitable*, qu'elles soient synchrones ou asynchrones.

### Comment décider ?

"Qui peut le plus peut le moins". S'il est possible que votre méthode soit asynchrone, rendez-la *awaitable*. En revanche, si votre méthode n'est jamais asynchrone, pour des raisons de performance, ne la rendez pas *awaitable*.

## Ni `await` ni *warning*

Imaginons une méthode `Task<int> ComputeAsync(int a, int b)`, définie dans une interface, qui a une implémentation synchrone :

```csharp
public async Task<int> ComputeAsync(int a, int b)
{
    return a * b;
}
```

Cette méthode fonctionne. Cependant, elle vous affiche le *warning* suivant :

> This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.

ou en français :

> Cette méthode async n'a pas d'opérateur 'await' et elle s'exécutera de façon synchrone. Utilisez l'opérateur 'await' pour attendre les appels d'API non bloquants ou 'await Task.Run(…)' pour effectuer un travail utilisant le processeur sur un thread d'arrière-plan.

Même si un *warning* n'empêche pas la compilation, il vaut mieux les éviter. Il faut d'abord comprendre pourquoi on a ce *warning*. Le mot-clé `async` va créer toute une machine à états, qui n'a cependant d'intérêt que pour gérer les `await`, or il n'y en a pas dans la méthode. Il faudrait donc qu'elle ne soit pas `async`. Mais comme on a dit qu'il s'agissait d'une méthode définie dans une interface, on ne peut pas changer le type de retour : `Task<int>`.

Reprennons les conditions pour qu'une méthode soit *awaitable* :

* La méthode renvoie une `Task`, `Task<T>` ou `ValueTask<T>`.
* La tâche renvoyée est démarrée ou déjà finie.

Le premier point, c'est bon. Mais comment créer une tâche déjà finie à partir de la valeur qu'on a ?

```csharp
public Task<int> ComputeAsync(int a, int b)
{
    int result = a * b;
    return Task.FromResult<int>(result); // Le type générique peut être inféré
}
```

La méthode statique `Task<T> Task.FromResult<T>(T result)` va exactement faire ça : créer une tâche terminée avec `result` comme résultat.

Et si la méthode ne doit pas renvoyer de résultat (donc une simple `Task` au lieu d'une `Task<T>`) ? Dans ce cas, il suffit de renvoyer la propriété statique `Task Task.CompletedTask`, qui, comme son nom l'indique, correspond juste à une tâche terminée.

## `Task<T>` vs `ValueTask<T>`

Ça fait déjà un certain temps que je parle de `ValueTask<T>`, mais que je n'utilise que des `Task<T>`.

`ValueTask<T>` est une structure capable de gérer aussi bien une `Task<T>` qu'un simple objet `T`.

Créer une `Task<T>` est une opération lourde. Si l'objectif est de faire un traitement asynchrone, c'est totalement justifié. En revanche, en créer une pour l'utiliser de manière synchrone avec un `Task.FromResult(result)` est lourd.

C'est pour cela que `ValueTask<T>` a été créé : être capable de gérer les opérations asynchrones avec des `Task<T>` et les opérations synchrones qui n'ont pas besoin que l'on en crée.

* Si l'intégralité des implémentations sont synchrones, la méthode doit être synchrone.
* Si la majorité des implémentations sont synchrones, la méthode doit renvoyer une `ValueTask<T>`.
* Si la majorité des implémentations sont asynchrones, la méthode doit renvoyer une `Task<T>`.