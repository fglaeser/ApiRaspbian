package main

import (
	"context"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"log"
	"net/http"
	"os"
	"path"
	"strconv"
	"time"

	"github.com/fglaeser/thermostat/internal/thermostat"
	"github.com/yryz/ds18b20"
	"golang.org/x/oauth2"
	"golang.org/x/oauth2/google"
	"google.golang.org/api/option"
	"google.golang.org/api/sheets/v4"
)

const (
	spreadsheetId string = "1RCRFRNucJB-fs4jyhIlPMfHnMOIv7xx81h1oeZx0j-g"
	configRange   string = "E4:E14"
	tempsRange    string = "A2:C50"
)

// Retrieve a token, saves the token, then returns the generated client.
func getClient(currentPath string, config *oauth2.Config) *http.Client {
	// The file token.json stores the user's access and refresh tokens, and is
	// created automatically when the authorization flow completes for the first
	// time.
	tokFile := path.Join(currentPath, "token.json")
	tok, err := tokenFromFile(tokFile)
	if err != nil {
		tok = getTokenFromWeb(config)
		saveToken(tokFile, tok)
	}
	return config.Client(context.Background(), tok)
}

// Request a token from the web, then returns the retrieved token.
func getTokenFromWeb(config *oauth2.Config) *oauth2.Token {
	authURL := config.AuthCodeURL("state-token", oauth2.AccessTypeOffline)
	fmt.Printf("Go to the following link in your browser then type the "+
		"authorization code: \n%v\n", authURL)

	var authCode string
	if _, err := fmt.Scan(&authCode); err != nil {
		log.Fatalf("Unable to read authorization code: %v", err)
	}

	tok, err := config.Exchange(context.TODO(), authCode)
	if err != nil {
		log.Fatalf("Unable to retrieve token from web: %v", err)
	}
	return tok
}

// Retrieves a token from a local file.
func tokenFromFile(file string) (*oauth2.Token, error) {
	f, err := os.Open(file)
	if err != nil {
		return nil, err
	}
	defer f.Close()
	tok := &oauth2.Token{}
	err = json.NewDecoder(f).Decode(tok)
	return tok, err
}

// Saves a token to a file path.
func saveToken(path string, token *oauth2.Token) {
	fmt.Printf("Saving credential file to: %s\n", path)
	f, err := os.OpenFile(path, os.O_RDWR|os.O_CREATE|os.O_TRUNC, 0600)
	if err != nil {
		log.Fatalf("Unable to cache oauth token: %v", err)
	}
	defer f.Close()
	json.NewEncoder(f).Encode(token)
}

func main() {

	configs := make(chan Config)
	temps := make(chan map[string]float64)

	ctx := context.Background()

	currentPath, err := os.Executable()
	if err != nil {
		log.Fatal("Unable to read executable path.")
	}
	currentPath = path.Dir(currentPath)

	b, err := ioutil.ReadFile(path.Join(currentPath, "credentials.json"))
	if err != nil {
		log.Fatalf("Unable to read client secret file: %v", err)
	}

	//If modifying these scopes, delete your previously saved token.json.
	authConfig, err := google.ConfigFromJSON(b, "https://www.googleapis.com/auth/spreadsheets")
	if err != nil {
		log.Fatalf("Unable to parse client secret file to config: %v", err)
	}
	client := getClient(currentPath, authConfig)

	srv, err := sheets.NewService(ctx, option.WithHTTPClient(client))
	if err != nil {
		log.Fatalf("Unable to retrieve Sheets client: %v", err)
	}
	go getConfigFromSheet(configs, srv, configRange, spreadsheetId)
	go getTemperatures(temps)
	//traer structura
	t := <-temps
	config := <-configs

	if t == nil || config == (Config{}) {
		fmt.Println("Cannot get temps or config.")
		return
	}

	fmt.Printf("Temp Target: %s\n", config.TargetTemp)
	fmt.Printf("Sensor In: %s\n", config.SensorInID)
	fmt.Printf("Sensor Out: %s\n", config.SensorOutID)
	fmt.Printf("Pin Cool: %s\n", config.PinCool)
	fmt.Printf("Pin Heat: %s\n", config.PinHeat)
	fmt.Printf("Diff Temp: %s\n", config.DiffTemp)

	for key, temp := range t {
		fmt.Printf("sensor: %s temperature: %.2f°C\n", key, temp)
	}

	//Obtengo valor de los pines
	pinCool, err := strconv.Atoi(config.PinCool)
	if err != nil {
		fmt.Println("Cannot convert Pin Cool to int.")
		return
	}
	pinHeat, err := strconv.Atoi(config.PinHeat)
	if err != nil {
		fmt.Println("Cannot convert Pin Heat to int.")
		return
	}
	tempOut := t[config.SensorOutID]
	tempIn := t[config.SensorInID]

	target, err := strconv.ParseFloat(config.TargetTemp, 64)
	if err != nil {
		fmt.Println("Cannot convert Target Temp to float.")
		return
	}
	diff, err := strconv.ParseFloat(config.DiffTemp, 64)
	if err != nil {
		fmt.Println("Cannot convert Diff Temp to float.")
		return
	}
	// Thermostat Logic
	// Con los valores, que tenemos de los pin, podemos ver en que estado esta. Idle, Heat, Cool.

	mode, err := thermostat.Run(uint8(pinCool), uint8(pinHeat), tempOut, target, diff)
	if err != nil {
		fmt.Printf("Error running Thermostat: %v", err)
		return
	}
	updateTemps(srv, tempsRange, spreadsheetId, tempIn, tempOut)
	updateMode(srv, "G1:2", spreadsheetId, mode)
}

func getConfigFromSheet(ch chan Config, srv *sheets.Service, configRange, spreadsheetID string) {
	resp, err := srv.Spreadsheets.Values.Get(spreadsheetId, configRange).Do()
	if err != nil {
		fmt.Printf("Unable to retrieve data from sheet: %v\n", err)
		ch <- Config{}
		return
	}
	var config Config
	config.TargetTemp = resp.Values[0][0].(string)
	config.SensorInID = resp.Values[4][0].(string)
	config.SensorOutID = resp.Values[5][0].(string)
	config.PinCool = resp.Values[6][0].(string)
	config.PinHeat = resp.Values[7][0].(string)
	config.DiffTemp = resp.Values[8][0].(string)
	ch <- config
}

func getTemperatures(ch chan map[string]float64) {

	sensors, err := ds18b20.Sensors()
	if err != nil {
		fmt.Printf("Unable to retrieve sensors: %v\n", err)
		ch <- nil
		return
	}
	temps := make(map[string]float64)
	for _, sensor := range sensors {
		t, err := ds18b20.Temperature(sensor)
		if err == nil {
			temps[sensor] = t
		} else {
			fmt.Printf("Unable to get temp from sensor %s, %v\n", sensor, err)
		}
	}
	ch <- temps
}

func updateTemps(srv *sheets.Service, cellRange, spreadsheetID string, tempIn, tempOut float64) {
	resp, err := srv.Spreadsheets.Values.Get(spreadsheetId, cellRange).ValueRenderOption("UNFORMATTED_VALUE").Do()
	if err != nil {
		fmt.Printf("Unable to retrieve data from sheet: %v\n", err)
		return
	}

	var vr sheets.ValueRange
	vr.Values = append([][]interface{}{{tempIn, tempOut, time.Now().Format("02/01/2006 15:04:05")}}, resp.Values...)
	vr.Values = vr.Values[:len(vr.Values)-1]
	_, err = srv.Spreadsheets.Values.Update(spreadsheetID, cellRange, &vr).ValueInputOption("RAW").Do()
	if err != nil {
		fmt.Printf("Error writing temps: %v\n", err)
	}
}

func updateMode(srv *sheets.Service, cellRange, spreadsheetID string, mode thermostat.Mode) {
	var vr sheets.ValueRange
	switch mode {
	case thermostat.Idle:
		vr.Values = append(vr.Values, []interface{}{"OFF"})
		vr.Values = append(vr.Values, []interface{}{"OFF"})
	case thermostat.Cool:
		vr.Values = append(vr.Values, []interface{}{"ON"})
		vr.Values = append(vr.Values, []interface{}{"OFF"})
	case thermostat.Heat:
		vr.Values = append(vr.Values, []interface{}{"OFF"})
		vr.Values = append(vr.Values, []interface{}{"ON"})
	}
	_, err := srv.Spreadsheets.Values.Update(spreadsheetID, cellRange, &vr).ValueInputOption("RAW").Do()
	if err != nil {
		fmt.Printf("Error writing temps: %v\n", err)
	}
}
