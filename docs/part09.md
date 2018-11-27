# Bonnes pratiques

[Retour au sommaire](./../README.md)

## `ConfigureAwait`

Rappelons ce qu'il se passe au niveau des threads lors de l'utilisation d'un `await` :

```csharp
// Je suis sur le thread 1
synchronousAction();
// Là, je libère le thread 1
await asynchronousAction();
// Là, je retourne sur le thread 1
```

Mais si le thread 1 est déjà utilisé ailleurs pendant qu'on attendait `asynchronousAction()`, retourner sur le thread 1 peut prendre du temps. Or il peut ne pas être nécessaire de retourner sur le thread 1 pour l'action qui suit `asynchronousAction()`.

Il y a une méthode pour ça : `ConfigureAwait(false)`.

```csharp
// Je suis sur le thread 1
synchronousAction();
// Là, je libère le thread 1
await asynchronousAction().ConfigureAwait(false);
// Là, je suis sur un nouveau thread pour ne pas avoir à attendre le thread 1.
```

Lorsque le code qui suit un `await` ne nécessite pas d'être sur le même thread que le précédent, cette méthode peut faire gagner du temps.

## Deadlock

Non, je préfère être clair : un deadlock n'est **pas** une bonne pratique. La bonne pratique dont je vais parler permet d'éviter les deadlocks.

Déjà, qu'est-ce qu'un deadlock ? C'est une situation sans solution dans laquelle deux threads (ou plus) s'attendent mutuellement.

```csharp
void SynchronousMethod()
{
    // 1
    AsynchronousMethod().Wait();
    // 2
}

async Task AsynchronousMethod()
{
    // 3
    await Task.Delay(10);
    // 4
}
```

Ce code va produire un deadlock. Pourquoi ? À l'emplacement 1, on est sur le thread 1. On appelle `AsynchronousMethod()` sur ce même thread. Le fil d'exécution arrive donc à l'emplacement 3, sur ce thread 1. Là, on exécute un `Task.Delay(10)` et le `await` libère le thread 1. Le thread 1 est libéré, et arrive donc sur le `Wait()` de la `synchronousMethod()`. Vu qu'il n'y a pas de `await`, le thread 1 attend la fin de `AsynchronousMethod()`, sans être libéré.

Lorsque le `Task.Delay(10)` est terminé, pour passer à l'emplacement 4, il doit repasser sur le thread précédent, le thread 1. On attend donc que le thread 1 soit libéré pour passer à l'emplacement 4. Mais le thread 1 attend que `asynchronousMethod()`ait terminé.

Pour éviter ça, il existe plusieurs solutions :

* Eviter au maximum de mélanger le code synchrone et asynchrone.
* Mettre un `ConfigureAwait(false)` derrière `Task.Delay(10)`. Ainsi, il pourra atteindre l'emplacement 4 et la fin de la question sans le thread 1. L'exécution peut alors continuer.
* Démarrer `AsynchronousMethod()` sur un autre thread : `Task.Run(() => AsynchronousMethod()).Wait();`

## Renvoyer la tâche

On l'a vu précédemment, ajouter `async` à une méthode augmente la complexité et diminue les performances, puisqu'on crée une machine à états.

Si on n'a pas d'`await` au sein de la méthode, `async` n'est pas nécessaire.

Mais il existe un autre cas où l'on peut se dispenser d'`async`. Si la méthode contient un et un seul `await` (par chemin d'exécution) et que celui-ci est à la dernière instruction (de ce chemin), il est possible de renvoyer directement la tâche, sans que la méthode soit `async`.

```csharp
async Task MyMethodAsync()
{
    SynchronousAction();
    await AsynchronousAction();
}
```

peut être remplacé par

```csharp
Task MyMethodAsync()
{
    SynchronousAction();
    return AsynchronousAction();
}
```

> Attention :
>
> Si le `await` à enlever est au sein d'un bloc (`using`, `try/catch`...), le replacer par un `return` risque d'avoir des effets secondaires, puisqu'on sortira du bloc avant la fin de la tâche.

[Suite](./part10.md)