# Unity QTE (Quick time event) system
Simple QTE system for Unity (Quick time event)
<br>
<b>Works only with old input system. Tested on versions: 2018.3.14f1, 2019.4.17f1. Other versions should also work.</b>

Contains:
- QTEManager
    - Main script for processing events
- QTEEvent
    - Event class with settings

Demo scene contains simple example of usage.
<br>
<b>How to use:<b>
1) Attach QTEManager script to any gameobject. Set Slow motion time scale param, QTE ui object, Text component and Image component
- Slow motion time scale
    - If event time type set to 'Slow', this param will be used as Unity time scale
- QTE UI object
    - Main UI for event. Will shown on event start and hide on end. 
- Text
    - On event start it will filled with key codes, which should be pressed
- Image
    - Used for time indication. You can set any sprite for this
2) Attach QTEEvent script to any object. Set all settings you want:
- Keys
    - List of keyboard keys, which player should press
- Time type
    - Used for setting Unity time scale
        - If normal, time scale = 1. If slow, QTEManager's 'Slow motion time scale' param will be used as time scale. If pause, time scale = 0
- Time
    - Time in seconds for which all keys should be pressed
- Fail on wrong key
    - If this set to true, event will be failed when player press keys, which are not in 'Keys' list
- Press type
    - Single
        - Player should just press keys at least once. Keys can be released after press
    - Simultaneously
        - Player should press all keys simultaneously, i.e. player should hold all needed keys. If some keys are released, event won't complete
- Event actions
    - On Start, On End
        - Will be called in any case
    - On Success
       - Will be called only if all keys were pressed
    - On Fail
        - Will be called if player failed event, i.e. time is up or wrong key was pressed (if 'fail on wrong key' enabled)
