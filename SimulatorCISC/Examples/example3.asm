MOV R0, 10
MOV R1, 7
ADD R1, R0
MOV (R1), 2
ET1:
ADD R2, (R1)
DEC R0
BNE ET1
END