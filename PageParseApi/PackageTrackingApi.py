import json
from UpsTracking import GetUpsTracking
from TrackingResult import TrackingResult

class TrackingApi:
    def __init__(self, headers):
        self.UPS_OATH_CLIENT_ID = headers.get("X-ups-client-id")
        self.UPS_OATH_CLIENT_SECRET = headers.get("X-ups-client-secret")

    def GetTrackingStatus(self, data):
        results = []
        for tracking in data["Items"]:
            if tracking is None or "Provider" not in tracking or "TrackingNumber" not in tracking or "Name" not in tracking:
                results.append(TrackingResult(status="Invalid data"))
                continue
            provider = tracking["Provider"]
            trackingNumber = tracking["TrackingNumber"]
            name = tracking["Name"]

            if provider.upper() == "UPS":
                results.append(GetUpsTracking(trackingNumber, name, self.UPS_OATH_CLIENT_ID, self.UPS_OATH_CLIENT_SECRET))
            else:
                results.append(TrackingResult(name=name, provider=provider, trackingNumber=trackingNumber, status="Invalid provider"))

        return { "Results": results }

def GetTrackingInfo(headers, body):
    trackingApi = TrackingApi(headers)
    results = trackingApi.GetTrackingStatus(body)
    return results

if __name__ == "__main__":
    with open("packagetrackingsampledata.json", "r", encoding="utf-8") as infile:
        sampleData = json.loads(infile.read())
    data = GetTrackingInfo(sampleData["Headers"], sampleData["Body"])
    print(data)
