# Le besoin d'asynchronisme

[Retour au sommaire](./../README.md)

Il y a plusieurs cas où l'asynchronisme est essentiel, cependant, l'asynchronisme apporte un nouveau besoin.

## Libérons les threads

### Client lourd

Lorsqu'on lance un client lourd avec une interface graphique, le programme démarre sur le thread principal, également appelé thread graphique ou thread UI. C'est lui qui gère tout l'affichage. Si celui-ci est en train d'attendre, il ne peut pas s'occuper de l'affichage en même temps. Si le temps d'attente n'est que de quelques millisecondes, ça ne se voit pas. Si c'est plusieurs secondes, on voit que l'interface ne bouge plus, que plus rien ne répond : l'application *freeze*.

Effectuer l'attente sur un autre thread (avoir un comportement asynchrone) libère donc le thread UI, qui peut s'occuper de l'affichage. L'interface graphique répond, l'utiliateur est content.

### ASP.NET

Le besoin semble moins important, puisque ASP.NET (que ce soit pour un site web ou pour un service), ne fait que répondre à des requêtes. D'ailleurs, à combien de requêtes peut-il répondre simultanément ? Par défaut, il y a 100 threads (capables de répondre à une requête) disponibles par cœur logiciel. La 101ème requête renverra une réponse avec un statut 503 (service indisponible) indiquant que le serveur est encombré.

Mais on vient de voir qu'avec l'asynchronisme, on délègue l'attente à un autre thread (un qui ne fait pas partie des 100 threads évoqués) pour libérer le thread courant, qui peut alors commencer à traiter la requête suivante. Le traitement des requêtes commence donc tout de suite et sans encombrement du serveur, les utilisateurs sont contents.

## Le thread, c'était mieux celui d'avant

Comme je le disais, l'asynchronisme apporte également un nouveau besoin : celui de revenir sur le thread appelant.

### Client lourd

Là, c'est très simple : comme dit précédemment, tout l'affichage est géré par le thread UI. Cependant, c'est le seul à pouvoir tenir ce rôle. Ce qui signifie que toute modification d'affichage doit être faite sur le thread UI. Imaginons que nous ayons fait un traitement asynchrone (sur le thread 2), et que nous souhaitions afficher "Traitement terminé"... Il faut donc que le thread 2 indique au thread UI d'afficher ce texte.

Il existe plusieurs moyens d'effectuer ceci (Dispatcher, Background worker...) mais ce n'est jamais pratique.

### ASP.NET

`HttpContext` est une classe qui permet d'accéder à toutes les informations que l'on pourrait vouloir avoir sur la requête (la route, les paramètres, le corps, les cookies, la session...).

Cette classe contient une propriété statique `Current`. Cependant, une même application web gèrera des dizaines de requêtes simultanément, sur des threads différents. Chaque thread a donc un `HttpContext.Current` différent. Ce qui veut donc dire que lorsqu'on crée un nouveau thread, on ne peut plus accéder au contexte courant.

Il est toujours possible d'extraire le contexte dans une variable et de passer celle-ci dans le nouveau thread, mais c'est une action supplémentaire dont on se passerait bien.

[Suite](./part03.md)