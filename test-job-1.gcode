G90 ; Use absolute positioning
G28 ; Home all axes
M190 S40 ; Set the bed temperature and wait for the machine to come up to temp.
M109 S210 ; Set hotend temperature and wait for the machine to come up to temp.
G1 Z15 F6000 ; Lift the printerhead from the bed
G1 X65 Y20 ; avoid the clips on the bed
G1 Z0.1 F3000 ; get ready to prime
G92 E0 ; reset extrusion distance
G1 X120 Y20 E10 F600 ; prime nozzle
G1 X150 Y20 F5000 ; quick wipe
M190 S0 ; Cool down the bed
M109 S0 ; Cool down the hotend
G92 E0 ; Reset extrusion distance
G1 Z15 F6000 ; Lif the printer head