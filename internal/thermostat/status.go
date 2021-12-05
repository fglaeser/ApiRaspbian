package thermostat

import (
	"fmt"

	"github.com/stianeikeland/go-rpio"
)

type Mode int8

const (
	Idle Mode = iota
	Cool
	Heat
)

func Run(pinCool uint8, pinHeat uint8, temp float64, target float64, diff float64) (Mode, error) {
	cool, heat, err := getGPIOStatus(pinCool, pinHeat)

	if err != nil {
		fmt.Printf("Error getting pin values: %v\n", err)
		return Idle, err
	}

	fmt.Printf("Cool: %d, Heat: %d\n", cool, heat)

	mode := getMode(cool, heat)

	switch mode {
	case Idle:
		fmt.Println("Thermostat in Idle Mode")
		mode = checkIdle(temp, target, diff, pinCool, pinHeat)
	case Cool:
		fmt.Println("Thermostat in Cool Mode")
		mode = checkCool(temp, target, diff, pinCool)
	case Heat:
		fmt.Println("Thermostat in Heat Mode")
		mode = checkHeat(temp, target, diff, pinHeat)
	}
	return mode, nil
}

func getMode(cool int, heat int) Mode {
	if (cool == 0) && (heat == 0) {
		return Idle
	}
	if cool == 1 {
		return Cool
	}
	return Heat
}

func checkIdle(temp float64, target float64, diff float64, pinCool uint8, pinHeat uint8) Mode {
	if temp >= (target + diff) {
		fmt.Println("Thermostat goes from Idle Mode -> Cool Mode")
		pinOn(pinCool)
		return Cool
	}
	if temp <= (target - diff) {
		fmt.Println("Thermostat goes from Idle Mode -> Heat Mode")
		pinOn(pinHeat)
		return Heat
	}
	return Idle
}

func checkCool(temp float64, target float64, diff float64, pinCool uint8) Mode {
	if temp <= target {
		fmt.Println("Thermostat goes from Cool Mode -> Idle Mode")
		pinOff(pinCool)
		return Idle
	}
	return Cool
}

func checkHeat(temp float64, target float64, diff float64, pinHeat uint8) Mode {
	if temp >= target {
		fmt.Println("Thermostat goes from Heat Mode -> Idle Mode")
		pinOff(pinHeat)
		return Idle
	}
	return Heat
}

func pinOn(number uint8) error {
	err := rpio.Open()
	if err != nil {
		return err
	}
	defer rpio.Close()

	pin := rpio.Pin(number)
	pin.Output()
	pin.High()
	return nil
}

func pinOff(number uint8) error {
	err := rpio.Open()
	if err != nil {
		return err
	}
	defer rpio.Close()

	pin := rpio.Pin(number)
	pin.Output()
	pin.Low()
	return nil
}

func getGPIOStatus(cool uint8, heat uint8) (int, int, error) {

	err := rpio.Open()
	if err != nil {
		return 0, 0, err
	}
	defer rpio.Close()
	pinCool := rpio.Pin(cool)
	pinCool.Output()

	pinHeat := rpio.Pin(heat)
	pinHeat.Output()

	return int(pinCool.Read()), int(pinHeat.Read()), nil
}
