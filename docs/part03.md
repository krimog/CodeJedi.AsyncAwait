# Quand on devait tout faire à la main

[Retour au sommaire](./../README.md)

Comme on l'a vu, ce qu'on cherche généralement avec l'asynchronisme c'est d'effectuer un traitement sur un autre thread, puis te revenir sur le thread appelant une fois le traitement fini. Voici quelques méthodes pour faire ça.

## BackgroundWorker

Le `BackgroundWorker` est une classe clé en main utilisant des événements. C'est simple à utiliser, cependant, l'utilisation d'événements complexifie le débugging.

```csharp
internal void BackgroundWorkerExample()
{
    var backgroundWorker = new BackgroundWorker();
    // La méthode abonnée sera exécutée sur un autre thread
    backgroundWorker.DoWork += BackgroundWorker_DoWork;
    // La méthode abonnée sera exécutée sur le thread UI, une fois le traitement fini.
    backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

    // Code exécuté sur le thread UI
    Processing.BeforeProcessing("On lance le traitement via un BackgroundWorker");

    backgroundWorker.RunWorkerAsync();
}

private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
{
    Processing.DoSomeHeavyProcessing();
}

private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
{
    // Code exécuté sur le thread UI
    // Une fois le traitement fini
    Processing.AfterProcessing("Le traitement du BackgroundWorker est terminé");
}
```

## Dispatcher

Le `Dispatcher` est un objet permettant d'exécuter du code sur le thread d'où il vient. On peut donc créer manuellement un nouveau thread sur lequel on exécutera le traitement lourd, puis on exécutera, grâce au dispatcher, le code d'un délégué sur le thread UI.

```csharp
public void DispatcherExample()
{
    // Code exécuté sur le thread UI
    Processing.BeforeProcessing("On lance le traitement manuellement");

    // On récupère le dispatcher du thread courant
    var dispatcher = Dispatcher.CurrentDispatcher;

    // Ce code permet de lancer l'exécution dans un autre thread
    Task.Run(() =>
    {
        // Code exécuté sur un autre thread
        Processing.DoSomeHeavyProcessing();

        // On appelle le dispatcher pour retourner sur le thread UI
        dispatcher.Invoke(() =>
        {
            // Code à nouveau exécuté sur le thread UI
            Processing.AfterProcessing("Le traitement est terminé et ce texte est affiché grâce au dispatcher");
        });
    });
}
```