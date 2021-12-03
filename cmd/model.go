package main

type Config struct {
	TargetTemp         string
	PollingInterval    int
	PollingPath        string
	FileName           string
	SensorInID         string
	SensorOutID        string
	PinCool            string
	PinHeat            string
	DiffTemp           string
	PublishInterval    int
	ThermostatInterval int
}
