import requests
import json
from BrianTools.Tools import ApiError, IsNullOrEmpty
import datetime



def ParseFlight(fid, flight):
    if flight is None or type(flight) is not list or len(flight) < 12:
        return None
    if fid is None or type(fid) is not str:
        return None

    name = flight[0]
    if IsNullOrEmpty(name):
        return None
    lat = flight[1]
    lon = flight[2]
    if type(lat) is not float or type(lon) is not float:
        return None

    altitude = flight[4]
    if altitude is None or type(altitude) is not int:
        altitude = 0
    aircraft = flight[5]
    heading = flight[7]
    if heading is None or type(heading) is not float:
        heading = 0
    origin = flight[10]
    destination = flight[11]

    return {
        "fid": fid,
        "name": name,
        "latitude": lat,
        "longitude": lon,
        "altitude": altitude,
        "aircraft": aircraft,
        "heading": int(heading),
        "origin": origin,
        "destination": destination
    }

def GetFlightRadarData():
    timestamp = str(int(datetime.datetime.now().timestamp() * 1000))

    url = "https://data.rb24.com/live"

    headers = {
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:123.0) Gecko/20100101 Firefox/123.0",
        "origin": "https://www.radarbox.com",
        "referrer": "https://www.radarbox.com/"
    }

    querystring = {"aircraft":"","airport":"","fn":"","far":"","fms":"","zoom":"5","flightid":"","bounds":"56.961,17.114,34.041,-140.00","timestamp":"false","designator":"iata","showLastTrails":"true","tz":"local","vehicles":"true","ff":"false","os":"web","adsb":"true","adsbsat":"true","asdi":"true","ocea":"true","mlat":"true","sate":"true","uat":"true","hfdl":"true","esti":"true","asdex":"true","flarm":"true","aust":"true","diverted":"false","delayed":"false","ground":"true","onair":"true","blocked":"false","station":"","class[]":["?","A","B","C","G","H","M"],"airline":"","route":"","squawk":"","country":"","durationFrom":"0","durationTo":"14","distanceFrom":"0","distanceTo":"16000"}
    
    try:
        response = requests.request("GET", url, data="", headers=headers, params=querystring)
    except Exception as e:
                return ApiError(f"FlightRadar request exception: e")

    if response is None:
        return ApiError(f"FlightRadar request failed with null response")
    if response.status_code != 200:
        return ApiError(f"FlightRadar request failed with status {response.status_code}")
    if IsNullOrEmpty(response.text):
        return ApiError(f"FlightRadar request failed with empty text response")
    
    try:
        data = json.loads(response.text)
    except Exception as e:
        return ApiError(f"FlightRadar failed to deserialize response: {e}")
    
    if type(data) is not list or len(data) == 0:
        return ApiError(f"FlightRadar deserialized request was invalid")
    
    parsedFlights = []
    for flights in data:
        if type(flights) is not dict or len(flights) == 0:
            continue
        for fid, flight in flights.items():
            parsedFlight = ParseFlight(fid, flight)
            if parsedFlight is not None:
                parsedFlights.append(parsedFlight)

    if len(parsedFlights) == 0:
        return ApiError("FlightRadar failed to parse any flight results")
    
    return {
        "timestamp": timestamp,
        "flights": parsedFlights
    }

def GetOrDefault(dictionary, key):
    if (dictionary is None):
        return None
    if (type(dictionary) is not dict):
        return None
    if (len(dictionary) == 0):
        return None
    if (key is None):
        return None
    if (type(key) is not str):
        return None
    if (len(key) == 0):
        return None
    if (key not in dictionary):
        return None
    return dictionary[key]

def GetSingleFlightData(fid):
    if fid is None or type(fid) is not str or len(fid) == 0:
        return ApiError("Invalid request")
    
    
    url = "https://data.rb24.com/live-flight-info"

    querystring = {"fid":fid,"locale":"en"}

    payload = ""
    headers = {
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:123.0) Gecko/20100101 Firefox/123.0",
        "origin": "https://www.radarbox.com",
        "referrer": "https://www.radarbox.com/"
    }

    try:
        response = requests.request("GET", url, data="", headers=headers, params=querystring)
    except Exception as e:
                return ApiError(f"FlightRadar request exception: e")

    if response is None:
        return ApiError(f"FlightRadar request failed with null response")
    if response.status_code != 200:
        return ApiError(f"FlightRadar request failed with status {response.status_code}")
    if IsNullOrEmpty(response.text):
        return ApiError(f"FlightRadar request failed with empty text response")
    
    try:
        data = json.loads(response.text)
    except Exception as e:
        return ApiError(f"FlightRadar failed to deserialize response: {e}")
    if data is None or type(data) is not dict or len(data) == 0:
        return ApiError(f"FlightRadar deserialized request was invalid")
    
    try:
        result = {
            "origin": GetOrDefault(data, "aporgci"),
            "destination": GetOrDefault(data, "apdstci"),
            "departureRelative": GetOrDefault(data, "departureRelative"),
            "arrivalRelative": GetOrDefault(data, "arrivalRelative"),
            "duration": GetOrDefault(data, "duration"),
            "aircraft": GetOrDefault(data, "acd"),
            "airline": GetOrDefault(data, "alna"),
            "fnia": GetOrDefault(data, "fnia"),
        }
    except Exception as e:
        return ApiError(f"FlightRadar failed to parse response: {e}")
    return result
    
if __name__ == "__main__":
    # result = GetFlightRadarData()
    result = GetSingleFlightData("2117410970")
    pass
