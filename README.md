# Fallen Ashes

Un roguelike 2D en dark fantasy pixel art desarrollado en Unity 6 como Trabajo de Fin de Grado.

## Características

- **Combate**: Pistola que orbita al jugador, dispara con clic izquierdo, sigue al ratón
- **Enemigos**: Goblin verde (básico), Goblin rojo (tanque), Goblin jefe
- **Sistema de oleadas**: Salas con oleadas enemigas, puertas se abren al limpiar la sala
- **Almas**: Los enemigos sueltan almas al morir, se recogen automáticamente al pasar
- **Tienda de mejoras**: El Mago en el castillo vende mejoras (daño, cadencia, velocidad, vida máxima) con almas
- **Sistema de diálogos**: NPCs con diálogos condicionales (Rey, Mago, Guardias)
- **Jefe final**: Al derrotarlo aparece un portal de regreso al castillo
- **Pantalla de muerte**: Al morir se vuelve al castillo conservando almas y mejoras
- **Sistema de guardado**: Progreso persistente con PlayerPrefs
- **Menú principal**: Continuar, Nueva Partida, Opciones (volumen, resolución, pantalla completa)
- **Cinemática inicial**: Carrusel de lore + cinemática del Mago al empezar partida nueva
- **Pausa**: ESC para pausar con acceso a opciones y salir al menú

## Cómo jugar

| Tecla | Acción |
|-------|--------|
| WASD / Flechas | Moverse |
| Ratón | Apuntar |
| Clic izquierdo | Disparar |
| E | Interactuar / Recoger |
| ESC | Pausa |

## Escenas

1. **Menu**: Menú principal
2. **LoreScene**: Carrusel de imágenes de lore
3. **Castle_MainScene**: Castillo con NPCs, tienda y portal a la mazmorra
4. **DungeonScene**: Mazmorra con enemigos, oleadas y jefe final

## Mejoras

| Mejora | Costes por nivel | Efecto |
|--------|-----------------|--------|
| Daño | 3, 5, 8, 12, 18 | +0.5 de daño por nivel |
| Cadencia | 3, 5, 8, 12, 18 | -5% de tiempo entre disparos |
| Velocidad | 3, 5, 8, 12, 18 | +20% velocidad por nivel |
| Vida Máx. | 3, 5, 8, 12, 18 | +0.5 corazón por nivel |

## Tecnologías

- Unity 6
- C#
- TextMeshPro
- PlayerPrefs (save system)
