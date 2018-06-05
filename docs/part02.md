# Le besoin d'asynchronisme

[Retour au sommaire](./../README.md)

Il y a plusieurs cas où l'asynchronisme est essentiel, cependant, l'asynchronisme apporte un nouveau besoin.

## Libérons les threads

### Client lourd

Lorsqu'on lance un client lourd avec une interface graphique, le programme démarre sur le thread principal, également appelé thread graphique ou thread UI. C'est lui qui gère tout l'affichage. Si celui-ci est en train d'attendre, il ne peut pas s'occuper de l'affichage en même temps. Si le temps d'attente n'est que de quelques millisecondes, ça ne se voit pas. Si c'est plusieurs secondes, on voit que l'interface ne bouge plus, que plus rien ne répond : l'application *freeze*.

Effectuer l'attente sur un autre thread (avoir un comportement asynchrone) libère donc le thread UI, qui peut s'occuper de l'affichage. L'interface graphique répond, l'utiliateur est content.

### ASP.NET

Le besoin semble moins important, puisque ASP.NET (que ce soit pour un site web ou pour un service), ne fait que répondre à des requêtes. D'ailleurs, à combien de requêtes peut-il répondre simultanément ? Par défaut, il y a 100 threads (capables de répondre à une requête) disponibles par cœur logiciel. La 101ème requête devra attendre qu'un des 100 threads soit libéré pour que son traitement puisse commencer. Et si les 100 threads sont en train d'appeler une API qui, au final, ne répond pas, et qu'ils attendent tous le timeout, la pauvre petite requête supplémentaire devra attendre que les threads finisse d'attendre...

Mais on vient de voir qu'avec l'asynchronisme, on délègue l'attente à un autre thread (un qui ne fait pas partie des 100 threads évoqués) pour libérer le thread courant, qui peut alors commencer à traiter la requête suivante. Le traitement des requêtes commence donc tout de suite, les utilisateurs sont contents.

## Le thread, c'était mieux celui d'avant