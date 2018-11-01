# ApiRaspbian

## Sensores DS18B20 
Se esta usando el Pin 7 (GPIO4) para w1. Alimentacion del Pin 1 (3.3v PWR) y tierra del Pin 6(GND).

![Alt text](RaspberryPi_PINS.png?raw=true "Pins")

Link a configuraciones de Pins.
https://es.pinout.xyz/pinout/1_wire

## Buid & Deploy en Raspberry Pi

Desde el directorio Api, ejecutamos los siguiente. Usuario pi, Password raspberry

```powershell
deploy-ubuntu.ps1 192.168.1.2 dotnetconsoleapps/apiraspbian pi
```

## Reles para control de Temperatura.
Se pretende utilizar el Pin 16 (GPIO 23) para la señal de frio y el Pin 18 (GPIO 24) para calor.
