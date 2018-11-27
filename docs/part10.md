# Conclusion

[Retour au sommaire](./../README.md)

L'ajout des mots-clés `async` et `await` a permis de libérer les threads importants dès que possible, de manière à la fois simple et efficace.

L'appel de méthodes en parallèle est devenu encore plus facile, ce qui pousse le développeur à paralléliser ce qui peut l'être, et ainsi obtenir une application plus performante.

En revanche, utilisés à mauvais escient, ces mots-clés peuvent avoir de mauvaises conséquences au niveau performances, voire aboutir à des deadlocks. Il faut donc être vigilant sur la façon de les utiliser. 