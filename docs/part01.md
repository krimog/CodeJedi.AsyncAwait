# L'asynchronisme et le parallélisme

## Le thread

Imaginons un programme comme celui-ci :

```pseudo
repère 1
instruction 2
instruction 3
si condition 4
    aller au repère 1
instruction 5
```

Il s'agit d'une simple boucle `do while`. Le programme va être lu séquentiellement. Mais lu par quoi ? Par un thread / un fil d'exécution. Le thread sait où il en est de l'exécution du programme et n'est qu'à un seul endroit à la fois. Ainsi, le thread sait ce qu'il doit faire et comment savoir ce qu'il devra faire après (passer à l'instruction suivante).

Mais voilà : vu qu'un thread n'est qu'à un seul endroit à la fois, il ne peut faire qu'une chose à la fois. Et, parfois, ça nous suffit. Mais il y a deux cas où il va falloir utiliser plusieurs threads dans le même programme : lorsqu'on a besoin de **parallélisme** et quand on a besoin d'**asynchronisme**.

## Parallélisme

### Processeurs et cœurs logiciels

La très grande majorité des processeurs actuels ont plusieurs cœurs. Lorsque vous voyez un processeur avec 8 cœurs logiques, en pratique, ça signifie que le processeur peut traiter 8 threads à la fois, en parallèle.

### Effectuer des traitements en parallèle

Imaginons que nous voulions effectuer un traitement d'image complexe. Si le traitement d'une image prend 100 ms, effectuer ce traitement sur une image nous donne rapidement le résultat, tout va bien. Cependant, si l'on doit appliquer ce traitement sur 10 000 images, ça prendra `100 * 10 000 = 1 000 000 ms`, donc environ 17 minutes. C'est long.

Mais si mon processeur est capable de faire 8 choses à la fois, pourquoi ne pas tout simplement lui demander de traiter 8 images à la fois ?! On arrive à un temps total de `100 * (10 000 / 8) = 125 000 ms`, soit à peine plus de 2 minutes.

### On gagne du temps...

Le parallélisme est le fait d'exécuter plusieurs tâches en même temps (en parallèle). Et ça nous fait gagner du temps. C'est là l'objectif du parallélisme.

### ...mais pas toujours !

Dans l'exemple du traitement des 10 000 images, peut-on réellement espérer diviser le temps de traitement par 8 ? La réponse est non. Créer un thread est une opération coûteuse. Répartir les tâches peut également l'être. Mettre en commun les résultats aussi. Le temps de traitement individuel a donc été divisé par 8, mais on a rajouté des actions qui prennent un certain temps avant et après ces traitement.

Ce surcoût peut être négligeable, non négligeable, et parfois même supérieur au gain de temps. N'oublions pas non plus que tous les processeurs n'ont pas 8 cœurs logiques.

## Asynchronisme

### Tout n'est pas instantané

Il y a énormément de choses qui prennent du temps pour un programme.

* L'appel à une API
* La lecture d'un fichier
* L'écriture dans une base de données
* Un traitement lourd
* ...

Et que peut-on faire entre le moment où on commence, disons, l'appel à une API, et le moment où on reçoit la réponse ?

### Attendre ou ne pas attendre, telle est la question

Le plus simple est clairement de demander au thread d'attendre la réponse. On a donc un comportement **synchrone**. Une fois qu'on a la réponse, on peut l'utiliser, l'afficher, la transformer...

Si la réponse ne nous intéresse pas (ce qui est rare, car on voudra généralement savoir, au minimum, si l'appel s'est bien passé ou non), on n'attend pas la réponse et le thread peut exécuter les instructions suivantes. Ce comportement est nommé **fire and forget** (ce qui se traduit en "tire et oublie").

Attendre la réponse, c'est simple, mais ce n'est pas efficace. Comment faire pour pouvoir à la fois exécuter les instructions suivantes, mais également pouvoir utiliser la réponse ? En créant un deuxième thread, qui fera l'appel et attendra la réponse, puis l'utilisera comme on veut, on libère le premier thread, qui peut donc exécuter les instructions suivantes. On a un comportement dit **asynchrone**.

### Si tu crois que je n'ai que ça à faire...

L'idée derrière l'asynchronisme n'est pas la vitesse, mais de "réquisitionner" le thread le moins longtemps possible. En effet, bloquer certains threads peut être très problématique dans certains cas.

## Libérons les threads

### Client lourd

Lorsqu'on lance un client lourd avec une interface graphique, le programme démarre sur le thread principal, également appelé thread graphique ou thread UI. C'est lui qui gère tout l'affichage et c'est le seul à pouvoir tenir ce rôle. Si celui-ci est en train d'attendre, il ne peut pas s'occuper de l'affichage en même temps. Si le temps d'attente n'est que de quelques millisecondes, ça ne se voit pas. Si c'est plusieurs secondes, on voit que l'interface ne bouge plus, que plus rien ne répond : l'application *freeze*.

Effectuer l'attente sur un autre thread (avoir un comportement asynchrone) libère donc le thread UI, qui peut s'occuper de l'affichage. L'interface graphique répond, l'utiliateur est content.

### ASP.NET

