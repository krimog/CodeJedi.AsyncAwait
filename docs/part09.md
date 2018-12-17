# Annulation et progression

[Retour au sommaire](./../README.md)

De ce qu'on a vu, `async` et `await` ont de l'intérêt quand on doit faire des traitements longs. Cependant, pour de tels traitements, on va souvent vouloir être capable d'annuler ou au moins de voir la progression de celui-ci.

## Annulation

Pour annuler une tâche en cours, on va utiliser les `CancellationToken`, un mécanisme qui existait déjà avant l'apparition des tâches.

### Rendre la tâche annulable

Une méthode asynchrone annulable aura ainsi un nouveau paramètre de type `CancellationToken`, qu'elle pourra également fournir à ses sous-méthodes. Comme le fait d'annuler une tâche est généralement un comportement exceptionnel, l'idée est de lever une exception (de type `OperationCanceledException`) si la tâche est annulée. Pour lever une telle exception, l'objet `CancellationToken` a une méthode toute prête : `ThrowIfCancellationRequested()`.

```csharp
public async Task MyMethodAsync(CancellationToken cancellationToken)
{
    for(; ; ) // On peut, par exemple, avoir une boucle
    {
        // À chaque itération de la boucle, 
        // on lève une exception si une annulation est demandée.
        cancellationToken.ThrowIfCancellationRequested();

        // En passant le paramètre à la sous-méthode, 
        // la sous-méthode se chargera elle-même de lever l'exception.
        await MyOtherMethodAsync(cancellationToken);
    }
}
```

Il est parfois nécessaire de nettoyer l'état de la méthode avant de lever l'exception afin de garder un état du programme stable. Il suffit alors d'utiliser la propriété `IsCancellationRequested` du token.

```csharp
public async Task MyMethodAsync(CancellationToken cancellationToken)
{
    IsMyMethodRunning = true;
    for (; ; )
    {
        // Si une annulation est demandée
        if (cancellationToken.IsCancellationRequested)
        {
            // On nettoie l'état
            IsMyMethodRunning = false;

            // Puis on lève l'exception
            throw new OperationCanceledException(cancellationToken);
        }

        // Puisque la sous-méthode peut aussi lever une exception,
        // on va la placer dans un bloc try/catch
        try
        {
            await MyOtherMethodAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // On nettoie l'état
            IsMyMethodRunning = false;

            // Et on renvoie l'exception
            throw;
        }
    }
    IsMyMethodRunning = false;
}
```

### Demander l'annulation

On a vu comment utiliser un objet de type `CancellationToken`, mais comment le créer et l'utiliser ? C'est très simple : il suffit de créer un objet de type `CancellationTokenSource` et d'envoyer la valeur de sa propriété `Token` aux méthodes annulables. La méthode `Cancel()` de cet objet permet, quant à elle, de demander l'annulation.

```csharp
var cts = new CancellationTokenSource();
var runningTask = MyMethodAsync(cts.Token);

// Puis on demande l'annulation avec la méthode Cancel()
if (theTaskNeedsToBeCanceled)
{
	cts.Cancel();
}
```

> Vous pouvez lancer une tâche et l'annuler avec le neuvième exemple de code : [Example.09.CancelTask.cs](../sources/CodeJedi.AsyncAwait/Examples/Example.09.CancelTask.cs)

## Progression

Dans la partie précédente, nous avons vu que grâce à la méthode `WhenAll`, on peut facilement voir l'avancement de plusieurs tâches. Voici donc comment voir l'avancement au sein d'une même tâche. On va utiliser l'interface `IProgress<T>`.

D'un côté, nous aurons donc la méthode asynchrone qui prendra un paramètre de type `IProgress<T>`, sur lequel nous appelerons la méthode `Report()`. `T` peut être de n'importe quel type.

* `T` pourra être un `int` pour un avancement de 0 à 100.
* `T` pourra être un `double` pour un avancement de 0 à 1.
* `T` pourra être un `int[]` pour plusieurs avancement indépendants.
* `T` pourra être un `enum` indiquant les différents étapes du process.
* `T` pourra être un `string` pour indiquer textuellement l'étape en cours.

```csharp
public async Task MyMethodAsync(IProgress<int> progress)
{
	for (int i = 0; i < 100; i++)
	{
		progress?.Report(i);
		await MyOtherMethodAsync();
	}
}
```

> Attention :
>
> La méthode appelante peut tout-à-fait ne pas chercher à afficher de progression. Elle enverra donc `null` pour le paramètre `IProgress<T>`. Vérifiez donc toujours la non-nullité du paramètre avant de l'utiliser.

D'un autre côté, nous appellerons cette méthode en lui fournissant une implémentation de cette interface, par exemple `Progress<T>`. Le constructeur de cette classe prend en paramètre une `Action<T>` qui sera appelée quand la progression de la tâche changera.

```csharp
await MyMethodAsync(new Progress<int>(progress => MyProgressBar.Value = progress));
```

> Pour voir la progression en action, exécutez le dixième exemple de code : [Example.10.Progress.cs](../sources/CodeJedi.AsyncAwait/Examples/Example.10.Progress.cs)

[Suite](./part10.md)