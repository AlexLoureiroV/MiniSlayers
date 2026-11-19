# 🛡️ MiniSlayers

Mini-RPG de supervivencia en **Unity** desarrollado como práctica del
**Módulo 5 - C# Avanzado (I) - 5.3.5 Sistema de eventos Parte 3 y Práctica**.

Un héroe (EVA-01) se enfrenta al Ángel (Ramiel) en una arena. El jugador
no mueve al personaje: **pulsa teclas que lanzan eventos** y todo el juego
reacciona a través de un sistema de eventos desacoplado.

## 🎮 Controles

| Tecla | Acción | Evento |
|-------|--------|--------|
| **A** | El enemigo golpea al héroe | `OnKeyDamage` |
| **D** | Beber poción de curación | `OnKeyHeal` |
| **K** | Golpe al enemigo (+puntos, -25 vida al Ángel) | `OnKeyPoints` |
| **L** | Subir de nivel | `OnKeyAddLevel` |
| **H** | Mostrar / ocultar guía de controles | (extra) |
| **R** | Reiniciar partida (al terminar) | (extra) |
| cualquier | Saltar la intro (tras 2 s) | (extra) |

**Victoria:** el Ángel acumula 100 de daño (4 golpes) o se alcanza el nivel 5.
**Derrota:** la vida del héroe llega a 0.

## 🏗️ Arquitectura (sistema de eventos)

```
InputSystem (lanza eventos)
      │
      ▼
EventSystem (núcleo: listeners en Start)
      │ ejecuta
      ▼
Health / Points ──(onDamaged, onHealed, onGetPoints, onLevelUp, onDeath)──▶
      UIController (slider, textos TMP)  +  SoundController (playDamage, playDie...)
```

- **InputSystem**: lanza los 4 eventos `Action` del enunciado.
- **EventSystem**: referencia `_inputs`; añade listeners en `Start`.
- **Health**: daño, curación y muerte del héroe y del enemigo.
- **Points**: `CurrentPoints`, `CurrentLevel`, `AddLevel()` y evento `onLevelUp`.
- **UIController**: `updateLive`, `updatePoints` (ToString, alineado derecha), `UpdateLevel` (`_levelText`) y fin de partida con frase aleatoria.
- **SoundController**: `damageSound`/`dieSound` + `playDamage()`/`playDie()` y extras.

**Extras:** intro con tarjeta de título + cuenta atrás 3-2-1-¡YA! con pausa real,
guía de controles en pantalla (H), flash de daño en sprites y fondo con shader
de "calorímo", TextMeshPro con fuentes pixel personalizadas.

## 📁 Estructura

```
Assets/
├── Scripts/    (9 scripts, esquema de regiones, namespace MiniSlayers)
├── Scenes/
├── Sprites/    (EVA-01 y Ángel pixel art, fondo, intro)
├── Audio/      (sonidos CC0)
├── Fonts/      (fuentes TMP)
└── Shaders/    (HeatHaze.shader - distorsión del fondo)
Docs/           (UML del sistema en draw.io + PNG)
```

## 🚀 Cómo jugar

1. Abre la carpeta del proyecto en **Unity Hub** (2022.3 LTS o superior).
2. Abre la escena `Assets/Scenes/ArenaSupervivencia.unity`.
3. Pulsa **Play** y usa los controles de arriba.

---
Práctica académica — curso de C# avanzado con Unity.
