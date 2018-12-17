# Les mots-clés magiques

[Retour au sommaire](./../README.md)

## Le mot-clé `async`

### Quand peut-on l'utiliser ?

Ce mot-clé n'est lié qu'à l'**implémentation** d'une méthode. Il n'est lié ni à la signature ni au prototype. Ce qui implique que :

* Il n'apparaît pas dans une interface.
* Il n'apparaît pas dans une méthode abstraite.
* Il n'apparaît pas dans un type délégué.
* Il peut apparaître dans une méthode virtuelle et ne pas apparaître dans sa surchage, et vice-versa.

Cependant, pour utiliser ce mot-clé, la méthode en question doit avoir l'un de ces types de retour :

* `void`
* `Task`
* `Task<T>`
* `ValueTask<T>`

De plus, une telle méthode ne peut avoir ni de paramètre `out` ni de paramètre `ref`.

Voici donc quelques exemples de méthodes valides :

```csharp
public async void MyMethod1()
{
    // Code
}
```

```csharp
public async Task MyMethod2Async()
{
    // Code
}
```

### Qu'est-ce que ça change ?

#### Pour le développeur

Cela va changer quelques petites choses :

* Il peut utiliser le mot-clé `await` au sein de la méthode.
* Si la méthode a comme type de retour `Task`, `Task<T>` ou `ValueTask<T>`, la tâche retournée est démarrée.

> Ce qui signifie donc que la méthode est *awaitable*, et donc qu'on doit la suffixer par `Async`.

* Si la méthode a comme type de retour `Task`, l'implémentation se comporte comme celle d'une méthode de retour `void`.
* Si la méthode a comme type de retour `Task<T>` ou `ValueTask<T>`, l'implémentation se comporte comme celle d'une méthode de retour `T`.

```csharp
public async ValueTask<int> MyMethod3Async()
{
    return 3; // Ce code est parfaitement valide !
}
```

#### Pour le compilateur

Il va devoir travailler. Le code généré est beaucoup plus complexe, alors soyons heureux que le compilateur fasse tout pour nous.

Le site [SharpLab.io](https://sharplab.io/) permet de compiler et décompiler le code, afin de voir les optimisations et transformations effectuées par Roslyn. On peut ainsi voir que le code suivant...

```csharp
public class MyClass {
    public async Task MyMethod() {
    }
}
```

...dans lequel la méthode `MyMethod` est vide, est compilé en un code *IL* correspondant à ce code *C#* :

```csharp
public class MyClass
{
    [CompilerGenerated]
    private sealed class <MyMethod>d__0 : IAsyncStateMachine
    {
        public int <>1__state;

        public AsyncTaskMethodBuilder <>t__builder;

        public MyClass <>4__this;

        private void MoveNext()
        {
            int num = <>1__state;
            try
            {
            }
            catch (Exception exception)
            {
                <>1__state = -2;
                <>t__builder.SetException(exception);
                return;
            }
            <>1__state = -2;
            <>t__builder.SetResult();
        }

        void IAsyncStateMachine.MoveNext()
        {
            //ILSpy generated this explicit interface implementation from .override directive in MoveNext
            this.MoveNext();
        }

        [DebuggerHidden]
        private void SetStateMachine(IAsyncStateMachine stateMachine)
        {
        }

        void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
        {
            //ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
            this.SetStateMachine(stateMachine);
        }
    }

    [AsyncStateMachine(typeof(<MyMethod>d__0))]
    [DebuggerStepThrough]
    public Task MyMethod()
    {
        <MyMethod>d__0 <MyMethod>d__ = new <MyMethod>d__0();
        <MyMethod>d__.<>4__this = this;
        <MyMethod>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
        <MyMethod>d__.<>1__state = -1;
        AsyncTaskMethodBuilder <>t__builder = <MyMethod>d__.<>t__builder;
        <>t__builder.Start(ref <MyMethod>d__);
        return <MyMethod>d__.<>t__builder.Task;
    }
}
```

Il s'agit d'une *machine à états* permettant de sauvegarder le contexte (thread, variables...) et de le réappliquer quand nécessaire, tout en gérant le retour des méthodes et leurs exceptions.

### Renvoyer une `Task` ou `void`

Comme expliqué dans la partie précédente, une méthode avec `void` comme type de retour ne peut pas être attendue. `void` ne doit donc être utilisé que dans les rares cas où vous n'avez pas d'autre choix (abonnement à un événement, par exemple).

## Le mot-clé `await`

### Quand peut-on l'utiliser ?

Pour utiliser ce mot-clé, il y a deux conditions :

* La méthode courante doit être `async`.
* On ne peut attendre qu'une `Task`, `Task<T>` ou `ValueTask<T>` démarrée ou déjà finie (soit les mêmes conditions pour qu'une méthode soit *awaitable*).

### Comment l'utiliser ?

On peut l'utiliser directement devant l'appel à une méthode *awaitable* :

```csharp
public async Task MyMethod1Async()
{
    await MyMethod2Async();
}
```

On peut également garder une référence sur la tâche et utiliser `await` devant la référence :

```csharp
public async Task MyMethod3Async()
{
    Task myTask = MyMethod4Async();
    await myTask;
}
```

> Puisqu'une méthode *awaitable* renvoie une tâche démarrée, celle-ci débute dès l'appel à la méthode, donc avant le mot-clé `await`.

Si la tâche que l'on attend renvoie une valeur, `await` permet d'y accéder :

```csharp
public async Task MyMethod5Async()
{
    Task<int> myTask = MyMethod4Async();
    int result = await myTask;
}
```

> Pour voir un appel asynchrone attendu, exécutez le quatrième exemple de code : [Example.04.AsyncAwaitSimple.cs](../sources/CodeJedi.AsyncAwait/Examples/Example.04.AsyncAwaitSimple.cs)

## Les blocs

Comme on l'avait vu [précédemment](./part03.md), les précédents moyens de faire de l'asynchronisme utilisaient des *délégués*. Le code d'un *délégué* est dans un bloc à part, indépendant du bloc appelant. En revanche, en utilisant `async` et `await`, on n'utilise pas de délégué.

```csharp
public async Task MyMethod1Async()
{
    MyMethod2();
    await MyMethod3Async();
    MyMethod4();
    await MyMethod5Async();
}
```

Les appels aux méthodes 2, 3, 4 et 5 sont ainsi faites dans le même bloc. Ceci permet de faire ce genre de choses :

```csharp
public async Task MyMethod1Async()
{
    try
    {
        using (var myDisposableObject = new MyDisposableClass())
        {
            MyMethod2();
            if (myCondition)
            {
                await MyMethod3Async();
                MyMethod4();
            }
        }
        // L'objet myDisposableObject est Disposed

        await MyMethod5Async();
    }
    catch
    { 
        // En cas d'exception, d'une méthode synchrone ou asynchrone
    }
}
```

Et la logique d'exécution est exactement la même que si tout le code était synchrone.

[Suite](./part06.md)