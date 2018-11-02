# ApiRaspbian

## Sensores DS18B20 
El conexionado se realizo utilizando el siguiente esquema

![Alt text](http://www.bujarra.com/wp-content/uploads/2015/01/raspberry-DS18B20-bujarra.jpg)

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

Ya que la señal emitida por los pins GPIO son del tipo CMOS y para activar los reles necesitamos una señal del tipo TTL, utilizaremos un chip 74HC541 para convertir las señales. La conexion sera de la siguiente manera.

OE1 (Pin 1), OE2 (Pin 19) y GND (Pin 10) a Masa (algun pin GND de las raspberry), VCC (Pin 20) a 5V PWR. Luego utilizaremos A1 y A2 con señales de activacion, los cuales conectaremos con el Pin 16 (GPIO 23) y el Pin 18 (GPIO 24), luego las salidas Y1 e Y2 la conectaremos a los reles.

![Alt text](200px-74HC541E_pinouts.png?raw=true "Pins")
