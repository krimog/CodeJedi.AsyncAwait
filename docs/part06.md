# Un exemple concret

[Retour au sommaire](./../README.md)

## Transformer une méthode synchrone en méthode asynchrone

Prenons l'exemple suivant, qui récupère le contenu textuel d'une simple requête HTTP :

```csharp
public string GetResponseContent(string url)
{
    WebRequest request = WebRequest.Create(url);
    WebResponse response = request.GetResponse();
    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
    {
        string result = reader.ReadToEnd();
        return result;
    }
}
```

On va le transformer en code asynchrone de deux manières :

* En utilisant les méthodes asynchrones déjà existantes (c'est la bonne chose à faire).
* En rendant nous-même les méthodes asynchrones (dans le cas où elles n'existent pas).

### Les modifications à la déclaration de la méthode

On va vouloir que la méthode puisse attendre (utiliser le mot-clé `await`). Cela implique que la méthode doit être déclarée comme `async`.

```csharp
public async string GetResponseContent(string url)
```

Cependant, une méthode `async` ne peut renvoyer que `void`, `Task`, `Task<T>` ou `ValueTask<T>`. Evidemment, vu que notre méthode a un retour, on élimine les deux premiers choix. Pour le moment, je ne parlerai pas des différences entre `Task<T>` et `ValueTask<T>`. On choisira `Task<T>`.

```csharp
public async Task<string> GetResponseContent(string url)
```

Mais une méthode renvoyant une `Task<T>` démarrée est *awaitable*. On va donc suffixer la méthode par `Async`.

```csharp
public async Task<string> GetResponseContentAsync(string url)
```

### Utiliser les méthodes asynchrones existantes

La méthode `GetResponseContent` contient 2 appels potentiellement qui peuvent être longs (un appel rapide et non coûteux n'a pas besoin d'être asynchrone) : `request.GetResponse()` et `reader.ReadToEnd()`. Ça tombe bien, chacune de ces méthodes existe aussi en version asynchrone. On va donc utiliser respectivement `request.GetResponseAsync()` et `reader.ReadToEndAsync()`, et attendre les tâches retournées grâce à `await` :

```csharp
public async Task<string> GetResponseContentAsync(string url)
{
    WebRequest request = WebRequest.Create(url);
    WebResponse response = await request.GetResponseAsync();
    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
    {
        string result = await reader.ReadToEndAsync();
        return result;
    }
}
```

Et c'est tout ! Nous avons maintenant une méthode capable d'obtenir le contenu d'une requête HTTP de manière asynchrone.

### Et s'il n'y a pas de méthode asynchrone disponible

Il suffit alors de créer une `Task<T>` démarrée qui exécute le code dans un autre thread. C'est exactement ce que faire la méthode statique `Task.Run(Func<T> delegate)`. On va donc remplacer les appels à `request.GetResponse()` et `reader.ReadToEnd()` par `Task.Run(() => request.GetResponse())` et `Task.Run(() => reader.ReadToEnd())`, puis attendre ces tâches grâce à `await` :

```csharp
public async Task<string> GetResponseContentAsync(string url)
{
    WebRequest request = WebRequest.Create(url);
    WebResponse response = await Task.Run(() => request.GetResponse());
    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
    {
        string result = await Task.Run(() => reader.ReadToEnd());
        return result;
    }
}
```

## L'arbre d'appel

### Le problème du retour

On a transformé la méthode `GetResponseContent` en `GetResponseContentAsync`. Mais nous avons changé son type de retour de `string` en `Task<string>`.

### Restons asynchrones

Si j'attends la tâche renvoyé de manière synchrone, je perds tout l'intérêt d'avoir passé ma méthode en asynchrone, puisque le thread appelant ne sera pas libéré (puisqu'il attend de manière synchrone).

Il va donc falloir que j'utilise `await` dans la méthode appelante. Cela implique qu'elle devienne `async`. Cela changera aussi son type de retour en `Task` ou `Task<T>`. On ne fait donc que reporter le problème d'un niveau. **Il faudra donc transformer l'arbre d'appel en asynchrone**.

### Transformer, mais jusqu'où ?

Aussi loin qu'il est nécessaire d'attendre, c'est-à-dire généralement jusqu'au point d'entrée de l'actions courante.

* En ASP.NET Core, les *actions* des *contrôleurs* peuvent renvoyer des `Task` ou `Task<T>`.
* En client lourd, la méthode abonnée à l'événement déclenchant l'action (`Click` par exemple) sera `async void`.
* En WPF / UWP, la méthode `Execute` de la classe implémentant `ICommand` sera `async void`.
* Les tests unitaires peuvent renvoyer des `Task`.

### Dans le doute, renvoyons une tâche

Si l'on veut qu'une méthode soit *awaitable*, il **faut** qu'elle renvoie une tâche (que ce soit une `Task`, `Task<T>` ou `ValueTask<T>`). Or, on **peut** rendre une méthode `async` et lui faire renvoyer une tâche sans avoir d'`await` à l'intérieur (ça va juste marquer un *warning*, dont on parlera plus tard).

Ainsi, en partant du principe qu'une méthode aura potentiellement des appels asynchrones, on évite d'avoir à toucher après coup à tout l'arbre d'appel.