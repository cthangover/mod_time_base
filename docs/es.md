# Time Engine — el reloj del juego, el día y la noche, las luces

Time Engine lleva el reloj del juego y hace que las escenas lo sigan: un mismo lugar se ve distinto por la mañana, al mediodía, por la tarde y de noche. Además ilumina las escenas con lámparas y ventanas, y al anochecer te da una lámpara portátil. Es un mod de motor: lo incluyen juegos como Cthangover y los RPG de tiles, y su trabajo se ve en cada escena, no en una pantalla propia.

## El reloj

La hora se muestra en la esquina superior izquierda de la pantalla. Un segundo real es un minuto del juego, así que una hora de juego pasa en un minuto real. Algunas acciones consumen tiempo de juego al instante: por ejemplo, cocinar o fabricar adelanta el reloj lo que dura la receta.

El calendario es simplificado: meses de 30 días y 12 meses al año.

## El día y la noche

El fondo de cada escena se dibuja en cuatro estados — **mañana**, **día**, **tarde** y **noche** — y la imagen pasa suavemente de uno a otro:

```text
00 ─ noche ─ 04 ⇢ 06 ─ mañana ─ 09 ⇢ 11 ─── día ─── 17 ⇢ 19 ─ tarde ─ 21 ⇢ 22 ─ noche ─ 24
                  (⇢ = un fundido de dos horas de un estado al siguiente)
```

![La misma escena, el molino junto a la villa en Cthangover, a las 07:00, 13:00, 18:00 y 23:30: mañana cálida, mediodía brillante, el declinar de la tarde y la noche con ventanas encendidas.](assets/day-cycle.jpg)

## Las luces

De noche una escena está oscura salvo por sus luces. Ventanas, velas y lámparas brillan donde las puso el autor de la escena, y cada luz tiene su color, radio e intensidad.

![Noche junto a la villa a las 23:33: solo brillan las ventanas; el icono del farol arriba de la pantalla es la lámpara portátil.](assets/night.jpg)

**La lámpara portátil.** De 22:00 a 06:00 aparece un farol en la parte superior de la pantalla. Arrástralo sobre la escena para iluminar el punto que quieras mirar.

![La cocina de Cthangover, iluminada por su vela y su lámpara.](assets/kitchen-lights.jpg)

## Para autores

**Colocar luces en una escena** se hace desde su escenario con `light_set` y una lista de luces. Las posiciones son fracciones de la pantalla (de 0 a 1), así que encajan en cualquier tamaño de ventana; hasta 10 luces por escena, más la lámpara portátil:

```scenario
light_set [{"x":0.673,"y":0.461,"radius":500,"influence":1,"color":"#ffff00"},{"x":0.135,"y":0.315,"radius":500,"influence":1,"color":"#ffff00"}]
```

- `x`, `y` — posición como fracción del ancho y el alto;
- `radius` — hasta dónde llega la luz, en píxeles;
- `influence` — intensidad de 0 a 1;
- `color` — un color en hexadecimal.

Un `light_set` vacío quita las luces de la escena. El editor de luces de Editor Tools las coloca con el ratón y te escribe esta línea. La acción de escenario `lighting.clear_map` apaga la iluminación por profundidad de una escena cuando pasas a una escena que no la usa.

**Fondos que conocen la noche.** Un fondo se ilumina correctamente cuando tiene al lado un mapa de profundidad, llamado como la imagen con `_depth` (por ejemplo `bar.png` y `bar_depth.png`). Un fondo también puede tener un acompañante `_albedo`. Sin estos mapas, las luces de la escena no tienen forma sobre la que brillar.

**Otros mods** leen el mismo reloj: el mapa de tiles del RPG sigue el mismo día que los fondos, y el cambio de hora se anuncia a todos los mods una vez por minuto de juego.

## Si algo falla

1. **Una escena sigue clara de noche.** Su fondo no tiene mapa de profundidad, o la escena no usa sombreado según la hora.
2. **No hay farol de noche.** Aparece solo de 22:00 a 06:00 y solo en los juegos cuya interfaz lo incluye.
